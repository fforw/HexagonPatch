using Godot;

namespace HexagonPatch;

public class Constants
{
    public const float Tau = Mathf.Pi * 2;
    public static readonly float Sin60 = Mathf.Sin( Tau / 6);

    /**
     * The relation between Hexagon width in point-up position to the radius of the hexagon (spoke length)
     */
    public static readonly float HFactor = Mathf.Sqrt(3);

    /**
     * Unscaled points for our cell. First point is center and then in clockwise order from the top 6 hexagon points
     * with a distance of 1.
     */
    public static readonly List<Vector3> Cell =
    [
        new(0, 0, 0),
        new(0, -1, 0),
        new(Sin60, -0.5f, 0),
        new(Sin60, 0.5f, 0),
        new(0, 1, 0),
        new(-Sin60, 0.5f, 0),
        new(-Sin60, -0.5f, 0)
    ];

    /**
     * Number of triangles in a hexagon 
     */
    public const int NumFaces = 6;
}