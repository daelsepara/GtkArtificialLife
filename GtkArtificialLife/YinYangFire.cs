using Gdk;
using System.Collections.Generic;

public class YinYangFire : ArtificialLife
{
    int MaxStates = 256;
    int Density;
    int Delta = 1;

    public YinYangFire()
    {
        InitGrid(256, 256);

        ColonyColor = DefaultColor;

        GenerateRandomColorPalette();

        AddMooreNeighborhood();
    }

    public YinYangFire(int width, int height)
    {
        InitGrid(width, height);

        ColonyColor = DefaultColor;

        GenerateRandomColorPalette();

        AddMooreNeighborhood();
    }

    public YinYangFire(int width, int height, Color color)
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

    public void GradientPalette()
    {
        ColorPalette.Clear();

        ColorPalette.AddRange(Utility.Gradient(ColonyColor, MaxStates));
    }

    public void WriteCell(int x, int y, int val)
    {
        if (x >= 0 && x < Width && y >= 0 && y < Height)
        {
            PushPixel(new Pixel(x, y, val >= 0 && val < MaxStates ? ColorPalette[val * Delta] : EmptyColor));

            ChangeList.Add(new Change(x, y, val));
        }
    }

    CountSum CountCellNeighbors(int x, int y, int minVal, int maxVal)
    {
        int neighbors = 0;
        int sum = 0;

        foreach (var neighbor in Neighborhood)
        {
            var nx = Cyclic ? World.Cyclic(x, neighbor.X, Width) : x + neighbor.X;
            var ny = Cyclic ? World.Cyclic(y, neighbor.Y, Height) : y + neighbor.Y;

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

                var CellSum = CountCellNeighbors(x, y, 0, MaxStates).Sum;

                if (state * 9 + 2 >= CellSum)
                {
                    state--;

                    if (state < 0)
                    {
                        state = MaxStates - 1;
                    }
                }
                else
                {
                    state += CellSum;
                }

                WriteCell(x, y, state > MaxStates - 1 ? MaxStates - 1 : state);
            }
        }

        ApplyChanges();
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

    public void Randomize()
    {
        if (Density > 0)
        {
            for (int i = 0; i < Density; i++)
            {
                var x = random.Next(0, Width);
                var y = random.Next(0, Height);
                var val = random.Next(0, MaxStates);

                WriteCell(x, y, val);
            }

            ApplyChanges();
        }
    }

    public override List<Parameter> Parameters()
    {
        var density = (Width > 0 && Width > 0) ? (double)Density / (Width * Height) : 0;

        return new List<Parameter>
        {
            new Parameter("Density", density, (double)1 / 100, 1),
            new Parameter("MaxStates", MaxStates, 2, 256)
        };
    }

    public override void SetParameters(List<Parameter> parameters)
    {
        MaxStates = (int)Utility.GetNumeric(parameters, "MaxStates");
        Density = (int)(Utility.GetNumeric(parameters, "Density") * Width * Height);

        Delta = MaxStates > 0 ? (256 / MaxStates) : 0;
    }
}
