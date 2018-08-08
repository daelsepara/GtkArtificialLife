using Gdk;
using System;
using System.Collections.Generic;

public class ForestFire : ArtificialLife
{
    List<Pixel> PixelWriteBuffer = new List<Pixel>();
    List<Cell> Neighborhood = new List<Cell>();
    List<Change> ChangeList = new List<Change>();
    List<Color> ColorPalette = new List<Color>();
    int[,] Grid;
    const int MaxStates = 3;
    double Density = 1;
    double F = 100;
    double P = 1000;
    double scale = 1.0 / 1000.0;
    const int Empty = 0;
    const int Normal = 1;
    const int Burning = 2;
    Random random = new Random(Guid.NewGuid().GetHashCode());

    public void GenerateColorPalette()
    {
        ColorPalette.Add(EmptyColor);
        ColorPalette.Add(ColonyColor);
        ColorPalette.Add(new Color((byte)(ColonyColor.Red / 2), (byte)(ColonyColor.Green / 2), (byte)(ColonyColor.Blue / 2)));
    }

    public ForestFire()
    {
        ColonyColor = DefaultColor;

        InitGrid(256, 256);

        GenerateColorPalette();
    }

    public ForestFire(int width, int height)
    {
        InitGrid(width, height);

        ColonyColor = DefaultColor;

        GenerateColorPalette();
    }

    public ForestFire(int width, int height, Color color)
    {
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

        GenerateColorPalette();

        InitGrid(width, height);
    }

    public void SetParameters(double f, double p)
    {
        F = f;
        P = p;
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

    public void WriteCell(int x, int y, int val)
    {
        if (x >= 0 && x < Width && y >= 0 && y < Height)
        {
            PushPixel(new Pixel(x, y, val >= 0 && val < MaxStates ? ColorPalette[val] : EmptyColor));

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

    protected CountSum CountNeighbors(int x, int y, int state)
    {
        int neighbors = 0;
        int sum = 0;

        foreach (var neighbor in GetNeighborhood())
        {
            var nx = x + neighbor.X;
            var ny = y + neighbor.Y;

            if (nx >= 0 && nx < Width && ny >= 0 && ny < Height && Grid[nx, ny] == state)
            {
                neighbors++;

                sum += Grid[nx, ny];
            }
        }

        return new CountSum(neighbors, sum);
    }

    public override void Update()
    {
        // Update state
        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                int state = Grid[x, y];
                int newstate = state;

                switch (state)
                {
                    case Normal:

                        var burning = CountNeighbors(x, y, Burning);

                        if (burning.Count > 0)
                        {
                            newstate = Burning;
                        }

                        break;

                    case Burning:

                        newstate = Empty;

                        break;

                    default:

                        newstate = state;

                        break;
                }

                WriteCell(x, y, newstate);
            }
        }

        // Random planting
        for (int i = 0; i < (int)P; i++)
        {
            var plantx = random.Next(0, Width);
            var planty = random.Next(0, Height);

            if (Grid[plantx, planty] == Empty)
            {
                if (random.NextDouble() < P * scale)
                {
                    WriteCell(plantx, planty, Normal);
                }
            }
        }

        // Random lightning strike / burning event
        for (int i = 0; i < (int)F; i++)
        {
            var burnx = random.Next(0, Width);
            var burny = random.Next(0, Height);

            if (Grid[burnx, burny] == Normal)
            {
                if (random.NextDouble() < F * scale)
                {
                    WriteCell(burnx, burny, Burning);
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
        Density = maxDensity;

        if (maxDensity > 0)
        {
            for (int i = 0; i < maxDensity; i++)
            {
                var x = random.Next(0, Width);
                var y = random.Next(0, Height);

                WriteCell(x, y, Normal);
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
                    WriteCell(x, y, Grid[x, y]);
                }
            }
        }
    }

    public override List<Parameter> Parameters()
    {
        var density = (Width > 0 && Width > 0) ? Density / (Width * Height) : 0.0;

        var set = new List<Parameter>
        {
            new Parameter("Density", density, 0.01, 1.0),
            new Parameter("P", P, 1, 1000),
            new Parameter("F", F, 1, 1000)
        };

        return set;
    }

    public void WriteGrid(int x, int y, int val)
    {
        if (x >= 0 && x < Width && y >= 0 && y < Height)
        {
            Grid[x, y] = val;
            WriteCell(x, y, Grid[x, y]);
        }
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
