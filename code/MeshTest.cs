using Sandbox;
using System; 
public sealed class MeshTest : Component
{

	[Property] public Model myModel {get; set;}
	public Vertex[] verts {get; set;}
	public System.UInt32[] indices {get; set;}

	protected override void OnUpdate()
	{

	}
	
	protected override void OnStart()
	{
		/*
		verts = myModel.GetVertices();
		indices = myModel.GetIndices();
		foreach(System.UInt32 i in indices) {
			Log.Info(i);
		}
		*/
	}
/*
	protected override void DrawGizmos()
	{
		base.DrawGizmos();
		verts = myModel.GetVertices();

		foreach(Vertex v in verts) {
			Vector3 vec = GameObject.WorldTransform.PointToWorld(v.Position);
			Gizmo.Draw.Color = Color.Blue;
			Gizmo.Draw.LineThickness = 3;
			Gizmo.Draw.SolidSphere(vec, 4f);
			
		}

	}
*/
}
