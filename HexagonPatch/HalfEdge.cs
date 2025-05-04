using System.Diagnostics;

namespace HexagonPatch;

public class HalfEdge
{
    private static int _nextId = 0;
    
    public HalfEdge(Edge edge, Face face, Vertex vertex)
    {
        Id = _nextId++;
        Edge = edge;
        Face = face;
        Vertex = vertex;
    }

    public readonly int Id;
    public Edge Edge { get; set; }
    public Face Face { get; set; }
    public Vertex Vertex { get; set; }
    
    public HalfEdge Next { get; set; }  
    public HalfEdge? Twin { get; set; }

    public HalfEdge FindPrev()
    {
        HalfEdge curr = this;
        do
        {
            curr = curr.Next;
        } while (curr.Next != this);

        return curr;
    }

    /*
     * Twins the current half edge with another given half edge.
     *
     * For debug builds, we make sure that the twinned edge actually refers to the same set of coordinates in reverse
     * order.
     */
    public void TwinWith(HalfEdge other)
    {
        // Check twin position invariant
        Debug.Assert(
            Vertex.Position.Equals(other.Next.Vertex.Position) &&
            Next.Vertex.Position.Equals(other.Vertex.Position)
        );
        
        Twin = other;
        other.Twin = this;

        // We make sure to make the twinned positions the identical Position instances so that we have an easier time
        // when we need to relax the mesh at the end
        Vertex.Position = other.Next.Vertex.Position;
        Next.Vertex.Position = other.Vertex.Position;
    }
}