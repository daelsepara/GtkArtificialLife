using Gdk;
using System;
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
    // Grid / Colony
    protected List<Pixel> PixelWriteBuffer = new List<Pixel>();
    protected readonly List<Cell> Neighborhood = new List<Cell>();
    protected List<Change> ChangeList = new List<Change>();
    protected int[,] Grid;
    public int Width;
    public int Height;
    public bool Cyclic;

    // Colors
    protected List<Color> ColorPalette = new List<Color>();
    protected Color DefaultColor = new Color(255, 255, 255);
    protected Color EmptyColor = new Color(0, 0, 0);
    public Color ColonyColor = new Color(255, 0, 255);

    // A-Life-Type specific updates
    public abstract void Update();
    public abstract List<Parameter> Parameters();
    public abstract void Refresh();

    protected Random random = new Random(Guid.NewGuid().GetHashCode());

    // Rendering
    public List<Pixel> GetPixelWriteBuffer()
    {
        return new List<Pixel>(PixelWriteBuffer);
    }

    public void ClearPixelWriteBuffer()
    {
        PixelWriteBuffer.Clear();
    }

    public void PushPixel(Pixel pixel)
    {
        if (pixel != null)
        {
            PixelWriteBuffer.Add(pixel);
        }
    }

    public Color Color()
    {
        return ColonyColor;
    }

    protected void GenerateRandomColorPalette()
    {
        ColorPalette.Clear();

        ColorPalette.AddRange(Utility.GenerateRandomColorPalette(ColonyColor));
    }

    public void ApplyChanges()
    {
        foreach (var change in ChangeList)
        {
            Grid[change.X, change.Y] = change.Value;
        }

        ChangeList.Clear();
    }

    // Neighborhood
    public List<Cell> GetNeighborhood()
    {
        return new List<Cell>(Neighborhood);
    }

    public void SetNeighborhood(List<Cell> neighborhood)
    {
        Neighborhood.Clear();

        Neighborhood.AddRange(neighborhood);
    }

    public void SetCyclic(bool cyclic)
    {
        Cyclic = cyclic;
    }

    protected void InitGrid(int width, int height)
    {
        Width = width;
        Height = height;

        Grid = new int[width, height];
    }

    protected void AddMooreNeighborhood()
    {
        Neighborhood.Clear();

        Neighborhood.AddRange(World.MooreNeighborhood());
    }

    protected void AddVonNeumannNeighborhood()
    {
        Neighborhood.Clear();

        Neighborhood.AddRange(World.VonNeumannNeighborhood());
    }

    protected void AddHexNeighborhood()
    {
        Neighborhood.Clear();

        Neighborhood.AddRange(World.HexNeighborhood());
    }
}
