using Gdk;
using System;
using System.Collections.Generic;

public class Life : ArtificialLife
{
    readonly List<int> BirthRules = new List<int>();
    readonly List<int> SurvivalRules = new List<int>();

    string Birth = "3";
    string Survival = "2,3";

    int Density;

    public Life()
    {
        InitGrid(256, 256);

        ColonyColor = DefaultColor;

        AddMooreNeighborhood();

        GenerateColorPalette();
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

    public void GenerateColorPalette()
    {
        ColorPalette.Clear();

        // Generate Gradient
        ColorPalette.AddRange(Utility.Gradient(ColonyColor));

        ColorPalette[0] = EmptyColor;
        ColorPalette[1] = ColonyColor;
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

                WriteCell(x, y, 1);
            }

            ApplyChanges();
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
        var density = (Width > 0 && Height > 0) ? (double)Density / (Width * Height) : 0;

        return new List<Parameter>
        {
            new Parameter("Density", density, (double)1 / 100, 1),
            new Parameter("Birth", Birth),
            new Parameter("Survival", Survival)
        };
    }

    public override void SetParameters(List<Parameter> parameters)
    {
        Density = (int)(Utility.GetNumeric(parameters, "Density") * Width * Height);
        Birth = Utility.GetString(parameters, "Birth");
        Survival = Utility.GetString(parameters, "Survival");

        ParseRules(BirthRules, Birth);
        ParseRules(SurvivalRules, Survival);
    }
}
