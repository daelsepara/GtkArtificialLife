using Gdk;
using System;
using System.Collections.Generic;

public class TuringMachine : ArtificialLife
{
    class Tuple
    {
        public int PresentState;
        public int Read;
        public int Write;
        public int NewState;
        public int Turn;

        public Tuple(int presentState, int read, int write, int newState, int turn)
        {
            PresentState = presentState;
            Read = read;
            Write = write;
            NewState = newState;
            Turn = turn;
        }
    }

    class Point
    {
        public int X, Y;

        public Point(int x, int y)
        {
            X = x;
            Y = y;
        }
    }

    class Turn
    {
        public int Direction;

        public Turn(int direction)
        {
            Direction = direction;
        }
    }

    class Machine
    {
        public Point Head = new Point(0, 0);
        public int State = 1;
        public int Color;
        public int Direction;

        public List<string> Colors = new List<string>();
        public List<string> Directions = new List<string>();

        int Width;
        int Height;
        bool Cyclic;

        public readonly List<Tuple> Program = new List<Tuple>();
        public List<Turn> Turns = new List<Turn>();
        public List<Point> Moves = new List<Point>();

        string ParseString(string code, List<string> codebook)
        {
            string pattern = "";

            if (code.Length > 0)
            {
                int offset = 0;

                var current = "";

                while (offset < code.Length && !char.IsDigit(code[offset]))
                {
                    current += code[offset];

                    if (codebook.Contains(current))
                    {
                        pattern = current;
                    }

                    offset++;
                }
            }

            return pattern;
        }

        string ParseDigit(string code)
        {
            string pattern = "";

            if (code.Length > 0)
            {
                int offset = 0;

                while (char.IsDigit(code[offset]))
                {
                    pattern += code[offset];
                    offset++;
                }
            }

            return pattern;
        }

        string Slice(string source, int offset)
        {
            return source.Substring(offset);
        }

        void Extract(ref string pattern, ref int variable)
        {
            variable = -1;

            var code = ParseDigit(pattern);

            if (code.Length > 0)
            {
                pattern = Slice(pattern, code.Length);
                variable = Convert.ToInt32(code);
            }
        }

        void Extract(ref string pattern, ref int variable, ref List<string> limits)
        {
            variable = -1;

            var code = ParseString(pattern, limits);

            if (code.Length > 0)
            {
                pattern = Slice(pattern, code.Length);

                if (limits.Contains(code))
                {
                    variable = limits.FindIndex(item => item == code);
                }
            }
        }

        public void AddColor(string color)
        {
            if (color.Length > 0)
            {
                if (!Colors.Contains(color))
                    Colors.Add(color);
            }
        }

        public void AddMove(int DeltaX, int DeltaY)
        {
            var movement = new Point(DeltaX, DeltaY);

            if (!Moves.Contains(movement))
            {
                Moves.Add(movement);
            }
        }

        public void AddTurn(int Direction, string Name)
        {
            if (Name.Length > 0)
            {
                var turn = new Turn(Direction);

                if (!Turns.Contains(turn) && !Directions.Contains(Name))
                {
                    Turns.Add(turn);

                    Directions.Add(Name);
                }
            }
        }

        void Clip(ref int Value, int Limit)
        {
            // clip to lower bound
            var clipped = Math.Max(0, Value);

            // clip to upper bound
            Value = Math.Min(clipped, Limit - 1);
        }

        public void ParseProgram(string source)
        {
            int PresentState = 0;
            int Read = 0;
            int Write = 0;
            int NewState = 0;
            int Turn = 0;

            var codes = source.Split(',');

            if (codes.Length > 0)
            {
                foreach (var code in codes)
                {
                    string copy = code.Trim();

                    Extract(ref copy, ref PresentState);
                    Extract(ref copy, ref Read, ref Colors);
                    Extract(ref copy, ref Write, ref Colors);
                    Extract(ref copy, ref NewState);
                    Extract(ref copy, ref Turn, ref Directions);

                    if (PresentState >= 0 && Read >= 0 && Write >= 0 && NewState >= 0 && Turn >= 0)
                    {
                        var tuple = new Tuple(PresentState, Read, Write, NewState, Turn);

                        if (!Program.Contains(tuple))
                        {
                            Program.Add(tuple);
                        }
                    }
                }
            }
        }

        public void Move()
        {
            Head.X = Cyclic ? World.Cyclic(Head.X, Moves[Direction].X, Width) : Head.X + Moves[Direction].X;
            Head.Y = Cyclic ? World.Cyclic(Head.Y, Moves[Direction].Y, Height) : Head.Y + Moves[Direction].Y;

            Clip(ref Head.X, Width);
            Clip(ref Head.Y, Height);
        }

        public List<Tuple> ConsultProgram(int state, int color)
        {
            var Tuples = new List<Tuple>();

            if (Program.Count > 0)
            {
                foreach (var tuple in Program)
                {
                    if (tuple.PresentState == state && tuple.Read == color)
                        Tuples.Add(tuple);
                }
            }

            return Tuples;
        }

