using Editor;
using Sandbox.UI;
using Button = Editor.Button;
using ControlSheet = Editor.ControlSheet;

namespace Sandbox;


public class ParticleEffectManagerWidget : EditorTool<ParticleEffectsManager>
{
	ParticleSystemToolWindow window;
	public override void OnEnabled()
	{
		window = new ParticleSystemToolWindow();
		AddOverlay( window, TextFlag.RightBottom, 10 );
	}

	public override void OnUpdate()
	{
		window.ToolUpdate();
	}

	public override void OnDisabled()
	{

	}

	public override void OnSelectionChanged()
	{
		var effect = GetSelectedComponent<ParticleEffectsManager>();
		window.OnSelectionChanged( effect );
	}

	class ParticleSystemToolWindow : WidgetWindow
	{
		private ParticleEffectsManager targetComponent;
		Button PauseButton;

		FloatSlider PlaybackSlider;
		
		static bool IsClosed = false;
		
		public ParticleSystemToolWindow()
		{
			ContentMargins = 0;
			Layout = Layout.Column();
		}
		
		public void ToolUpdate()
		{
			if ( !targetComponent.IsValid() )
				return;
			var remappedTiming = targetComponent.PlaybackTime.Remap( 0, targetComponent.LongestDuration, 0, 1f );
			PlaybackSlider.Value = remappedTiming;
		}

		public void OnSelectionChanged( ParticleEffectsManager effect )
		{
			targetComponent = effect;

			Rebuild();
		}

		private void Rebuild()
		{
			Layout.Clear( true );
			Layout.Margin = 0;
			Icon = IsClosed ? "" : "shower";
			IsGrabbable = !IsClosed;
			MaximumWidth = 230;

			UpdateTitle();

			if ( IsClosed )
			{
				var closedRow = Layout.AddRow();
				closedRow.Add( new IconButton( "shower", () => { IsClosed = false; Rebuild(); } ) { ToolTip = "Open", FixedHeight = HeaderHeight, FixedWidth = HeaderHeight, Background = Color.Transparent } );
				return;
			}

			var headerRow = Layout.AddRow();
			headerRow.AddStretchCell();
			headerRow.Add( new IconButton( "close", CloseWindow ) { ToolTip = "Close", FixedHeight = HeaderHeight, FixedWidth = HeaderHeight, Background = Color.Transparent } );

			var slider = new SliderControl();
			
			var buttonRow = Layout.AddRow();
			buttonRow.Spacing = 2;
			buttonRow.AddStretchCell();

			PauseButton = new Button( "Pause" )
			{
				Clicked = PauseToggle,
				FixedWidth = 40
			};

			PlaybackSlider = new FloatSlider( this )
			{
				Minimum = 0,
				Maximum = 1,
				EditingStarted =  () => TogglePlayback(),
				EditingFinished = () => TogglePlayback(),
				OnValueEdited = () =>
				{
					targetComponent.PlayBack = PlaybackSlider.Value;
				},
			};

			buttonRow.Add( PauseButton );
			buttonRow.Add( new Button( "Restart" ) { Clicked = Restart } );
			buttonRow.AddStretchCell();

			if ( targetComponent.IsValid() )
			{
				PauseButton.Text = targetComponent.IsPaused ? "Play" : "Pause";

				var sheet = new ControlSheet();
				var so = targetComponent.GetSerialized();
				sheet.AddProperty( this, x => x.ParticleCount );
				sheet.AddProperty( this, x => x.PlaybackTime );
				Layout.Add( sheet );
			}

			var sliderRow =  Layout.AddRow();
			sliderRow.Add( PlaybackSlider );
			

			Layout.Margin = 4;
		}

		private void TogglePlayback()
		{
			if(targetComponent.IsValid()) targetComponent.IsPlayBack = !targetComponent.IsPlayBack;
		}

		void Restart()
		{
			if ( !targetComponent.IsValid() )
				return;

			targetComponent.IsRestart = true;
		}
		
		void PauseToggle()
		{
			if ( !targetComponent.IsValid() )
				return;

			targetComponent.IsPaused = !targetComponent.IsPaused;

			if ( PauseButton.IsValid() )
				PauseButton.Text = targetComponent.IsPaused ? "Play" : "Pause";
		}

		public float PlaybackTime
		{
			get
			{
				if ( !targetComponent.IsValid() )
					return 0.0f;
				
				return targetComponent.PlaybackTime;
			}
		}

		private int ParticleCount
		{
			get
			{
				if ( !targetComponent.IsValid() )
					return 0;

				return targetComponent.ParticleCount;
			}
		}

		private void UpdateTitle()
		{
			if ( !IsClosed )
			{
				if ( targetComponent.IsValid() && targetComponent.GameObject.IsValid() )
				{
					WindowTitle = $"Particles - {targetComponent.GameObject.Name}";
				}
				else
				{
					WindowTitle = "Particles";
				}
			}
			else
			{
				WindowTitle = "";
			}
		}
		
		void CloseWindow()
		{
			IsClosed = true;
			Rebuild();
			Position = Parent.Size - 32;
		}
	}
	
}
