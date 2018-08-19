using Gdk;
using System.Collections.Generic;

public class Cell
{
    public int X;
    public int Y;

    public Cell()
    {

    }

    public Cell(int x, int y)
    {
        X = x;
        Y = y;
    }
}

public abstract class ArtificialLife
{
    public int Width;
    public int Height;
    public Color ColonyColor;
    public bool Cyclic;

    protected Color DefaultColor = new Color(255, 255, 255);
    protected Color EmptyColor = new Color(0, 0, 0);

    // Rendering
    public abstract List<Pixel> GetPixelWriteBuffer();

    public abstract void ClearPixelWriteBuffer();

    public abstract void Update();

    public abstract List<Parameter> Parameters();

    public abstract void Refresh();

    public abstract Color Color();

    public abstract List<Cell> GetNeighborhood();

    public abstract void SetNeighborhood(List<Cell> neighborhood);

    public void SetCyclic(bool cyclic)
    {
        Cyclic = cyclic;
    }
}
