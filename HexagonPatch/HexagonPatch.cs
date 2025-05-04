using System.Diagnostics;
using Godot;

namespace HexagonPatch;

public class HexagonPatch
{
    //private static Logger log = LogManager.GetCurrentClassLogger();
    
    public readonly HexagonPatchOptions Opts;

    public readonly int Q;
    public readonly int R; 

    public readonly List<Face> Faces;

    public HexagonPatch(int q, int r, HexagonPatchOptions opts = null)
    {
        Q = q;
        R = r;
        
        Opts = opts ?? new HexagonPatchOptions();

        Faces = Build();
    }

    public List<Face> Build()
    {
        List<Face> tris = CreateTriangleGrid();
        // List<Face> faces = Subdivide(RemoveEdges(tris));
        // relax(faces);
        // return faces;
        return RemoveEdges(tris);
    }

    private void relax(List<Face> faces)
    {
        throw new NotImplementedException();
    }

    /**
     * Subdivides the given list of triangle and quad faces one time so that the new list contains only
     * quads.
     */
    private List<Face> Subdivide(List<Face> faces)
    {
        List<Face> subdivided = new List<Face>();
        foreach (Face face in faces)
        {
            if (face.VertexCount == 3)
            {
                subdivideTri(subdivided, face);
            }
            else
            {
                subdivideQuad(subdivided, face);
            }
        }

        return subdivided;
    }

    /**
     * Subdivides a Quad into four new quads
     */
    private void subdivideQuad(List<Face> subdivided, Face face)
    {
        throw new NotImplementedException();
    }

    /**
     * Subdivides a triangle into three new quads
     */
    private void subdivideTri(List<Face> subdivided, Face face)
    {
        throw new NotImplementedException();
    }

    /**
     * Removes random edges from the triangle grid. Each removal turns 2 triangles into a diamond shaped quad.
     *
     * To qualify for random removal, an edge has to be twinned (i.e. not touch the patch border) and both twins must
     * belong to faces that are triangles.
     *
     * We have a target multiplier limit for how many edges we want to remove.
     * However we give up on 10 failures in a row to spare us overly long searches.
     */
    public List<Face> RemoveEdges(List<Face> faces, float target = 0.5f)
    {
        int targetCount = (int)(target * faces.Count);

        List<Face> merged = new List<Face>();
        
        Random rng = Opts.Random;
        
        int failures = 0;
        while (faces.Count > targetCount && failures < RemovalFailureLimit)
        {
            Face f = faces[rng.Next(faces.Count-1)];
            HalfEdge he = f.GetNthHalfEdge(rng.Next(2));

            //Console.WriteLine($"RANDOM EDGE: Face #{f.Id} / HalfEdge #{he.Id}");
            
            HalfEdge? twin = he.Twin;
            if (twin != null)
            {
                //Console.WriteLine($"TWIN: Face #{twin.Face.Id} / HalfEdge #{twin.Id}");
                //log.Debug($"Found candidate after {failures} failures");
                failures = 0;

                // we remove both faces from our initial list to keep the list quad-free, which makes the only
                // disqualifying issue that of the twin existence

                if (!faces.Remove(f))
                {
                    throw new InvalidOperationException("Current face not in list");
                }

                if (!faces.Remove(twin.Face))
                {
                    // we followed a twin pointer into an already removed face
                    failures++;
                    continue;
                }
                
                HalfEdge prev = he.FindPrev();
                HalfEdge twinPrev = twin.FindPrev();

                prev.Next = twin.Next;
                twinPrev.Next = he.Next;
                
                if (f.HalfEdge == he)
                {
                    f.HalfEdge = prev;
                }

                ResetFacesInLoop(prev, f);

                //Debug.Assert(f.VertexCount == 4);

                // we add the face which is now a quad to the list of merged faces
                merged.Add(f);
            }
            else
            {
                failures++;
            }
        }
        

        // if (failures == RemovalFailureLimit)
        // {
        //     Console.Write($"Gave up after {RemovalFailureLimit} failures");
        // }
        
        // We add the remaining triangles back to the list of merged faces
        merged.AddRange(faces);
        return merged;
    }

