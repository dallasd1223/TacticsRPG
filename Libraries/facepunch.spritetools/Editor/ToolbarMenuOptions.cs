using Editor;
using Sandbox;
using System;
using System.Collections.Generic;

namespace SpriteTools;

public static class ToolbarMenuOptions
{

	[Menu( "Editor", "Sprite Tools/Flush Texture Cache" )]
	public static void FlushTextureCache ()
	{
		TextureAtlas.ClearCache();
		TileAtlas.ClearCache();
		PixmapCache.ClearCache();
	}

	[Menu( "Editor", "Sprite Tools/Curve Test" )]
	public static void UltimateCurveTest ()
	{
		List<int> keyframes = [1, 2, 5, 25, 50, 100, 1000];
		List<float> evaluateVals = [];
		int valsToEvaluate = 2000000;
		for ( int i = 0; i < valsToEvaluate; i++ )
		{
			evaluateVals.Add( Random.Shared.Float() );
		}

		foreach ( var keyframeCount in keyframes )
		{
			var frames = new List<Curve.Frame>();
			for ( int i = 0; i < keyframeCount; i++ )
			{
				// Create a frame with a random time and value
				var frame = new Curve.Frame
				{
					Time = i / keyframeCount,
					Value = Random.Shared.Float()
				};
				frames.Add( frame );
			}
			var curve = new Curve( frames );

			// Create a timer and benchmark the curve evaluation
			var timer = new System.Diagnostics.Stopwatch();
			timer.Start();
			for ( int i = 0; i < valsToEvaluate; i++ )
			{
				var evalTime = evaluateVals[i];
				curve.Evaluate( evalTime );
			}
			timer.Stop();
			Log.Info( $"Curve with {keyframeCount} keyframes took {timer.ElapsedMilliseconds} ms to evaluate {valsToEvaluate} times." );
		}

	}

}