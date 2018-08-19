using Gdk;
using System;
using System.Collections.Generic;

public class Snowflake : ArtificialLife
{
    List<Pixel> PixelWriteBuffer = new List<Pixel>();
    List<Cell> Neighborhood = new List<Cell>();
    List<Change> ChangeList = new List<Change>();
    List<Color> ColorPalette = new List<Color>();
    List<int> GrowthRules = new List<int>();

    string Growth = "1,3,6";
    int[,] Grid;
    int MaxStates = 12;
    int Delta = 1;
    int Current = 0;

    public void GenerateRandomColorPalette()
    {
        ColorPalette.Clear();

        ColorPalette.AddRange(Utility.GenerateRandomColorPalette(ColonyColor));

        Delta = MaxStates > 0 ? (256 / MaxStates) : 1;
    }

    public void GradientPalette()
    {
        ColorPalette.Clear();

        ColorPalette.AddRange(Utility.Gradient(ColonyColor, MaxStates));

        Delta = MaxStates > 0 ? (256 / MaxStates) : 1;
    }

    public void GreyPalette()
    {
        ColorPalette.Clear();

        ColorPalette.AddRange(Utility.GreyPalette());
    }

    public Snowflake()
    {
        InitGrid(256, 256);

        ColonyColor = DefaultColor;

        GenerateRandomColorPalette();

        AddHexNeighborhood();

        WriteCell(128, 128, Utility.NextRandom(1, MaxStates + 1));

        ApplyChanges();
    }

    public Snowflake(int width, int height)
    {
        InitGrid(width, height);

        ColonyColor = DefaultColor;

        GenerateRandomColorPalette();

        AddHexNeighborhood();

        WriteCell(width / 2, height / 2, Utility.NextRandom(1, MaxStates + 1));

        ApplyChanges();
    }

    public Snowflake(int width, int height, Color color)
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

        AddHexNeighborhood();

        WriteCell(width / 2, height / 2, Utility.NextRandom(1, MaxStates + 1));

        ApplyChanges();
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

    public void AddHexNeighborhood()
    {
        Neighborhood.Clear();
        Neighborhood.AddRange(ParameterSets.HexNeighborhood());
    }

    public void AddRules()
    {
        ParseRules(GrowthRules, Growth);
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
            var nx = Cyclic ? Utility.Cyclic(x, neighbor.X, Width) : x + neighbor.X;
            var ny = Cyclic ? Utility.Cyclic(y, neighbor.Y, Height) : y + neighbor.Y;

            if (nx >= 0 && nx < Width && ny >= 0 && ny < Height && Grid[nx, ny] >= minVal && Grid[nx, ny] < maxVal)
            {
                neighbors++;

                sum += Grid[nx, ny];
            }
        }

        return new CountSum(neighbors, sum);
    }

    /*
     * (i) If the value of a cell is 0, then count the cells in its hexagonal neighborhood which have state value greater than 0. If this sum is 1, 3 or 6, then the new state at the center is center value mod 12 + 1; otherwise the new state is 0
     * (ii) f the value of a cell is positive, then its new state value is center value mod 12 + 1 in any case
     */
    public override void Update()
    {
        if (Current < Math.Min(Width / 2, Height / 2))
        {
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    int state = Grid[x, y];
                    int newstate = state;

                    var CellSum = CountCellNeighbors(x, y, 1, MaxStates).Sum;

                    if (state == 0)
                    {
                        newstate = GrowthRules.Contains(CellSum) ? (state % MaxStates) + 1 : 0;
                    }
                    else
                    {
                        newstate = (state % MaxStates) + 1;
                    }

                    if (state != newstate || newstate > 0)
                        WriteCell(x, y, newstate);
                }
            }

            Current++;
        }
        else
        {
            Refresh();
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

    public override List<Parameter> Parameters()
    {
        var set = new List<Parameter>
        {
            new Parameter("Growth", Growth),
            new Parameter("MaxStates", MaxStates, 1, 256)
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

    public void SetParameters(string growth, int maxStates)
    {
        if (!String.IsNullOrEmpty(growth))
            Growth = growth;

        if (maxStates > 0)
        {
            MaxStates = maxStates;
            Delta = maxStates > 0 ? (256 / maxStates) : 0;
        }
    }

    public void ParseRules(List<int> Set, string rules)
    {
        if (!String.IsNullOrEmpty(rules))
        {
            var conditions = rules.Split(',');

            if (conditions.Length > 0)
            {
                Set.Clear();

                for (int i = 0; i < conditions.Length; i++)
                {
                    try
                    {
                        var count = Convert.ToInt32(conditions[i]);

                        if (count > 0 && !Set.Contains(count))
                            Set.Add(count);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("{0}: Unable to convert: {1}", ex.Message, conditions[i]);
                    }
                }
            }
        }
    }
}
