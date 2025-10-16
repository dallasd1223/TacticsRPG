using Sandbox;

namespace TacticsRPG;

public sealed class ActiveTurnData : Component
{
	[Property] public float CurrentTileHeight {get; set;} = 0f;
}
