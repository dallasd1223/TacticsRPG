using System;
using System.Collections.Generic;

namespace Sandbox;

/**
 * Simple class designed to set render attributes accounting for particule timings
 */
public class ParticleAttributesSetter
{
	private RenderAttributes _renderAttributes;
	private Particle _particle;
	
	public Dictionary<String, ParticleGradient> Colors { get; set; }
	public Dictionary<String, ParticleFloat> Floats { get; set; }
	public Dictionary<String, Vector2> Floats2 { get; set; }
	public Dictionary<String, Vector4> Floats4 { get; set; }
	public Dictionary<String, Texture> Textures { get; set; }
	public Dictionary<String, int> DynamicCombos { get; set; }
	
	public ParticleAttributesSetter(RenderAttributes renderAttributes, Particle particle)
	{
		_renderAttributes = renderAttributes;
		_particle = particle;
	}

	public void SetAttributes()
	{
		SetDynamicCombos();
		SetFloats();
		SetColors();
		SetFloats2();
		SetFloats4();
		SetTextures();
	}

	
	private void SetFloats( )
	{
		foreach ( var Float in Floats )
		{
			ParticleFloat particleFloat = Float.Value;
			float value = particleFloat.Evaluate( _particle, 6211 );
			_renderAttributes.Set( Float.Key, value );
		}
	}

	private void SetColors()
	{
		foreach ( var colorAttribute in Colors )
		{
			_renderAttributes.Set( colorAttribute.Key, colorAttribute.Value.Evaluate( _particle, 6211 ) );
		}
	}
	
	private void SetFloats2()
	{
		foreach ( var float2 in  Floats2 )
		{
			_renderAttributes.Set( float2.Key, float2.Value );
		}
	}
	
	private void SetFloats4()
	{
		foreach ( var float4 in Floats4 )
		{
			_renderAttributes.Set( float4.Key, float4.Value );
		}
	}
	
	private void SetTextures( )
	{
		foreach ( var texture in Textures )
		{
			_renderAttributes.Set( texture.Key, texture.Value );
		}
	}
	
	private void SetDynamicCombos()
	{
		foreach ( var dynamicCombo in DynamicCombos )
		{
			_renderAttributes.SetCombo( dynamicCombo.Key, dynamicCombo.Value );
		}
	}
	
	
}
