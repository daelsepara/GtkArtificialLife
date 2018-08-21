using Gdk;
using System.Collections.Generic;

public class LangtonAnt : ArtificialLife
{
    public class Rule
    {
        public int TurnDirection;
        public Color TrailColor;

        public Rule(int turnDirection, Color color)
        {
            TurnDirection = turnDirection;
            TrailColor = color;
        }
    }

    public class Ant
    {
        public int X;
        public int Y;
        public int MoveDirection;

        public List<Movement> Moves = new List<Movement>();

        public class Movement
        {
            public int DX;
            public int DY;

            public Movement(int dx, int dy)
            {
                DX = dx;
                DY = dy;
            }
        }

        public Ant()
        {
            X = 0;
            Y = 0;

            MoveDirection = 0;
        }

        public Ant(int x, int y, int moveDirection)
        {
            X = x;
            Y = y;

            MoveDirection = moveDirection;
        }
    }

    readonly List<Ant> Ants = new List<Ant>();
    List<Rule> Rules = new List<Rule>();
    string RuleString;

    bool Randomize;

    public LangtonAnt()
    {
        InitGrid(256, 256);

        ColonyColor = DefaultColor;
    }

    public LangtonAnt(int width, int height)
    {
        InitGrid(width, height);

        ColonyColor = DefaultColor;

        GenerateRandomColorPalette();
    }

    public LangtonAnt(int width, int height, Color color)
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
    }

    public void GradientPalette()
    {
        ColorPalette.Clear();

        ColorPalette.AddRange(Utility.Gradient(ColonyColor, RuleString.Length));
    }

    public void WriteCell(int x, int y, int val)
    {
        if (x >= 0 && x < Width && y >= 0 && y < Height)
        {
            ChangeList.Add(new Change(x, y, val));
        }
    }

    void WriteCell(Ant ant, int val)
    {
        if (ant.X >= 0 && ant.X < Width && ant.Y >= 0 && ant.Y < Height)
        {
            PushPixel(new Pixel(ant.X, ant.Y, Rules[val].TrailColor));

            WriteCell(ant.X, ant.Y, val);
        }
    }

    public override void Update()
    {
        Refresh();

        foreach (var ant in Ants)
        {
            if (ant.X >= 0 && ant.X < Width && ant.Y >= 0 && ant.Y < Height)
            {
                var val = Grid[ant.X, ant.Y];

                WriteCell(ant, (val + 1) % Rules.Count);

                if (Rules[val].TurnDirection == 0)
                {
                    ant.MoveDirection = (ant.MoveDirection + (ant.Moves.Count - 1)) % ant.Moves.Count;
                }
                else
                {
                    ant.MoveDirection = (ant.MoveDirection + 1) % ant.Moves.Count;
                }

                // Move ant in specified direction
                ant.X = Cyclic ? World.Cyclic(ant.X, ant.Moves[ant.MoveDirection].DX, Width) : ant.X + ant.Moves[ant.MoveDirection].DX;
                ant.Y = Cyclic ? World.Cyclic(ant.Y, ant.Moves[ant.MoveDirection].DY, Height) : ant.Y + ant.Moves[ant.MoveDirection].DY;
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
                    PushPixel(new Pixel(x, y, Rules[Grid[x, y]].TrailColor));
                }
            }
        }
    }

    public void Random(bool randomize = true)
    {
        Randomize = randomize;
    }

    public void ParseRules(List<Rule> Rules, string rules, List<Color> ColorPalette, bool randomize = true)
    {
        if (rules.Length > 0)
        {
            var Delta = 256 / rules.Length;

            for (int i = 0; i < rules.Length; i++)
            {
                if (rules[i] == 'R' || rules[i] == 'r')
                {
                    if (i == 0)
                    {
                        Rules.Add(new Rule(0, new Color(0, 0, 0)));
                    }
                    else
                    {
                        Rules.Add(new Rule(0, randomize ? ColorPalette[random.Next(0, ColorPalette.Count)] : ColorPalette[i * Delta]));
                    }
                }
                else
                {
                    Rules.Add(new Rule(1, randomize ? ColorPalette[random.Next(0, ColorPalette.Count)] : ColorPalette[i * Delta]));
                }
            }
        }
    }

    public void AddMoves(Ant ant)
    {
        foreach (var cell in Neighborhood)
        {
            ant.Moves.Add(new Ant.Movement(cell.X, cell.Y));
        }
    }

    void GenerateAnts(int ants)
    {
        if (!Randomize)
            GradientPalette();

        Rules.Clear();

        ParseRules(Rules, RuleString, ColorPalette, Randomize);

        for (int i = 0; i < ants; i++)
        {
            var x = random.Next(0, Width);
            var y = random.Next(0, Height);
            var dir = random.Next(0, 4);

            var ant = new Ant(x, y, dir);

            AddMoves(ant);
            Ants.Add(ant);

            WriteCell(ant, random.Next(0, RuleString.Length));
        }
    }

    public override List<Parameter> Parameters()
    {
        return new List<Parameter>
        {
            new Parameter("Ants", Ants.Count, 1, 1000),
            new Parameter("Rule", RuleString)
        };
    }

    public override void SetParameters(List<Parameter> parameters)
    {
        var ants = (int)Utility.GetNumeric(parameters, "Ants");

        RuleString = Utility.GetString(parameters, "Rule");

        if (ants > 0 && RuleString.Length > 0)
        {
            GenerateAnts(ants);

            ApplyChanges();
        }
    }
}
