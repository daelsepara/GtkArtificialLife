using Gdk;
using System;
using System.Collections.Generic;

public class ForestFire : ArtificialLife
{
    const int MaxStates = 3;
    double Density = 1;
    double F = 100;
    double P = 1000;
    double scale = 1.0 / 1000.0;
    const int Empty = 0;
    const int Normal = 1;
    const int Burning = 2;

    public void GenerateColorPalette()
    {
        ColorPalette.Clear();

        // Generate Gradient
        ColorPalette.AddRange(Utility.Gradient(ColonyColor));

        // Preserve colors for tree states
        var max = Math.Max(ColonyColor.Red, Math.Max(ColonyColor.Green, ColonyColor.Blue));
        ColorPalette[Empty] = EmptyColor;
        ColorPalette[Normal] = new Color((byte)(ColonyColor.Red / max * 128), (byte)(ColonyColor.Green / max * 128), (byte)(ColonyColor.Blue / max * 128));
        ColorPalette[Burning] = ColonyColor;
    }

    public ForestFire()
    {
        InitGrid(256, 256);

        ColonyColor = DefaultColor;

        GenerateColorPalette();

        AddMooreNeighborhood();
    }

    public ForestFire(int width, int height)
    {
        InitGrid(width, height);

        ColonyColor = DefaultColor;

        GenerateColorPalette();

        AddMooreNeighborhood();
    }

    public ForestFire(int width, int height, Color color)
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

        GenerateColorPalette();

        AddMooreNeighborhood();
    }

    public void SetParameters(double f, double p)
    {
        F = f;
        P = p;
    }

    public void WriteCell(int x, int y, int val)
    {
        if (x >= 0 && x < Width && y >= 0 && y < Height)
        {
            PushPixel(new Pixel(x, y, ColorPalette[val]));

            ChangeList.Add(new Change(x, y, val));
        }
    }

    CountSum CountNeighbors(int x, int y, int state)
    {
        var neighbors = 0;
        var sum = 0;

        foreach (var neighbor in Neighborhood)
        {
            var nx = Cyclic ? World.Cyclic(x, neighbor.X, Width) : x + neighbor.X;
            var ny = Cyclic ? World.Cyclic(y, neighbor.Y, Height) : y + neighbor.Y;

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
                var state = Grid[x, y];
                var newstate = state;

                switch (state)
                {
                    case Empty:

                        newstate = state;

                        break;

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

                        // deteriorate
                        newstate = state - 1;

                        break;
                }

                if (state != newstate || newstate > 0)
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

        // Random lightning strike / burning / deterioration event
        for (int i = 0; i < (int)F; i++)
        {
            var burnx = random.Next(0, Width);
            var burny = random.Next(0, Height);
            var state = Grid[burnx, burny];

            if (state == Normal || state > Burning)
            {
                if (random.NextDouble() < F * scale)
                {
                    if (state == Normal)
                    {
                        WriteCell(burnx, burny, Burning);
                    }
                    else
                    {
                        WriteCell(burnx, burny, state - 1);
                    }
                }
            }
        }

        ApplyChanges();
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

    public override List<Parameter> Parameters()
    {
        var density = (Width > 0 && Width > 0) ? Density / (Width * Height) : 0.0;

        return new List<Parameter>
        {
            new Parameter("Density", density, 0.01, 1.0),
            new Parameter("P", P, 1, 1000),
            new Parameter("F", F, 1, 1000)
        };
    }

    public void SetDensity(int density)
    {
        Density = density;
    }
}
