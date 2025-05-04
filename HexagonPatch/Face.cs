namespace HexagonPatch;

public class Face
{
    private static int nextId = 0;

    public readonly int Id;
    
    public HalfEdge? HalfEdge { get; set; }

    public Face()
    {
        Id = nextId++;
    }

    public int VertexCount
    {
        get
        {

            int count = 0;
            
            ArgumentNullException.ThrowIfNull(HalfEdge, "HalfEdge is null.");
            
            HalfEdge current = HalfEdge;
            do
            {
                count++;
                current = current.Next;

            } while (current != HalfEdge);
            return count;
        }
    }
    
    
    // public override string ToString()
    // {
    //     string s = base.ToString() + ": ";
    //
    //     HalfEdge current = HalfEdge;
    //     do
    //     {
    //         s += current.Vertex.Position.ToString() + ", ";
    //         current = current.Next;
    //         
    //     } while (current != HalfEdge);
    //     
    //     return s;
    // }

    public HalfEdge GetNthHalfEdge(int nth)
    {
        ArgumentNullException.ThrowIfNull(HalfEdge, "HalfEdge is null.");

        int count = nth;
        HalfEdge current = HalfEdge;
        do
        {
            if (count == 0)
            {
                return current;
            }
            current = current.Next;
            count--;
            
        } while (current != HalfEdge);

        throw new ArgumentOutOfRangeException("Face has less than " + nth + " edges.");
    }
}