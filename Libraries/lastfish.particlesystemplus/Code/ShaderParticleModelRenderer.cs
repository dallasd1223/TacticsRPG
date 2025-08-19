using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using Sandbox;

namespace Sandbox;

[Title( "Shader Particle Model Renderer" )]
[Category( "Particles" )]
[Description(
	"Adds the \"Particle Shader\" feature which let's you set shader parameters on the particles model using the particle system !" )]
public class ShaderParticleModelRenderer : ParticleController, Component.ExecuteInEditor
{
	private ParticleModelRenderer _particleModelRenderer { get; set; } = new ParticleModelRenderer();


	/**
	 *  These are the dictionaries used to set the values for the render attributes in the particle system
	 */
	[Property, FeatureEnabled("ParticleShader")]
	public bool ParticleShaderEnabled { get; set; } = true;
	
	
	[Property, Group( "ColorParameters" ), Feature( "ParticleShader" )]
	public Dictionary<String, ParticleGradient> Colors { get; set; }

	[Property, Group( "FloatParameters" ), Feature( "ParticleShader" )]
	public Dictionary<String, ParticleFloat> Floats { get; set; }

	[Property, Group( "Float2Parameters" ), Feature( "ParticleShader" )]
	public Dictionary<String, Vector2> Floats2 { get; set; }

	[Property, Group( "Float4Parameters" ), Feature( "ParticleShader" )]
	public Dictionary<String, Vector4> Floats4 { get; set; }

	[Property, Group( "Textures" ), Feature( "ParticleShader" )]
	public Dictionary<String, Texture> Textures { get; set; }

	[Property, Group( "DynamicCombos" ), Feature( "ParticleShader" )]
	public Dictionary<String, int> DynamicCombos { get; set; }
	

	
	

	[RequireComponent] public new ParticleEffect ParticleEffect { get; set; }

	[Property, Order( -100 ), InlineEditor( Label = false ), Group( "Advanced Rendering", StartFolded = true )]
	public RenderOptions RenderOptions => _particleModelRenderer.RenderOptions;
	
	
	protected override void OnStart()
	{
		Floats = Floats == null ? new Dictionary<string, ParticleFloat>() : Floats;
		Floats2 = Floats2 == null ? new Dictionary<string, Vector2>() : Floats2;
		Floats4 = Floats4 == null ? new Dictionary<string, Vector4>() : Floats4;
		Textures = Textures == null ? new Dictionary<string, Texture>() : Textures;
		Colors = Colors == null ? new Dictionary<String, ParticleGradient>() : Colors;
		DynamicCombos = DynamicCombos == null ? new Dictionary<string, int>() : DynamicCombos;
		
		if( ParticleShaderEnabled ) ReadAttributes();
	}
	
	[Button, Feature("ParticleShader")]
	private void ReadAttributes()
	{
		if ( MaterialOverride == null || !FileSystem.Mounted.FileExists( MaterialOverride.Shader.ResourcePath )) return;
		AttributesParser<ParticleFloat, ParticleGradient> parser = new AttributesParser<ParticleFloat, ParticleGradient>(new ParticleAttributeTypeSet());
		parser.Floats = Floats;
		parser.Floats2 = Floats2;
		parser.Floats4 = Floats4;
		parser.Textures = Textures;
		parser.DynamicCombos = DynamicCombos;
		parser.Colors = Colors;
		parser.ParseAttributes( MaterialOverride.Shader.ResourcePath );
		
	}

	public sealed class ModelEntry
	{
		private Model _model;

		[KeyProperty]
		public Model Model
		{
			get => _model;
			set
			{
				if ( _model == value )
					return;

				_model = value;

				MaterialGroup = default;
				BodyGroups = _model?.DefaultBodyGroupMask ?? default;
			}
		}

		[Model.MaterialGroup, ShowIf( nameof(HasMaterialGroups), true )]
		public string MaterialGroup { get; set; }

		[Model.BodyGroupMask, ShowIf( nameof(HasBodyGroups), true )]
		public ulong BodyGroups { get; set; }

		[Hide, JsonIgnore] public bool HasMaterialGroups => Model?.MaterialGroupCount > 0;

		[Hide, JsonIgnore] public bool HasBodyGroups => Model?.BodyParts.Sum( x => x.Choices.Count ) > 1;

		public static implicit operator ModelEntry( Model model ) => new() { Model = model };
	}

	[Hide, Obsolete( "Use Choices" )] public List<Model> Models { get; set; } = new();

	[Property] public List<ModelEntry> Choices { get; set; } = new List<ModelEntry> { Model.Cube };

	[Property] public Material MaterialOverride { get; set; }

	[Property, Feature( "ScaleXYZ" )] public ParticleFloat ScaleX { get; set; } = 1;
	[Property, Feature( "ScaleXYZ" )] public ParticleFloat ScaleY { get; set; } = 1;
	[Property, Feature( "ScaleXYZ" )] public ParticleFloat ScaleZ { get; set; } = 1;