    private static void ResetFacesInLoop(HalfEdge first, Face face)
    {
        HalfEdge current = first;
        do
        {
            current.Face = face;
            current = current.Next;
        } while (current != first);
    }

    private const int RemovalFailureLimit = 10;

    public List<Face> CreateTriangleGrid()
    {
        List<Face> faces = [];
        List<Vector3> cell = Opts.Cell;

        int last = Opts.PatchSize - 1;
        
        for (int r = 0; r < Opts.PatchSize; r++)
        {
            for (int q = 0; q < Opts.PatchSize; q++)
            {
                var offset = Opts.CalculateOffset(Q + q, R + r);

                Face f0 = CreateTri(cell[0], cell[1], cell[2], offset);
                Face f1 = CreateTri(cell[0], cell[2], cell[3], offset);
                Face f2 = CreateTri(cell[0], cell[3], cell[4], offset);
                Face f3 = CreateTri(cell[0], cell[4], cell[5], offset);
                Face f4 = CreateTri(cell[0], cell[5], cell[6], offset);
                Face f5 = CreateTri(cell[0], cell[6], cell[1], offset);
                
                f0.HalfEdge.TwinWith(f5.HalfEdge.Next.Next);
                f1.HalfEdge.TwinWith(f0.HalfEdge.Next.Next);
                f2.HalfEdge.TwinWith(f1.HalfEdge.Next.Next);
                f3.HalfEdge.TwinWith(f2.HalfEdge.Next.Next);
                f4.HalfEdge.TwinWith(f3.HalfEdge.Next.Next);
                f5.HalfEdge.TwinWith(f4.HalfEdge.Next.Next);

                if (q > 0)
                {
                    // twin with the second face of the previous group
                    faces[^5].HalfEdge.Next.TwinWith(f4.HalfEdge.Next);                    
                }

                if (r > 0)
                {
                    bool isEvenRow = (r & 1) == 0;

                    if (isEvenRow)
                    {
                        if (q > 0)
                        {
                            faces[^((Opts.PatchSize + 1) * Constants.NumFaces - 2)].HalfEdge.Next.TwinWith(f5.HalfEdge.Next);
                        }
                        faces[^(Opts.PatchSize * Constants.NumFaces - 3)].HalfEdge.Next.TwinWith(f0.HalfEdge.Next);
                        
                    }
                    else
                    {
                        faces[^(Opts.PatchSize * Constants.NumFaces - 2)].HalfEdge.Next.TwinWith(f5.HalfEdge.Next);
                        if (q < last)
                        {
                            faces[^((Opts.PatchSize - 1) * Constants.NumFaces - 3)].HalfEdge.Next.TwinWith(f0.HalfEdge.Next);
                        }
                    }
                }

                faces.Add(f0);
                faces.Add(f1);
                faces.Add(f2);
                faces.Add(f3);
                faces.Add(f4);
                faces.Add(f5);
                
            }
        }

        return faces;
    }

    /**
     * 
     */
    private Face CreateTri(Vector3 p0, Vector3 p1, Vector3 p2, Vector2I offset)
    {
        Face face = new Face();
        HalfEdge h0 = new HalfEdge(new Edge(), face, new Vertex(new Vector3(p0.X + offset.X, p0.Y + offset.Y, p0.Z)));
        h0.Edge.HalfEdge = h0;
        h0.Vertex.HalfEdge = h0;
        HalfEdge h1 = new HalfEdge(new Edge(), face, new Vertex(new Vector3(p1.X + offset.X, p1.Y + offset.Y, p1.Z)));
        h1.Edge.HalfEdge = h1;
        h1.Vertex.HalfEdge = h1;
        HalfEdge h2 = new HalfEdge(new Edge(), face, new Vertex(new Vector3(p2.X + offset.X, p2.Y + offset.Y, p2.Z)));
        h2.Edge.HalfEdge = h2;
        h2.Vertex.HalfEdge = h2;

        h0.Next = h1;
        h1.Next = h2;
        h2.Next = h0;
        
        face.HalfEdge = h0;
        
        return face;
    }
}