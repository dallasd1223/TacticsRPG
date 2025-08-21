using Sandbox;
using SpriteTools;
using System;

namespace TacticsRPG;

public sealed class DamageNumber : Component
{
	[Property] SpriteComponent sprite {get; set;}


	protected override void OnAwake()
	{
		if(sprite is null)
		{
			sprite = GetComponent<SpriteComponent>();
		}

	}

	protected override void OnStart()
	{
		this.GameObject.MoveTo(this.GameObject.WorldPosition + new Vector3(0,0,90), 0.5f, EasingType.OutElastic);
		this.GameObject.ScaleTo(new Vector3(0.2f,0.2f,0.2f), 0.1f, EasingType.OutSine);
		sprite.FadeTo(0,0.25f)
			.SetDelay(0.75f)
			.OnCompleteCall(() => DestroySelf());
		
	}
	protected override void OnUpdate()
	{
		Log.Info($"Tint Color: {sprite.Tint}");

	}
	public void DestroySelf()
	{
		this.GameObject.Destroy();
	}
}
