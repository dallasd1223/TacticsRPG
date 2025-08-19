using System;
using System.Collections.Generic;
using Sandbox;
namespace Sandbox;
[Title("ModelShaderAttributes")]
[Category("Shaders")]
[Description("Let's you set the render attributes of the scene object of a ModelRenderer")]
public sealed class ModelShaderAttributes : Component, Component.ExecuteInEditor
{
	[Property] private ModelRenderer ModelRenderer { get; set; }
	[Property, Group( "Floats1" )] public Dictionary<String, float> Floats { get; set; } 
	[Property, Group("Floats2")] public Dictionary<String, Vector2> Floats2 { get; set; }
	[Property, Group("Colors")] public Dictionary<String, Color> Colors {get; set;}
	[Property, Group("Textures")] public Dictionary<String, Texture> Textures { get; set; }
	
	[Property, Group("Floats4")] public Dictionary<String, Vector4> Floats4 { get; set; }
	
	[Property, Group("DynamicCombos")] public Dictionary<String, int> DynamicCombos { get; set; }
	
	[Property] public bool Batchable { get; set; } = true;
	
	
	protected override void OnStart()
	{
		ModelRenderer.SceneObject.Batchable = Batchable;
		Floats = Floats == null ? new Dictionary<string, float>() : Floats;
		Floats2 = Floats2 == null ? new Dictionary<string, Vector2>() : Floats2;
		Floats4 = Floats4 == null ? new Dictionary<string, Vector4>() : Floats4;
		Textures = Textures == null ? new Dictionary<string, Texture>() : Textures;
		Colors = Colors == null ? new Dictionary<String, Color>() : Colors;
		DynamicCombos = DynamicCombos == null ? new Dictionary<string, int>() : DynamicCombos;
		
	}
	[Button]
	private void ReadAttributes()
	{
		if ( ModelRenderer.MaterialOverride == null || !FileSystem.Mounted.FileExists( ModelRenderer.MaterialOverride.Shader.ResourcePath )) return;
		AttributesParser<float,Color> parser = new AttributesParser<float,Color>(new NativeAttributeTypeSet());
		parser.Floats = Floats;
		parser.Floats2 = Floats2;
		parser.Floats4 = Floats4;
		parser.Textures = Textures;
		parser.DynamicCombos = DynamicCombos;
		parser.Colors = Colors;
		parser.ParseAttributes( ModelRenderer.MaterialOverride.Shader.ResourcePath );
		
	}
	protected override void OnUpdate()
	{
		SetColorAttributes();
		SetFloatAttributes();
		SetFloat2Attributes();
		SetTexturesAttributes();
		SetFloat4Attributes();
		SetDynamicCombos();
	}

	private void SetDynamicCombos()
	{
		foreach ( var dynamicCombo in DynamicCombos )
		{
			ModelRenderer.SceneObject.Attributes.SetCombo( dynamicCombo.Key, dynamicCombo.Value );
		}
	}

	private void SetFloat4Attributes()
	{
		foreach ( var float4Attribute in Floats4 )
		{
			ModelRenderer.SceneObject.Attributes.Set( float4Attribute.Key, float4Attribute.Value );
		}
	}

	private void SetTexturesAttributes()
	{
		foreach ( var textureAttribute in Textures )
		{
			ModelRenderer.SceneObject.Attributes.Set( textureAttribute.Key, textureAttribute.Value );
		}
	}


	private void SetColorAttributes()
	{
		foreach ( var colorAttribute in Colors )
		{
			ModelRenderer.SceneObject.Attributes.Set( colorAttribute.Key, colorAttribute.Value );
		}
	}

	private void SetFloatAttributes()
	{
		foreach ( var float1 in Floats )
		{
			ModelRenderer.SceneObject.Attributes.Set( float1.Key, float1.Value );
		}
	}

	private void SetFloat2Attributes()
	{
		foreach ( var float2 in Floats2 )
		{
			ModelRenderer.SceneObject.Attributes.Set( float2.Key, float2.Value );
		}
	}
}
