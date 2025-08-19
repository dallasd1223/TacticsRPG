using System;

namespace Sandbox;
[Title("Particle Ring Emitter Even")]
[Category( "Particles" )]	
[Description("Let's you ensure that the particles are placed evenly around the ring when emitted. Simply check the \"Even Angle\" checkbox and you are set !")]
public class ParticleRingEmitterEven : ParticleEmitter
{
	[Property] public ParticleFloat Radius { get; set; } = 50.0f;
	[Property] public ParticleFloat Thickness { get; set; } = 10.0f;
	[Property, Range( 0, 360 )] public ParticleFloat AngleStart { get; set; } = 0.0f;
	[Property, Range( 0, 360 )] public ParticleFloat Angle { get; set; } = 360.0f;
	[Property, Range( 0, 1 )] public ParticleFloat Flatness { get; set; } = 0.0f;
	[Property, Range( -100, 100 )] public ParticleFloat VelocityFromCenter { get; set; } = 0.0f;
	[Property, Range( -100, 100 )] public ParticleFloat VelocityFromRing { get; set; } = 0.0f;
	[Property] public bool EvenAngle { get; set; } = false;

	private float _angleStep = 0.0f;

	protected override void OnUpdate()
	{

		
	}
	
	public override bool Emit( ParticleEffect target )
	{
		
		if ( target.Particles.Count == 0 )
		{
			_angleStep = 0.0f;
		}
		
		var angle = 0f;
		if ( !EvenAngle )
		{
			angle = Random.Shared.Float( 0, Angle.Evaluate( Delta, EmitRandom ).DegreeToRadian() );
			angle += AngleStart.Evaluate( Delta, EmitRandom ).DegreeToRadian();
		}
		else
		{
			angle = _angleStep;
			AngleStepBurst();
		}

		var x = MathF.Sin( angle );
		var y = MathF.Cos( angle );

		var size = new Vector3( x, y, 0 ) * Radius.Evaluate( Delta, 0 );
		var ringOffset = Vector3.Zero;

		var thickness = Thickness.Evaluate( Delta, EmitRandom );

		if ( thickness > 0 )
		{
			ringOffset = Vector3.Random * thickness;
			ringOffset.z *= (1 - Flatness.Evaluate( Delta, EmitRandom ));

			size += ringOffset;
		}

		size = (size * WorldScale) * WorldRotation;

		var p = target.Emit( WorldPosition + size, Delta );
		if ( p is not null )
		{
			var velFromCenter = VelocityFromCenter.Evaluate( Delta, EmitRandom );
			if ( velFromCenter != 0 )
			{
				p.Velocity += (size.Normal * velFromCenter);
			}

			var velFromRing = VelocityFromRing.Evaluate( Delta, EmitRandom );
			if ( velFromRing != 0 )
			{
				ringOffset = (ringOffset * WorldScale) * WorldRotation;
				p.Velocity += (ringOffset.Normal * velFromRing);
			}
		}

		return true;
	}

	private void AngleStepBurst()
	{
		
		
		_angleStep += Angle.Evaluate( Delta, EmitRandom ).DegreeToRadian() / Burst ;
		
	}
}