	[Property, FeatureEnabled( "ScaleXYZ" )]
	public bool ApplyScaleXYZ { get; set; } = true;

	[Property] public float Scale { get; set; } = 1;
	
	[Property] public bool CastShadows { get; set; } = true;

	[Property] public Allignement Allignement { get; set; }

	protected override void OnParticleCreated( Particle p )
	{
		var particleModel = new CustomParticleModel( this );
		p.AddListener( particleModel, this );
		
	}

	public override int ComponentVersion => 1;

	[JsonUpgrader( typeof(ParticleModelRenderer), 1 )]
	static void Upgrader_v1( JsonObject obj )
	{
		if ( obj.TryGetPropertyValue( "Models", out var node ) )
		{
			var choices = new JsonArray();

			foreach ( var model in node.AsArray() )
			{
				if ( model is null )
					continue;

				choices.Add( new JsonObject { ["Model"] = model.ToString() } );
			}

			obj["Choices"] = choices;
			obj.Remove( "Models" );
		}
	}
	
	
}

public enum Allignement
{
	SimulationSpace,
	FaceCamera,
	FaceVelocity,
	
}





public class CustomParticleModel : Particle.BaseListener
{
	public ShaderParticleModelRenderer Renderer;

	public SceneObject so;

	private ParticleAttributesSetter _particleAttributesSetter;

	public CustomParticleModel( ShaderParticleModelRenderer renderer )
	{
		Renderer = renderer;
	}

	public override void OnEnabled( Particle p )
	{
		var entry = Random.Shared.FromList( Renderer.Choices );
		var model = entry?.Model;
		so = new SceneObject( Renderer.Scene.SceneWorld, model ?? Model.Cube );
		so.Batchable = false;
		if ( model is not null )
		{
			so.MeshGroupMask = entry.BodyGroups;
			so.SetMaterialGroup( entry.MaterialGroup );
		}

		if ( !Renderer.ParticleShaderEnabled ) return;
		_particleAttributesSetter = new ParticleAttributesSetter( so.Attributes, p );
		SetRenderAttributes();
	}

	public override void OnDisabled( Particle p )
	{
		if ( !so.IsValid() ) return;
		so.Delete();
	}

	public override void OnUpdate( Particle p, float dt )
	{
		if ( !so.IsValid() ) return;


		var angles = ComputeRotation( p );
		
		var scale = p.Size * Renderer.WorldScale;
		if ( Renderer.ApplyScaleXYZ )
		{
			scale *= EvaluateScale( p );
		}

		so.Transform = new Transform( p.Position, angles, scale * Renderer.Scale );
		so.ColorTint = p.Color.WithAlphaMultiplied( p.Alpha );
		so.Flags.CastShadows = Renderer.CastShadows;
		so.SetMaterialOverride( Renderer.MaterialOverride );
		
		if(Renderer.ParticleShaderEnabled) _particleAttributesSetter.SetAttributes();
		

		if ( Renderer.RenderOptions != null )
		{
			Renderer.RenderOptions.Apply( so );
		}
	}

	


	private Vector3 EvaluateScale( Particle p )
	{
		
		var scaleX = Renderer.ScaleX.Evaluate( p, 6211 );
		var scaleY = Renderer.ScaleY.Evaluate( p, 6211 );
		var scaleZ = Renderer.ScaleZ.Evaluate( p, 6211 );
		return new Vector3( scaleX, scaleY, scaleZ );
	}


	private Angles ComputeRotation(Particle p)
	{
		var angles = new Rotation();
		switch ( Renderer.Allignement )
		{
			case Allignement.FaceCamera :
				if ( Renderer.Scene.Camera == null ) break;
				var dir = Renderer.Scene.Camera.WorldPosition - p.Position;
				angles = Rotation.LookAt( dir, Vector3.Up ) * p.Angles.ToRotation();
				break;
			case Allignement.FaceVelocity :
				angles = Rotation.LookAt( p.Velocity.Normal, Vector3.Up ) * p.Angles.ToRotation();
				break;
			case Allignement.SimulationSpace :
				angles = Renderer.ParticleEffect.LocalSpace.Evaluate( p,65373 ) <= 1 ? Renderer.WorldRotation.Angles() : Rotation.Identity.Angles(); 
				angles *= p.Angles;
				break;
		}
		return angles;
	}
	private void SetRenderAttributes()
	{
		_particleAttributesSetter.Floats = Renderer.Floats;
		_particleAttributesSetter.Floats2 = Renderer.Floats2;
		_particleAttributesSetter.Floats4 = Renderer.Floats4;
		_particleAttributesSetter.Textures = Renderer.Textures;
		_particleAttributesSetter.Colors = Renderer.Colors;
		_particleAttributesSetter.DynamicCombos = Renderer.DynamicCombos;
		_particleAttributesSetter.SetAttributes();
	}
}
