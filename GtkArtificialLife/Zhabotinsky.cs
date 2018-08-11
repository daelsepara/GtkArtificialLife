using Gdk;
using System;
using System.Collections.Generic;

public class Zhabotinsky : ArtificialLife
{
    List<Pixel> PixelWriteBuffer = new List<Pixel>();
    List<Cell> Neighborhood = new List<Cell>();
    List<Change> ChangeList = new List<Change>();
    List<Color> ColorPalette = new List<Color>();

    int[,] Grid;

    const int MaxStates = 256;

    double Density = 1;
    double K1 = 1;
    double K2 = 1;
    double G = 10;

    public void GenerateRandomColorPalette()
    {
        ColorPalette.Clear();

        ColorPalette.AddRange(Utility.GenerateRandomColorPalette(ColonyColor));
    }

    public void GradientPalette()
    {
        ColorPalette.Clear();

        ColorPalette.AddRange(Utility.Gradient(ColonyColor));
    }

    public void GreyPalette()
    {
        ColorPalette.Clear();

        ColorPalette.AddRange(Utility.GreyPalette());
    }

    public Zhabotinsky()
    {
        InitGrid(256, 256);

        ColonyColor = DefaultColor;

        GenerateRandomColorPalette();

        AddMooreNeighborhood();
    }

    public Zhabotinsky(int width, int height)
    {
        InitGrid(width, height);

        ColonyColor = DefaultColor;

        GenerateRandomColorPalette();

        AddMooreNeighborhood();
    }

    public Zhabotinsky(int width, int height, Color color)
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

    public void SetParameters(double k1, double k2, double g)
    {
        K1 = k1;
        K2 = k2;
        G = g;
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

    /*
	 * (i) If the cell is healthy (i.e., in state 0) then its new state is [a/k1] + [b/k2], where a is the number of infected cells among its eight neighbors, b is the number of ill cells among its neighbors, and k1 and k2 are constants. Here "[]" means the integer part of the number enclosed, so that, for example, [7/3] = [2+1/3] = 2.
	 * (ii) If the cell is ill (i.e., in state n) then it miraculously becomes healthy (i.e., its state becomes 0).
	 * (iii) If the cell is infected (i.e., in a state other than 0 and n) then its new state is [s/(a+b+1)] + g, where a and b are as above, s is the sum of the states of the cell and of its neighbors and g is a constant.
	 */
    public override void Update()
    {
        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                int state = Grid[x, y];

                if (Grid[x, y] == MaxStates - 1)
                {
                    state = 0;
                }
                else if (Grid[x, y] == 0)
                {
                    var a = CountCellNeighbors(x, y, 1, MaxStates - 1);
                    var b = CountCellNeighbors(x, y, MaxStates - 1, MaxStates);

                    state = (int)(a.Count / K1 + b.Count / K2);
                }
                else
                {
                    var S = CountCellNeighbors(x, y, 0, MaxStates).Sum + Grid[x, y];
                    var a = CountCellNeighbors(x, y, 1, MaxStates - 1);
                    var b = CountCellNeighbors(x, y, MaxStates - 1, MaxStates);

                    state = (int)(S / (double)(a.Count + b.Count + 1) + G);
                }

                WriteCell(x, y, state > MaxStates - 1 ? MaxStates - 1 : state);
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
            var random = new Random(Guid.NewGuid().GetHashCode());

            for (int i = 0; i < maxDensity; i++)
            {
                var x = random.Next(0, Width);
                var y = random.Next(0, Height);
                var val = random.Next(0, MaxStates);

                WriteCell(x, y, val);
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

        var set = new List<Parameter>
        {
            new Parameter("Density", density, 0.01, 1.0),
            new Parameter("g", G, 1, 100),
            new Parameter("k1", K1, 1, 100),
            new Parameter("k2", K2, 1, 100)
        };

        return set;
    }

    public void SetDensity(int density)
    {
        Density = density;
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
