using Gdk;
using System;
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
        Random random;

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

        protected void InitializeSeed()
        {
            if (random == null)
            {
                random = new Random(Guid.NewGuid().GetHashCode());
            }
        }

        public Ant()
        {
            X = 0;
            Y = 0;

            MoveDirection = 0;

            InitializeSeed();
        }

        public Ant(int x, int y, int moveDirection)
        {
            X = x;
            Y = y;

            MoveDirection = moveDirection;
        }
    }

    List<Pixel> PixelWriteBuffer = new List<Pixel>();
    List<Change> ChangeList = new List<Change>();
    readonly List<Ant> Ants = new List<Ant>();
    List<Rule> Rules = new List<Rule>();

    string RuleString;
    int[,] Grid;
    Random random;
    readonly List<Color> ColorPalette = new List<Color>();

    public void GenerateRandomColorPalette()
    {
        ColorPalette.Clear();

        ColorPalette.AddRange(Utility.GenerateRandomColorPalette(ColonyColor));
    }

    public void GradientPalette()
    {
        ColorPalette.Clear();

        ColorPalette.AddRange(Utility.Gradient(ColonyColor, RuleString.Length));
    }

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

    public void WriteCell(Ant ant, int val)
    {
        if (ant.X >= 0 && ant.X < Width && ant.Y >= 0 && ant.Y < Height)
        {
            PushPixel(new Pixel(ant.X, ant.Y, val >= 0 ? Rules[val].TrailColor : EmptyColor));
            ChangeList.Add(new Change(ant.X, ant.Y, val));
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

    public void ParseRules(List<Rule> Rules, string rules, List<Color> ColorPalette, bool randomize = true)
    {
        InitializeSeed();

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
                ant.X += ant.Moves[ant.MoveDirection].DX;
                ant.Y += ant.Moves[ant.MoveDirection].DY;
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

    protected void InitializeSeed()
    {
        if (random == null)
        {
            random = new Random(Guid.NewGuid().GetHashCode());
        }
    }

    public void AddMoves(Ant ant)
    {
        ant.Moves.Add(new Ant.Movement(0, -1));
        ant.Moves.Add(new Ant.Movement(1, 0));
        ant.Moves.Add(new Ant.Movement(0, 1));
        ant.Moves.Add(new Ant.Movement(-1, 0));
    }

    public void Randomize(int ants, string rules, bool randomize =  true)
    {
        InitializeSeed();

        if (ants > 0 && rules.Length > 0)
        {
            RuleString = rules;
            ParseRulesOnce(randomize);

            for (int i = 0; i < ants; i++)
            {
                var x = random.Next(0, Width);
                var y = random.Next(0, Height);
                var dir = random.Next(0, 4);

                var ant = new Ant(x, y, dir);

                AddMoves(ant);
                Ants.Add(ant);

                WriteCell(ant, random.Next(0, rules.Length));
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
                    PushPixel(new Pixel(x, y, Rules[Grid[x, y]].TrailColor));
                }
            }
        }
    }

    public override List<Parameter> Parameters()
    {
        var set = new List<Parameter>
        {
            new Parameter("Ants", Ants.Count, 1, 1000),
            new Parameter("Rule", RuleString)
        };

        return set;
    }

    public void ParseRulesOnce(bool randomize = true)
    {
        Rules.Clear();
        ParseRules(Rules, RuleString, ColorPalette, randomize);
    }

    public void WriteCell(int x, int y, int val)
    {
        if (x >= 0 && x < Width && y >= 0 && y < Height)
        {
            ChangeList.Add(new Change(x, y, val));
        }
    }

    public void SetRules(string rules)
    {
        RuleString = rules;
    }

    public override Color Color()
    {
        return ColonyColor;
    }

    public override List<Cell> GetNeighborhood()
    {
        return new List<Cell>();
    }

    public override void SetNeighborhood(List<Cell> neighborhood)
    {

    }
}
