using Sandbox;

namespace StairsTool;

public struct Vertex
{
    public Vector3 Position;
    public Vector3 Normal; 
    public Vector2 TexCoord;

    public static readonly VertexAttribute[] Layout = new[]
    {
        new VertexAttribute(VertexAttributeType.Position, VertexAttributeFormat.Float32, 3),
        new VertexAttribute(VertexAttributeType.Normal, VertexAttributeFormat.Float32, 3),
        new VertexAttribute(VertexAttributeType.TexCoord, VertexAttributeFormat.Float32, 2)
    };

    public Vertex(Vector3 position, Vector3 normal, Vector2 texCoord)
    {
        Position = position;
        Normal = normal;
        TexCoord = texCoord;  
    }
}
