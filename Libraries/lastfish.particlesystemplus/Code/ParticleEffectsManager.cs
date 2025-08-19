using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using Sandbox;

[Title( "Particle Effects Manager" )]
[Category( "Particle" )]
[Description(
	"This component let's you manage the TimeScale , the pause and restart of every particle effect component present in the childrens of this component gameobject so it is easier to preview the whole system" )]
public sealed class ParticleEffectsManager : Component, Component.ExecuteInEditor
{
	[Property] List<ParticleEffect> ParticleEffects { get; set; }

	[Property] List<ParticleEmitter> ParticleEmitters { get; set; }


	[Property, Group( "Parameters" ), Range( -1f, 1f, 0.1f )]
	float TimeScale { get; set; } = 1f;

	[JsonIgnore] public bool IsPaused { get; set; }
	[JsonIgnore] public bool IsRestart { get; set; }

	[JsonIgnore]
	public float LongestDuration
	{
		get
		{
			var duration = 0f;
			foreach ( var emitter in ParticleEmitters )
			{
				if ( emitter.Duration + emitter.Delay > duration )
				{
					duration = emitter.Duration + emitter.Delay;
				}
			}
			return duration;
		}
	}

	[Property, Group( "Actions" )] private bool Emit { get; set; }

	[JsonIgnore, Range( 0f, 1f, 0.001f )] public float PlayBack { get; set; }

	[JsonIgnore] public bool IsPlayBack { get; set; }

	[Property, Range( 0, 10, 0.001f )] public float DelayEffect { get; set; }

	public int ParticleCount
	{
		get
		{
			int count = 0;
			foreach ( var particleEffect in ParticleEffects )
			{
				count += particleEffect.ParticleCount;
			}

			return count;
		}
	}

	public float PlaybackTime
	{
		get
		{
			float playbackTime = 0;
			foreach ( var emitter in ParticleEmitters )
			{
				if ( emitter.IsValid() && emitter.time > playbackTime )
				{
					playbackTime = emitter.time;
				}
			}

			return playbackTime;
		}
	}

	protected override void OnEnabled()
	{
		ParticleEffects = GameObject.GetComponentsInChildren<ParticleEffect>().ToList();
		ParticleEmitters = GameObject.GetComponentsInChildren<ParticleEmitter>().ToList();

		if ( Scene.IsEditor ) return;
		foreach ( var particleEmitter in ParticleEmitters )
		{
			particleEmitter.Delay += DelayEffect;
		}
	}

	protected override void OnUpdate()
	{
		if ( !Scene.IsEditor ) return;

		if ( Emit ) EmitParticles();

		if ( IsRestart ) Restart();


		if ( IsPlayBack )
		{
			foreach ( var particleEffect in ParticleEffects )
			{
				PlayBacking( particleEffect );
			}
		}
		else
		{
			foreach ( var particleEffect in ParticleEffects )
			{
				TimeScaling( particleEffect );
			}
		}
	}

	private void EmitParticles()
	{
		Emit = false;
		foreach ( var effect in ParticleEffects )
		{
			effect.Emit( 0, 0 );
		}
	}


	private void TimeScaling( ParticleEffect particleEffect )
	{
		particleEffect.TimeScale = TimeScale;
		var particleEmitter = particleEffect.Components.Get<ParticleEmitter>();
		if ( TimeScale < 0 && particleEmitter.time <= 0 && !IsPaused )
		{
			var timing = particleEmitter.Duration - 0.01f;
			SetParticulesTiming( particleEffect, timing );
			particleEmitter.time = timing;
		}
		else
		{
			particleEffect.Paused = IsPaused;
		}
	}

	private void PlayBacking( ParticleEffect particleEffect )
	{
		particleEffect.TimeScale = 0;
		var particleEmitter = particleEffect.Components.Get<ParticleEmitter>();
		var remappedTiming = PlayBack.Remap( 0, 1f, 0, particleEmitter.Duration + particleEmitter.Delay );
		particleEmitter.time = remappedTiming;
		SetParticulesTiming( particleEffect, remappedTiming );
		
	}

	private void Restart()
	{
		IsPaused = false;
		IsRestart = false;
		foreach ( var particleEffect in ParticleEffects )
		{
			particleEffect.Clear();
			particleEffect.ResetEmitters();
		}
	}

	private void SetParticulesTiming( ParticleEffect particleEffect, float timing )
	{
		var particleEmitter = particleEffect.Components.Get<ParticleEmitter>();
		particleEmitter.time = timing;
		foreach ( var particule in particleEffect.Particles )
		{
			particule.Age = timing;
		}
	}
}
