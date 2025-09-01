using Sandbox;

public sealed class MusicPlayer : Component
{
	[Property] public SoundEvent BattleTheme {get; set;}
	[Property] public SoundEvent EndTheme {get; set;}
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

	public void StopBattleTheme()
	{
		handle.Stop();
	}

	public void PlayEndTheme()
	{
		handle = Sound.Play(EndTheme);
	}
}
