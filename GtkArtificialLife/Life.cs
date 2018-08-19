using Gdk;
using System;
using System.Collections.Generic;

public class Life : ArtificialLife
{
    List<int> BirthRules = new List<int>();
    List<int> SurvivalRules = new List<int>();

    String Birth = "3";
    String Survival = "2,3";

    int Density;

    public void GenerateColorPalette()
    {
        ColorPalette.Clear();

        // Generate Gradient
        ColorPalette.AddRange(Utility.Gradient(ColonyColor));

        ColorPalette[0] = EmptyColor;
        ColorPalette[1] = ColonyColor;
    }

    public Life()
    {
        InitGrid(256, 256);

        ColonyColor = DefaultColor;

        AddMooreNeighborhood();
    }

    public Life(int width, int height)
    {
        InitGrid(width, height);

        ColonyColor = DefaultColor;

        AddMooreNeighborhood();

        GenerateColorPalette();
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

        GenerateColorPalette();
    }

    public void AddRules()
    {
        ParseRules(BirthRules, Birth);
        ParseRules(SurvivalRules, Survival);
    }

    public void WriteCell(int x, int y, int val)
    {
        if (x >= 0 && x < Width && y >= 0 && y < Height)
        {
            PushPixel(new Pixel(x, y, val >= 0 && val < ColorPalette.Count ? ColorPalette[val] : EmptyColor));

            ChangeList.Add(new Change(x, y, val));
        }
    }

    bool IsAlive(int x, int y)
    {
        return Grid[x, y] > 0;
    }

    int CountCellNeighbors(int x, int y)
    {
        int neighbors = 0;

        foreach (var neighbor in GetNeighborhood())
        {
            var nx = Cyclic ? World.Cyclic(x, neighbor.X, Width) : x + neighbor.X;
            var ny = Cyclic ? World.Cyclic(y, neighbor.Y, Height) : y + neighbor.Y;

            if (nx >= 0 && nx < Width && ny >= 0 && ny < Height)
            {
                neighbors += IsAlive(nx, ny) ? 1 : 0;
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

                var state = Grid[x, y];
                var newstate = state;

                if (IsAlive(x, y))
                {
                    if (!SurvivalRules.Contains(neighbors))
                    {
                        newstate = 0;
                    }
                }
                else
                {
                    if (BirthRules.Contains(neighbors))
                    {
                        newstate = 1;
                    }
                }

                if (state != newstate || newstate > 0)
                    WriteCell(x, y, newstate);
            }
        }

        ApplyChanges();
    }

    public void Randomize(int maxDensity)
    {
        if (maxDensity > 0)
        {
            Density = maxDensity;

            for (int i = 0; i < maxDensity; i++)
            {
                var x = random.Next(0, Width);
                var y = random.Next(0, Height);

                WriteCell(x, y, 1);
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
        var density = (Width > 0 && Width > 0) ? (double)Density / (Width * Height) : 0.0;

        return new List<Parameter>
        {
            new Parameter("Density", density, 0.01, 1.0),
            new Parameter("Birth", Birth),
            new Parameter("Survival", Survival)
        };
    }

    public void SetDensity(int density)
    {
        Density = density;
    }

    public void SetParameters(string birth, string survival)
    {
        if (!String.IsNullOrEmpty(birth))
            Birth = birth;

        if (!String.IsNullOrEmpty(survival))
            Survival = survival;
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
