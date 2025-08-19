namespace Sandbox;

public class ParticleEffectPlus : ParticleController
{
	[Property] private ParticleEffect ParticleEffect { get; set; }

	[Property] private ParticleEmitter ParticleEmitter { get; set; }

	[Property, FeatureEnabled( "ShapeXYZ" )]
	private bool ShapeXyzEnabled { get; set; }

	[Property, Feature( "ShapeXYZ" )] private ParticleFloat ScaleX { get; set; }
	[Property, Feature( "ShapeXYZ" )] private ParticleFloat ScaleY { get; set; }
	[Property, Feature( "ShapeXYZ" )] private ParticleFloat ScaleZ { get; set; }

	[Property, FeatureEnabled( "Shape" )] private bool ShapeEnabled { get; set; }

	[Property, FeatureEnabled( "Shape" )] private ParticleFloat Scale { get; set; }


	protected override void OnParticleStep( Particle particle, float delta )
	{
		SetSize( particle, delta );
	}

	private void SetSize( Particle particle, float delta )
	{
		Vector3 size;
		if ( ShapeXyzEnabled )
		{
			var x = ScaleX.Evaluate( ParticleEmitter.Delta, 65343 );
			var y = ScaleY.Evaluate( ParticleEmitter.Delta, 65343 );
			var z = ScaleZ.Evaluate( ParticleEmitter.Delta, 65343 );
			particle.Size = new Vector3( x, y, z );
		}

		if ( ShapeEnabled )
		{
			particle.Size = Scale.Evaluate( ParticleEmitter.Delta, 65343 );
		}
	}
}
