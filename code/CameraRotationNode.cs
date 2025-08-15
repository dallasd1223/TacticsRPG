using Sandbox;

namespace TacticsRPG;
public sealed class CameraRotationNode : Component
{
		[Property] public NodeFacing Facing {get; set;} = NodeFacing.NA;
}

public enum NodeFacing
{
	NA,
	Front,
	Back,
}
