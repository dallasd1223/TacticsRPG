using Sandbox;

public sealed class MusicPlayer : Component
{
	[Property] public SoundEvent BattleTheme {get; set;}
	[Property] public bool PlayMusic {get; set;} = true;
	public SoundHandle handle;

	protected override void OnStart()
	{
		if(!PlayMusic) return;
		if(BattleTheme.IsValid())
		{
			handle = Sound.Play(BattleTheme);
		}
	
	}

	public void StopTheme()
	{
		handle.Stop();
	}
}
