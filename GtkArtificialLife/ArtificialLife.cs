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

    protected Color DefaultColor = new Color(0x00ff, 0x00ff, 0x00ff);
    protected Color EmptyColor = new Color(0x0000, 0x0000, 0x0000);

    // Rendering
    public abstract List<Pixel> GetPixelWriteBuffer();

    public abstract void ClearPixelWriteBuffer();

    public abstract void Update();

    public abstract List<Parameter> Parameters();

    public abstract void Refresh();

    public abstract Color Color();

    public abstract List<Cell> GetNeighborhood();

    public abstract void SetNeighborhood(List<Cell> neighborhood);
}
