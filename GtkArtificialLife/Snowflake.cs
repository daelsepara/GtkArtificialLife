using Gdk;
using System;
using System.Collections.Generic;

public class Snowflake : ArtificialLife
{
    List<int> GrowthRules = new List<int>();

    string Growth = "1,3,6";
    int MaxStates = 12;
    int Delta = 1;
    int Current;

    public Snowflake()
    {
        InitGrid(256, 256);

        ColonyColor = DefaultColor;

        GenerateRandomColorPalette();

        AddHexNeighborhood();

        WriteCell(128, 128, random.Next(1, MaxStates + 1));

        Delta = MaxStates > 0 ? (256 / MaxStates) : 1;

        ApplyChanges();
    }

    public Snowflake(int width, int height)
    {
        InitGrid(width, height);

        ColonyColor = DefaultColor;

        GenerateRandomColorPalette();

        AddHexNeighborhood();

        WriteCell(width / 2, height / 2, random.Next(1, MaxStates + 1));

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

        WriteCell(width / 2, height / 2, random.Next(1, MaxStates + 1));

        ApplyChanges();
    }

    public void GradientPalette()
    {
        ColorPalette.Clear();

        ColorPalette.AddRange(Utility.Gradient(ColonyColor, MaxStates));

        Delta = MaxStates > 0 ? (256 / MaxStates) : 1;
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


    public override List<Parameter> Parameters()
    {
        return new List<Parameter>
        {
            new Parameter("Growth", Growth),
            new Parameter("MaxStates", MaxStates, 1, 256)
        };
    }

    public override void SetParameters(List<Parameter> parameters)
    {
        MaxStates = (int)Utility.GetNumeric(parameters, "MaxStates");
        Growth = Utility.GetString(parameters, "Growth");

        Delta = MaxStates > 0 ? (256 / MaxStates) : 0;

        ParseRules(GrowthRules, Growth);
    }
}
