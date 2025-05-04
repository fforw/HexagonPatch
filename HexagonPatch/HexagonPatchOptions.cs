using Godot;

namespace HexagonPatch;

public class HexagonPatchOptions
{
    
    public readonly int Size;
    public readonly int PatchSize;

    public readonly int Width;
    public readonly int HalfWidth;
    public readonly int Height;
    
    public readonly Random Random;

    public List<Vector3> Cell { get; }
    
    public HexagonPatchOptions(int size = 44, int patchSize = 10)
    {
        Size = size;
        PatchSize = patchSize;
        
        Width = (int)Mathf.Round(Constants.HFactor * size);

        if ((Width & 1) != 0)
        {
            throw new ArgumentException(
                "Regrettably, the size must be chosen so that Width = Round(Sqrt(3) * size) " +
                $"is an even number, but Size = {size} leads to Width = {Width}, and therefore it is invalid."
            );
        }
        
        HalfWidth = Width / 2;
        Height = Size * 2;
        
        //Random = random ?? new RandomNumberGenerator();
        
        Cell = Constants.Cell.ConvertAll(
            v => new Vector3(
                Mathf.Round(v.X * size),
                Mathf.Round(v.Y * size),
                0
            )
        );
        
        Random = new Random(14121970);
    }


    public Vector2I CalculateOffset(int q, int r)
    {
        return new Vector2I(
            Width * q + ((r & 1) != 0 ? HalfWidth : 0), 
            (int)(Mathf.Floor(Height * 0.75) * r)
        );
    }

}