using Sandbox;

public sealed class Billboard : Component
{
	protected override void OnUpdate()
	{
		this.WorldRotation = Rotation.LookAt(Scene.Camera.WorldPosition - this.WorldPosition);
	}
}
