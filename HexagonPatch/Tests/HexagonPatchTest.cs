using NUnit.Framework;

namespace HexagonPatch.Tests;

[TestFixture]
[TestOf(typeof(HexagonPatch))]
public class HexagonPatchTest
{
    
    [Test]
    public void TestCreateFaces()
    {
        HexagonPatch patch = new HexagonPatch(0, 0);


        List<Face> faces = patch.CreateTriangleGrid();

        int last = patch.Opts.PatchSize - 1;
        int i = 0;
        for (int r = 0; r < patch.Opts.PatchSize; r++)
        {
            for (int q = 0; q < patch.Opts.PatchSize; q++)
            {
                // all inner twins are set


                Face f0 = faces[i  ];
                Face f1 = faces[i+1];
                Face f2 = faces[i+2];
                Face f3 = faces[i+3];
                Face f4 = faces[i+4];
                Face f5 = faces[i+5];
                
                // all inner twins are set
                Assert.That(f0.HalfEdge.Twin, Is.Not.Null);
                Assert.That(f0.HalfEdge.Next.Next.Twin, Is.Not.Null);
                Assert.That(f1.HalfEdge.Twin, Is.Not.Null);
                Assert.That(f1.HalfEdge.Next.Next.Twin, Is.Not.Null);
                Assert.That(f2.HalfEdge.Twin, Is.Not.Null);
                Assert.That(f2.HalfEdge.Next.Next.Twin, Is.Not.Null);
                Assert.That(f3.HalfEdge.Twin, Is.Not.Null);
                Assert.That(f3.HalfEdge.Next.Next.Twin, Is.Not.Null);
                Assert.That(f4.HalfEdge.Twin, Is.Not.Null);
                Assert.That(f4.HalfEdge.Next.Next.Twin, Is.Not.Null);
                Assert.That(f5.HalfEdge.Twin, Is.Not.Null);
                Assert.That(f5.HalfEdge.Next.Next.Twin, Is.Not.Null);
                
                // check outer faces according to location
                HalfEdge twin0 = f0.HalfEdge.Next.Twin;
                HalfEdge twin1 = f1.HalfEdge.Next.Twin;
                HalfEdge twin2 = f2.HalfEdge.Next.Twin;
                HalfEdge twin3 = f3.HalfEdge.Next.Twin;
                HalfEdge twin4 = f4.HalfEdge.Next.Twin;
                HalfEdge twin5 = f5.HalfEdge.Next.Twin;

                bool firstCol = q == 0;
                bool lastCol = q == last;
                bool firstRow = r == 0;
                bool lastRow = r == last;
                bool evenRow = (r&1) == 0;
                
                Assert.That(twin0, firstRow || (lastCol && !evenRow) ? Is.Null : Is.Not.Null);
                Assert.That(twin1, lastCol ? Is.Null : Is.Not.Null);
                Assert.That(twin2, lastRow || (lastCol && !evenRow) ? Is.Null : Is.Not.Null);
                Assert.That(twin3, lastRow || (firstCol && evenRow) ? Is.Null : Is.Not.Null);
                Assert.That(twin4, firstCol ? Is.Null : Is.Not.Null);
                Assert.That(twin5, firstRow || (firstCol && evenRow) ? Is.Null : Is.Not.Null);

                for (int j = 0; j < Constants.NumFaces; j++)
                {
                    Face f = faces[j];
                    Assert.That(f.VertexCount, Is.EqualTo(3));

                    HalfEdge first = f.HalfEdge;
                    HalfEdge current = first;
                    do
                    {
                        Assert.That(current.Vertex, Is.Not.Null);
                        Assert.That(current.Vertex.Position, Is.Not.Null);
                        Assert.That(current.Vertex.HalfEdge, Is.EqualTo(current));
                        Assert.That(current.Edge, Is.Not.Null);
                        Assert.That(current.Edge.HalfEdge, Is.Not.Null);
                        
                        current = current.Next;
            
                    } while (current != first);
                }
                
                i += Constants.NumFaces;
            }
        }
    }

    [Test]
    public void TestBuild()
    {
        var p = new HexagonPatch(0,0);
        
        
        foreach (Face face in p.Faces)
        {
            Console.WriteLine($"FACE: {face}");
            var first = face.HalfEdge;
            var current = first;
            int count = 0;
            do
            {
                HalfEdge next = current.Next;
                Console.WriteLine("    " + current.Vertex.Position.X + ", " + current.Vertex.Position.Y);
                //DebugDraw3D.DrawLine(current.Vertex.Position, next.Vertex.Position, Colors.LightGray);
                current = next;
                count++;
            } while (current != first && count < 5);
        }

    }
}