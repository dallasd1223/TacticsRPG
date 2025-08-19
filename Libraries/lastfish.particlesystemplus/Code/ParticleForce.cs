using Sandbox;

public sealed class ParticleForce : ParticleController
{
	
	[Property]
	public ParticleFloat ForceX { get; set; }
	[Property]
	public ParticleFloat ForceY { get; set; }
	[Property]
	public ParticleFloat ForceZ { get; set; }
	
	[Property]
	public float ForceScale { get; set; }

	protected override void OnParticleStep( Particle particle, float delta )
	{
		var forceX = ForceX.Evaluate( particle, 62611 );
		var forceY = ForceY.Evaluate( particle, 62611 );
		var forceZ = ForceZ.Evaluate( particle, 62611 );
		particle.Velocity += (new Vector3( forceX, forceY, forceZ ) / 100) * ParticleEffect.TimeScale * ForceScale;
	}

	protected override void OnUpdate()
	{
		/*foreach ( var particle in ParticleEffect.Particles )
		{
			var forceX = ForceX.Evaluate( particle, 62611);
			var forceY = ForceY.Evaluate( particle, 62611);
			var forceZ = ForceZ.Evaluate( particle, 62611);
			particle.Velocity += new Vector3( forceX, forceY, forceZ ) * particle.TimeScale;
		}*/
	}
	
	
	
	
}
