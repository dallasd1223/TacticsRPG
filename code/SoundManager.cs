using Sandbox;

public class SoundManager : Component
{
	public static SoundManager Instance;

	[Property] public int wow {get; set;} = 52;
	[Property] public SoundEvent HealSound {get; set;}

	protected override void OnAwake()
	{
		if(Instance is null)
		{
			Instance = this;
		}
	}

	protected override void OnStart()
	{
		Log.Info($"HealSound is {HealSound}");
	}

	public void PlaySound(string s)
	{
		var sound = GetSoundFromString(s);
		Log.Info($"Sound Valid? {sound.IsValid()}");
		Sound.Play(sound);

	}

	private SoundEvent GetSoundFromString(string s)
	{
		switch(s)
		{
			case "HEAL":
				Log.Info($"Returning Heal Sound");
				return HealSound;
			default:
				Log.Info("Returning Null");
				return null;
		}
	}

	public void Play()
	{
		Log.Info("Playing Sound");
		Sound.Play(HealSound);
	}
}

