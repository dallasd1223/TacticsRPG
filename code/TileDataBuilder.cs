using Sandbox;

namespace TacticsRPG;

public class TileDataBuilder : Component
{
	[Property] public int TileIndex {get; set;}
	[Property] public int XIndex {get; set;}
	[Property] public int YIndex {get; set;}
	[Property] public Vector3 TilePosition {get; set;}
	private float RaiseAmount {get; set;} = 24f;
	[Property] public int HeightIndex {get; set;} = 1;
	[Property] public TileSurfaceType SurfaceType = TileSurfaceType.NA;

	[Button("Raise Tile Height")]
	public void RaiseHeight()
	{
		HeightIndex++;
		this.GameObject.LocalPosition += new Vector3(0,0,RaiseAmount);
		Log.Info($"{this.GameObject.Name} Height Raised to {HeightIndex}");
	}

	[Button("Lower Tile Height")]
	public void LowerHeight()
	{
		if(HeightIndex == 0 )
		{
			Log.Info("Already Lowest Height");
			return;
		}

		HeightIndex--;
		this.GameObject.LocalPosition -= new Vector3(0,0,RaiseAmount);
		Log.Info($"{this.GameObject.Name} Height Lowerd to {HeightIndex}");
	}

	protected override void DrawGizmos()
	{
		Gizmo.Draw.Text($"{TileIndex}", global::Transform.Zero);
	}
}
