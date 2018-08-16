using Gdk;
using System;
using System.Collections.Generic;

public class Cyclic : ArtificialLife
{
    List<Pixel> PixelWriteBuffer = new List<Pixel>();
    List<Cell> Neighborhood = new List<Cell>();
    List<Change> ChangeList = new List<Change>();
    List<Color> ColorPalette = new List<Color>();
    int[,] Grid;
    int MaxStates = 4;
    int Delta = 1;

    public void GenerateRandomColorPalette()
    {
        ColorPalette.Clear();

        ColorPalette.AddRange(Utility.GenerateRandomColorPalette(ColonyColor));
    }

    public void GradientPalette()
    {
        ColorPalette.Clear();

        ColorPalette.AddRange(Utility.Gradient(ColonyColor, MaxStates));
    }

    public void GreyPalette()
    {
        ColorPalette.Clear();

        ColorPalette.AddRange(Utility.GreyPalette());
    }

    public Cyclic()
    {
        InitGrid(256, 256);

        ColonyColor = DefaultColor;

        GenerateRandomColorPalette();

        AddMooreNeighborhood();
    }

    public Cyclic(int width, int height)
    {
        InitGrid(width, height);

        ColonyColor = DefaultColor;

        GenerateRandomColorPalette();

        AddMooreNeighborhood();
    }

    public Cyclic(int width, int height, Color color)
    {
        InitGrid(width, height);

        if (!color.Equal(EmptyColor))
        {
            ColonyColor.Red = color.Red;
            ColonyColor.Green = color.Green;
            ColonyColor.Blue = color.Blue;
        }
        else
        {
            ColonyColor = DefaultColor;
        }

        GenerateRandomColorPalette();
        AddMooreNeighborhood();
    }

    public Cyclic(int width, int height, int maxStates, Color color)
    {
        InitGrid(width, height);

        if (!color.Equal(EmptyColor))
        {
            ColonyColor = color;
        }
        else
        {
            ColonyColor = DefaultColor;
        }

        MaxStates = maxStates;

        Delta = maxStates > 0 ? (256 / maxStates) : 0;

        GenerateRandomColorPalette();

        AddMooreNeighborhood();
    }

    protected void InitGrid(int width, int height)
    {
        Width = width;
        Height = height;

        Grid = new int[width, height];
    }

    public override void ClearPixelWriteBuffer()
    {
        PixelWriteBuffer.Clear();
    }

    public override List<Pixel> GetPixelWriteBuffer()
    {
        return new List<Pixel>(PixelWriteBuffer);
    }

    public void AddMooreNeighborhood()
    {
        Neighborhood.Clear();
        Neighborhood.AddRange(ParameterSets.MooreNeighborhood());
    }

    public void WriteCell(int x, int y, int val)
    {
        if (x >= 0 && x < Width && y >= 0 && y < Height)
        {
            PushPixel(new Pixel(x, y, val >= 0 && val < MaxStates ? ColorPalette[val * Delta] : EmptyColor));

            ChangeList.Add(new Change(x, y, val));
        }
    }

    protected void RemovePixel(int index)
    {
        if (PixelWriteBuffer.Count > 0 && index < PixelWriteBuffer.Count)
        {
            PixelWriteBuffer.RemoveAt(index);
        }
    }

    public void PushPixel(Pixel pixel)
    {
        if (pixel != null)
        {
            PixelWriteBuffer.Add(pixel);
        }
    }

    public Pixel PopPixel()
    {
        if (PixelWriteBuffer.Count < 1)
        {
            return null;
        }

        var pixel = PixelWriteBuffer[PixelWriteBuffer.Count - 1];

        RemovePixel(PixelWriteBuffer.Count - 1);

        return pixel;
    }

    protected CountSum CountCellNeighbors(int x, int y, int minVal, int maxVal)
    {
        int neighbors = 0;
        int sum = 0;

        foreach (var neighbor in GetNeighborhood())
        {
            var nx = x + neighbor.X;
            var ny = y + neighbor.Y;

            if (nx >= 0 && nx < Width && ny >= 0 && ny < Height && Grid[nx, ny] >= minVal && Grid[nx, ny] < maxVal)
            {
                neighbors++;
                sum += Grid[nx, ny];
            }
        }

        return new CountSum(neighbors, sum);
    }

    public override void Update()
    {
        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                int state = Grid[x, y];
                int successor = (state + 1) % MaxStates;
                var newstate = state;

                var CellCount = CountCellNeighbors(x, y, successor, successor + 1).Count;

                if (CellCount > 0)
                {
                    newstate = successor;
                }

                if (newstate != state || newstate > 0)
                    WriteCell(x, y, newstate);
            }
        }

        ApplyChanges();
    }

    public void ApplyChanges()
    {
        foreach (var change in ChangeList)
        {
            Grid[change.X, change.Y] = change.Value;
        }

        ChangeList.Clear();
    }

    public override void Refresh()
    {
        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                if (Grid[x, y] > 0)
                {
                    WriteCell(x, y, Grid[x, y]);
                }
            }
        }
    }

    public void Randomize(int maxStates = 256)
    {
        MaxStates = maxStates;

        Delta = maxStates > 0 ? (256 / maxStates) : 0;

        var random = new Random(Guid.NewGuid().GetHashCode());

        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                WriteCell(x, y, random.Next(0, MaxStates));
            }
        }

        ApplyChanges();
    }

    public override List<Parameter> Parameters()
    {
        var set = new List<Parameter>
        {
            new Parameter("MaxStates", MaxStates, 2, 256)
        };

        return set;
    }

    public void WriteGrid(int x, int y, int val)
    {
        if (x >= 0 && x < Width && y >= 0 && y < Height)
        {
            WriteCell(x, y, val);
        }
    }

    public override Color Color()
    {
        return ColonyColor;
    }

    public override List<Cell> GetNeighborhood()
    {
        return new List<Cell>(Neighborhood);
    }

    public override void SetNeighborhood(List<Cell> neighborhood)
    {
        Neighborhood.Clear();
        Neighborhood.AddRange(neighborhood);
    }
}
