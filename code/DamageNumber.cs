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
		this.GameObject.MoveTo(this.GameObject.WorldPosition + new Vector3(0,0,10), 1, EasingType.OutElastic);
		sprite.FadeTo(0,0.5f)
			.SetDelay(0.5f)
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
