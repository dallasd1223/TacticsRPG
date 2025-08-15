using Sandbox;

public sealed class CameraFocusNode : Component
{
	[Property] public NodeType Type {get; set;} = NodeType.Unit;


}

public enum NodeType 
{
	Unit,
	Tile,
	Map,
	Event,
}

