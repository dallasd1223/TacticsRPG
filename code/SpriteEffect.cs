using Sandbox;

public sealed class SpriteEffect : Component
{
	[Property] public GameObject DamageNum {get; set;}

	public static SpriteEffect Instance;

	protected override void OnAwake()
	{
		if(Instance is null)
		{
			Instance = this;
		}
	}
}
