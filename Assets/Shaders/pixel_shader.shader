FEATURES
{
    #include "common/features.hlsl"
}

MODES
{
    Forward();
    Depth();
}

COMMON
{
	#include "common/shared.hlsl"
}

struct VertexInput
{
	#include "common/vertexinput.hlsl"
};

struct PixelInput
{
	#include "common/pixelinput.hlsl"
};

VS
{
	#include "common/vertex.hlsl"

	PixelInput MainVs( VertexInput i )
	{
		PixelInput o = ProcessVertex( i );
		// Add your vertex manipulation functions here
		return FinalizeVertex( o );
	}
}

PS
{
    #include "common/pixel.hlsl"

    CreateInputTexture2D( AlbedoMap, Srgb, 8, "", "_color", "My Material,10/10", Default3( 1.0, 1.0, 1.0 ) );

    Texture2D g_tColor < Channel( RGB, Box( AlbedoMap ), Srgb ); OutputFormat( BC7 ); SrgbRead( true ); >;

	SamplerState pixels < Filter( Point ); >;

	float3 ColorTint < UiType(Color); Default3(1.0, 1.0, 1.0); >;

	float4 MainPs( PixelInput i ) : SV_Target0
	{
		float3 MyTexture = g_tColor.Sample(pixels, i.vTextureCoords.xy);
		Material m = Material::Init( i );
		m.Albedo = MyTexture * ColorTint;
		/* m.Metalness = 1.0f; // Forces the object to be metalic */
		return ShadingModelStandard::Shade( m );
	}
}
