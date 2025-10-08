// Ideally you wouldn't need half these includes for an unlit shader
// But it's stupiod

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
		// Adjust the vertices so they appear correctly as a skybox material
		float3 vPositionWs = g_vCameraPositionWs.xyz + i.vPositionOs.xyz * 100;
		o.vPositionPs = Position3WsToPs( vPositionWs.xyz );
		o.vPositionWs = vPositionWs;

		return FinalizeVertex( o );
	}
}

PS
{
    #include "common/pixel.hlsl"

	// Backfaces rendering must be enabled to make shader render properly as a skybox material
	RenderState( CullMode, NONE );
    CreateInputTexture2D( SkyMask, Srgb, 8, "", "_color", "My Material,10/10", Default3( 1.0, 1.0, 1.0 ) );

    Texture2D g_Mask < Channel( RGB, Box( SkyMask ), Srgb ); OutputFormat( BC7 ); SrgbRead( true ); >;	
		// Material properties
	float3 SkyColor < UiType( Color ); Default3( 1, 1, 1 ); UiGroup( "Sky, 10/10" ); >;
	float3 SkyHorizon < UiType( Color ); Default3( 1, 0, 1 ); UiGroup( "Sky, 10/20" ); >;
	SamplerState pixels < Filter( Point ); >;
	float4 MainPs( PixelInput i ) : SV_Target0
	{
		float3 vPositionWs = i.vPositionWithOffsetWs + g_vCameraPositionWs;
		float3 vRay = normalize( vPositionWs - (g_vCameraPositionWs - float3(0,0,30)) );
		float time = g_flTime;
		float3 MyTexture = g_Mask.Sample(g_sAniso, (vRay.xz+float2(time * 0.05, -time * 0.05)));


		float r = sin(time + 0.0) * 0.5 + 0.5;
    	float g = sin(time + 2.0) * 0.5 + 0.5; // Offset by 2.0 for green
		float b = sin(time + 4.0) * 0.5 + 0.5; // Offset by 4.0 for blue
		float3 rainbow = float3(r, g, b);
		float3 sky = lerp( SkyHorizon*rainbow, SkyColor, saturate( vRay.z * 0.8) );
		return float4( sky * MyTexture, 1 );
	}
}
