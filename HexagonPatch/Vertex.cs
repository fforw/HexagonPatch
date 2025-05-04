using Godot;

namespace HexagonPatch;

public class Vertex 
{
    public Vertex(Vector3 position)
    {
        Position = position;
    }

    public Vector3 Position { get; set; }
    public HalfEdge HalfEdge { get; set; }
}