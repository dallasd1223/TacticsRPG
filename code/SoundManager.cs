using Sandbox;

public class SoundManager : SingletonComponent<SoundManager>
{

	[Property] public SoundEvent UIPress1 {get; set;}
	[Property] public SoundEvent UIBack1 {get; set;}

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
			case "UIPRESS1":
				return UIPress1;
			case "UIBACK1":
				return UIBack1;
			default:
				Log.Info("Returning Null");
				return null;
		}
	}
}

