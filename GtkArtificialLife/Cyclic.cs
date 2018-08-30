using Gdk;
using System.Collections.Generic;

public class Cyclic : ArtificialLife
{
    int MaxStates = 8;
    int Delta = 1;

    public Cyclic()
    {
        InitGrid(256, 256);

        ColonyColor = DefaultColor;

        GenerateRandomColorPalette();

        AddVonNeumannNeighborhood();
    }

    public Cyclic(int width, int height)
    {
        InitGrid(width, height);

        ColonyColor = DefaultColor;

        GenerateRandomColorPalette();

        AddVonNeumannNeighborhood();
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

        AddVonNeumannNeighborhood();
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

        foreach (var neighbor in GetNeighborhood())
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
        return new List<Parameter>
        {
            new Parameter("MaxStates", MaxStates, 2, 256)
        };
    }

    public override void SetParameters(List<Parameter> parameters)
    {
        MaxStates = (int)Utility.GetNumeric(parameters, "MaxStates");

        Delta = MaxStates > 0 ? (256 / MaxStates) : 0;
    }
}
