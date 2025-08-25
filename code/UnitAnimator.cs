using Sandbox;
using SpriteTools;
using System;

namespace TacticsRPG;

[Category("Unit")]
public sealed class UnitAnimator : Component
{
	private Unit Self {get; set;}
	[Property] public SpriteComponent UnitSprite {get; set;}
	public Vector3 StartPosition;
	public bool hasStarted = false;
	public bool jitter = false;
	public float elapsed = 0f;

	public event Action AnimationEvent;

	protected override void OnAwake()
	{
		Self = GetComponent<Unit>();
		UnitSprite = GetComponent<SpriteComponent>();
	}

	protected override void OnStart()
	{
		UnitSprite.OnBroadcastEvent += OnBroadcastEvent;
	}

	protected override void OnUpdate()
	{
		if(jitter)
		{
			SpriteJitter(0.5f);
		}
	
	}

	public void AssignAnimation()
	{
		if(Self.Move.Moving)
		{
			UnitSprite.PlayAnimation("idle");
			return;
		}
		if(Self.Stats.CurrentHP >= 10)
		{
			UnitSprite.PlayAnimation("idle");
		}
		else if(Self.Stats.CurrentHP > 0 && Self.Stats.CurrentHP < 10)
		{
			UnitSprite.PlayAnimation("low");
		}
	}
	public void OnBroadcastEvent(string name)
	{
		if(name == "attackend")
		{
			Log.Info("Attack Animation Has Ended");

		}
	}

	

	public void PlayAnimation(string name, Action<string> act = null)
	{
		UnitSprite.PlayAnimation(name);
		if(act is not null)
		{
			UnitSprite.OnBroadcastEvent += act;
		}
		
		AnimationEvent?.Invoke();
		Log.Info($"Playing Animation {name}");
	}

	public void SpriteJitter(float duration)
	{
		if(!hasStarted)
		{
			elapsed = 0f;
			hasStarted = true;
			StartPosition = this.GameObject.WorldPosition;
		}
		if(elapsed <= duration)
		{
			elapsed += Time.Delta;
			this.GameObject.WorldPosition = StartPosition + new Vector3(Game.Random.Float(0,1),Game.Random.Float(0,1),Game.Random.Float(0,1));	
		}

	}

	public void EndJitter()
	{
		jitter = false;
		this.GameObject.WorldPosition = StartPosition;
		hasStarted = false;
	}
}

public enum SpriteAnimationType
{
	Idle,
	Walking,
	Jump,
	Attack,
}
