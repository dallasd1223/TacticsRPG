using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Sandbox;

public class AttributesParser<TFloat, TGradient>
{
	private readonly IAttributeTypeSet<TFloat, TGradient> _typeSet;
	public AttributesParser(IAttributeTypeSet<TFloat, TGradient> typeSet)
	{
		if (!((typeof(TFloat) == typeof(ParticleFloat) && typeof(TGradient) == typeof(ParticleGradient)) ||
		      (typeof(TFloat) == typeof(float) && typeof(TGradient) == typeof(Color))))
		{
			throw new InvalidOperationException("Only Particle or Native types allowed.");
		}
		_typeSet = typeSet;
	}
	public Dictionary<String, TGradient> Colors { get; set; }
	
	public Dictionary<String, TFloat> Floats { get; set; }
	
	public Dictionary<String, Vector2> Floats2 { get; set; }
	
	public Dictionary<String, Vector4> Floats4 { get; set; }
	
	public Dictionary<String, Texture> Textures { get; set; }
	
	public Dictionary<String, int> DynamicCombos { get; set; }


	private const string AttributesPattern = @"(float(?:[2,3,4])?|Texture2D) +.+ +< *Attribute\(.?""(.+)"".?\)\; *(?:>|Default(?:[1,2,3,4])?\(.*\); >)\;";
	private const string DynamicComboPattern = "DynamicCombo\\(([^,]+),.+\\)\\;";

	public void ParseAttributes( string filePath )
	{
		string fileContent = FileSystem.Mounted.ReadAllText( filePath );
		var attributeMatches = Regex.Matches(fileContent, AttributesPattern);
		var dynamicComboMatches = Regex.Matches(fileContent, DynamicComboPattern);

		foreach ( Match match in dynamicComboMatches )
		{
			var value = match.Groups[1].Value.Trim();
			if(!DynamicCombos.ContainsKey(value)) DynamicCombos.Add(value, 0);
		}

		foreach (Match match in attributeMatches)
		{
			var type = match.Groups[1].Value;
			var value = match.Groups[2].Value;
			SetAttribute(type, value);
		}
	}

	private void SetAttribute( string attributeType, string attributeName )
	{
		
		switch ( attributeType )
		{
			case "float":
				if ( !Floats.ContainsKey( attributeName ) ) Floats.Add( attributeName, _typeSet.GetDefaultFloat() );
				break;
			case "float2":
				if(!Floats2.ContainsKey( attributeName )) Floats2.Add( attributeName, new Vector2(  ) );
				break;
			case "float4":
				if(!Floats4.ContainsKey( attributeName ) && !Colors.ContainsKey( attributeName )) SetFloat4( attributeName);
				break;
			case "Texture2D":
				if(!Textures.ContainsKey( attributeName )) Textures.Add( attributeName, Texture.White );
				break;
			default: break;
		}
	}

	private void SetFloat4( string attributeName)
	{
		if ( attributeName.ToLower().Contains( "color" ) )
		{
			Colors.Add( attributeName, _typeSet.GetDefaultColor() );
		}
		else
		{
			Floats4.Add( attributeName, new Vector4(  ) );	
		}
		
	}
}
