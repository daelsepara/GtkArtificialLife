using Gdk;
using System;
using System.Collections.Generic;

public class Life : ArtificialLife
{
    List<Pixel> PixelWriteBuffer = new List<Pixel>();
    List<int> BirthRules = new List<int>();
    List<int> SurvivalRules = new List<int>();
    List<Cell> Neighborhood = new List<Cell>();
    List<Change> ChangeList = new List<Change>();

    int Density;
    int[,] Grid;

    public Life()
    {
        InitGrid(256, 256);

        ColonyColor = DefaultColor;

        AddMooreNeighborhood();
        AddRules();
    }

    public Life(int width, int height)
    {
        InitGrid(width, height);

        ColonyColor = DefaultColor;

        AddMooreNeighborhood();
        AddRules();
    }

    public Life(int width, int height, Color color)
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

        AddMooreNeighborhood();
        AddRules();
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

    public List<Cell> GetNeighborhood()
    {
        return new List<Cell>(Neighborhood);
    }

    public void AddNeighbor(Cell neighbor)
    {
        if (!Neighborhood.Contains(neighbor))
        {
            Neighborhood.Add(neighbor);
        }
    }

    public void AddMooreNeighborhood()
    {
        Neighborhood.Clear();

        AddNeighbor(new Cell(-1, -1));
        AddNeighbor(new Cell(0, -1));
        AddNeighbor(new Cell(1, -1));
        AddNeighbor(new Cell(-1, 0));
        AddNeighbor(new Cell(1, 0));
        AddNeighbor(new Cell(-1, 1));
        AddNeighbor(new Cell(0, 1));
        AddNeighbor(new Cell(1, 1));
    }

    public void AddRules()
    {
        BirthRules.Clear();
        SurvivalRules.Clear();

        AddBirthRule(3);
        AddSurvivalRule(2);
        AddSurvivalRule(3);
    }

    public void AddBirthRule(int count)
    {
        if (count > 0 && !BirthRules.Contains(count))
        {
            BirthRules.Add(count);
        }
    }

    public void AddSurvivalRule(int count)
    {
        if (count > 0 && !SurvivalRules.Contains(count))
        {
            SurvivalRules.Add(count);
        }
    }

    public void WriteCell(int x, int y, int val)
    {
        if (x >= 0 && x < Width && y >= 0 && y < Height)
        {
            PushPixel(new Pixel(x, y, val > 0 ? ColonyColor : EmptyColor));
            ChangeList.Add(new Change(x, y, val > 0 ? 1 : 0));
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

    protected bool IsAlive(int x, int y)
    {
        return Grid[x, y] != 0;
    }

    protected int CountCellNeighbors(int x, int y)
    {
        int neighbors = 0;

        foreach (var neighbor in GetNeighborhood())
        {
            var nx = x + neighbor.X;
            var ny = y + neighbor.Y;

            if (nx >= 0 && nx < Width && ny >= 0 && ny < Height)
            {
                neighbors += Grid[nx, ny];
            }
        }

        return neighbors;
    }

    public override void Update()
    {
        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                var neighbors = CountCellNeighbors(x, y);

                if (IsAlive(x, y))
                {
                    if (!SurvivalRules.Contains(neighbors))
                    {
                        WriteCell(x, y, 0);
                    }
                }
                else
                {
                    if (BirthRules.Contains(neighbors))
                    {
                        WriteCell(x, y, 1);
                    }
                }
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

    public void Randomize(int maxDensity)
    {
        if (maxDensity > 0)
        {
            Density = maxDensity;

            var random = new Random(Guid.NewGuid().GetHashCode());

            for (int i = 0; i < maxDensity; i++)
            {
                var x = random.Next(0, Width);
                var y = random.Next(0, Height);

                WriteCell(x, y, 1);
            }

            ApplyChanges();
        }
    }

    public void Refresh()
    {
        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                if (Grid[x, y] > 0)
                {
                    WriteCell(x, y, 1);
                }
            }
        }
    }

    public override List<Parameter> Parameters()
    {
        var set = new List<Parameter>();

        var density = (Width > 0 && Width > 0) ? (double)Density / (Width * Height) : 0.0;

        set.Add(new Parameter("Density", density, 0.01, 1.0));

        return set;
    }

    public void SetDensity(int density)
    {
        Density = density;
    }

    public Color Color()
    {
        return ColonyColor;
    }
}