        public void ExecuteProgram(int state, int Read, List<Tuple> procedures)
        {
            foreach (var procedure in procedures)
            {
                if (procedure.PresentState == state && Read == procedure.Read)
                {
                    Color = procedure.Write;
                    State = procedure.NewState;
                    Direction = World.Cyclic(Direction, Turns[procedure.Turn].Direction, Moves.Count);
                }
            }
        }

        public void SetLimits(int width, int height)
        {
            Width = width;
            Height = height;
        }

        public void SetCyclic(bool cyclic)
        {
            Cyclic = cyclic;
        }

        public void List()
        {
            if (Program.Count > 0)
            {
                foreach (var tuple in Program)
                {
                    Console.WriteLine("{0}{1}{2}{3}{4}", tuple.PresentState, Colors[tuple.Read], Colors[tuple.Write], tuple.NewState, Directions[tuple.Turn]);
                }
            }
        }
    }

    string ColorString = "BXYMCRGW";
    string Source;
    int CellStates = 2;
    int Delta = 1;

    List<Machine> Machines = new List<Machine>();

    public TuringMachine()
    {
        InitGrid(256, 256);

        ColonyColor = DefaultColor;

        AddVonNeumannNeighborhood();
    }

    public TuringMachine(int width, int height)
    {
        InitGrid(width, height);

        ColonyColor = DefaultColor;

        AddVonNeumannNeighborhood();
    }

    public TuringMachine(int width, int height, Color color)
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

        AddVonNeumannNeighborhood();
    }

    public void GradientPalette()
    {
        ColorPalette.Clear();

        ColorPalette.AddRange(Utility.Gradient(ColonyColor, CellStates));
    }

    public void WriteCell(int x, int y, int val)
    {
        if (x >= 0 && x < Width && y >= 0 && y < Height)
        {
            PushPixel(new Pixel(x, y, val >= 0 && val < CellStates ? ColorPalette[val * Delta] : EmptyColor));

            ChangeList.Add(new Change(x, y, val));
        }
    }

    public override void Update()
    {
        Refresh();

        for (int i = 0; i < Machines.Count; i++)
        {
            var machine = Machines[i];

            machine.Move();

            var color = Grid[machine.Head.X, machine.Head.Y] % machine.Colors.Count;
            var state = machine.State;

            var procedures = machine.ConsultProgram(state, color);

            if (procedures.Count > 0)
            {
                machine.ExecuteProgram(state, color, procedures);

                if (color != machine.Color || machine.Color > 0)
                    WriteCell(machine.Head.X, machine.Head.Y, machine.Color);

                ApplyChanges();
            }
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

    void AddMoves(Machine machine)
    {
        machine.Moves.Clear();

        foreach (var cell in Neighborhood)
        {
            machine.AddMove(cell.X, cell.Y);
        }
    }

    void AddTurns(Machine machine)
    {
        machine.Turns.Clear();
        machine.Directions.Clear();

        machine.AddTurn(-1, "L");
        machine.AddTurn(1, "R");

        // Add backward turn
        machine.AddTurn((int)Math.Ceiling((double)Neighborhood.Count / 2), "B");

        // Add No-Turn
        machine.AddTurn(0, "S");
    }

    void AddColors(Machine machine)
    {
        machine.Colors.Clear();

        for (int i = 0; i < CellStates; i++)
        {
            if (i < ColorString.Length)
                machine.AddColor(ColorString[i].ToString());
        }
    }

    void GenerateMachines(int machines)
    {
        for (int i = 0; i < machines; i++)
        {
            var machine = new Machine();

            AddColors(machine);
            AddMoves(machine);
            AddTurns(machine);

            machine.ParseProgram(Source);
            machine.SetCyclic(Cyclic);
            machine.SetLimits(Width, Height);

            machine.Head.X = random.Next(0, Width);
            machine.Head.Y = random.Next(0, Height);
            machine.State = 1;
            machine.Direction = random.Next(0, machine.Moves.Count);

            Machines.Add(machine);

            WriteCell(machine.Head.X, machine.Head.Y, random.Next(0, CellStates));
        }

        ApplyChanges();
    }

    public override List<Parameter> Parameters()
    {
        return new List<Parameter>
        {
            new Parameter("Machines", Machines.Count, 1, 10000),
            new Parameter("CellStates", CellStates, 2, 8),
            new Parameter("Source", Source)
        };
    }

    public override void SetParameters(List<Parameter> parameters)
    {
        int machines = (int)Utility.GetNumeric(parameters, "Machines");
        CellStates = (int)Utility.GetNumeric(parameters, "CellStates");
        Source = Utility.GetString(parameters, "Source");

        Delta = CellStates > 0 ? (256 / (CellStates)) : 0;

        GradientPalette();

        GenerateMachines(machines);
    }
}
