using Gdk;
using System.Collections.Generic;

public static class World
{
    public static void AddNeighbor(List<Cell> Neighborhood, Cell neighbor)
    {
        if (!Neighborhood.Contains(neighbor))
        {
            Neighborhood.Add(neighbor);
        }
    }

    public static List<Cell> EmptyNeighborhood()
    {
        return new List<Cell>();
    }

    public static List<Cell> MooreNeighborhood()
    {
        var neighborhood = new List<Cell>();

        AddNeighbor(neighborhood, new Cell(-1, -1));
        AddNeighbor(neighborhood, new Cell(0, -1));
        AddNeighbor(neighborhood, new Cell(1, -1));
        AddNeighbor(neighborhood, new Cell(1, 0));
        AddNeighbor(neighborhood, new Cell(1, 1));
        AddNeighbor(neighborhood, new Cell(0, 1));
        AddNeighbor(neighborhood, new Cell(-1, 1));
        AddNeighbor(neighborhood, new Cell(-1, 0));

        return neighborhood;
    }

    // 6 Neighbor approximation of the hexagonal lattice
    public static List<Cell> HexNeighborhood()
    {
        var neighborhood = new List<Cell>();

        AddNeighbor(neighborhood, new Cell(-1, -1));
        AddNeighbor(neighborhood, new Cell(0, -1));
        AddNeighbor(neighborhood, new Cell(1, 0));
        AddNeighbor(neighborhood, new Cell(1, 1));
        AddNeighbor(neighborhood, new Cell(0, 1));
        AddNeighbor(neighborhood, new Cell(-1, 0));

        return neighborhood;
    }

    // 4 Neighbor Von Neumann
    public static List<Cell> VonNeumannNeighborhood()
    {
        var neighborhood = new List<Cell>();

        AddNeighbor(neighborhood, new Cell(0, -1));
        AddNeighbor(neighborhood, new Cell(1, 0));
        AddNeighbor(neighborhood, new Cell(0, 1));
        AddNeighbor(neighborhood, new Cell(-1, 0));

        return neighborhood;
    }

    public static int Cyclic(int location, int offset, int limit)
    {
        return offset >= 0 ? (location + offset) % limit : (location + limit + offset) % limit;
    }

    public static void AddLifeColony(List<Colony> Colonies, int width, int height, int x, int y, double Density, string Birth, string Survival, Color color, List<Cell> Neighborhood, bool Cyclic = false)
    {
        var LifeColony = new Life(width, height, color);

        LifeColony.SetParameters(new List<Parameter>
        {
            new Parameter("Birth", Birth),
            new Parameter("Survival", Survival),
            new Parameter("Density", Density, 0.01, 1.0)
        });

        LifeColony.Randomize();
        LifeColony.SetNeighborhood(Neighborhood);
        LifeColony.SetCyclic(Cyclic);

        Colonies.Add(new Colony(x, y, LifeColony));
    }

    public static void AddZhabotinskyColony(List<Colony> Colonies, int width, int height, int x, int y, double Density, double k1, double k2, double g, Color color, List<Cell> Neighborhood, bool Cyclic = false, bool Gradient = false)
    {
        var ZhabotinskyColony = new Zhabotinsky(width, height, color);
        var maxDensity = (int)(width * height * Density);

        ZhabotinskyColony.SetParameters(new List<Parameter>{
            new Parameter("g", g, 1, 100),
            new Parameter("k1", k1, 1, 100),
            new Parameter("k2", k2, 1, 100),
            new Parameter("Density", Density, 0.01, 1.0)
        });

        if (Gradient)
        {
            ZhabotinskyColony.GradientPalette();
        }

        ZhabotinskyColony.Randomize();
        ZhabotinskyColony.SetNeighborhood(Neighborhood);
        ZhabotinskyColony.SetCyclic(Cyclic);

        Colonies.Add(new Colony(x, y, ZhabotinskyColony));
    }

    public static void AddLangtonAntColony(List<Colony> Colonies, int width, int height, int x, int y, int ants, string rules, Color color, bool Cyclic = false, bool Gradient = false)
    {
        var LangtonAntColony = new LangtonAnt(width, height, color);

        LangtonAntColony.Random(!Gradient);

        LangtonAntColony.SetParameters(new List<Parameter>
        {
            new Parameter("Ants", ants, 1, 1000),
            new Parameter("Rule", rules)
        });

        LangtonAntColony.SetCyclic(Cyclic);

        Colonies.Add(new Colony(x, y, LangtonAntColony));
    }

    public static void AddYinYangFireColony(List<Colony> Colonies, int width, int height, int x, int y, double Density, int maxStates, Color color, List<Cell> Neighborhood, bool Cyclic = false, bool Gradient = false)
    {
        var YinYangFireColony = new YinYangFire(width, height, color);

        YinYangFireColony.SetParameters(new List<Parameter>{
            new Parameter("MaxStates", maxStates, 2, 256),
            new Parameter("Density", Density, 0.01, 1.0)
        });

        if (Gradient)
        {
            YinYangFireColony.GradientPalette();
        }

        YinYangFireColony.Randomize();
        YinYangFireColony.SetNeighborhood(Neighborhood);
        YinYangFireColony.SetCyclic(Cyclic);

        Colonies.Add(new Colony(x, y, YinYangFireColony));
    }

    public static void AddForestFireColony(List<Colony> Colonies, int width, int height, int x, int y, double Density, double F, double P, Color color, List<Cell> Neighborhood, bool Cyclic = false)
    {
        var ForestFireColony = new ForestFire(width, height, color);

        ForestFireColony.SetParameters(new List<Parameter>
        {
            new Parameter("P", P, 1, 1000),
            new Parameter("F", F, 1, 1000),
            new Parameter("Density", Density, 0.01, 1.0)
        });

        ForestFireColony.Randomize();
        ForestFireColony.SetNeighborhood(Neighborhood);
        ForestFireColony.SetCyclic(Cyclic);

        Colonies.Add(new Colony(x, y, ForestFireColony));
    }

    public static void AddIceColony(List<Colony> Colonies, int width, int height, int x, int y, double Density, double Freeze, Color color, List<Cell> Neighborhood, bool Cyclic = false)
    {
        var IceColony = new Ice(width, height, color);
        var maxDensity = (int)(width * height * Density);

        IceColony.SetParameters(new List<Parameter>
        {
            new Parameter("Freeze", Freeze, 1, 1000),
            new Parameter("Density", Density, 0.01, 1.0)
        });

        IceColony.Randomize();
        IceColony.SetNeighborhood(Neighborhood);
        IceColony.SetCyclic(Cyclic);

        Colonies.Add(new Colony(x, y, IceColony));
    }

    public static void AddElementaryCA(List<Colony> Colonies, int width, int height, int x, int y, string rule, Color color, bool Cyclic = false)
    {
        var ElementaryCAColony = new ElementaryCA(width, height, color);

        ElementaryCAColony.SetCyclic(Cyclic);

        ElementaryCAColony.SetParameters(new List<Parameter>
        {
            new Parameter("Rule", rule)
        });

        Colonies.Add(new Colony(x, y, ElementaryCAColony));
    }

    public static void AddSnowflakeColony(List<Colony> Colonies, int width, int height, int x, int y, int MaxStates, string Growth, Color color, List<Cell> Neighborhood, bool Cyclic = false, bool Gradient = false)
    {
        var SnowflakeColony = new Snowflake(width, height, color);

        SnowflakeColony.SetParameters(new List<Parameter>
        {
            new Parameter("Growth", Growth),
            new Parameter("MaxStates", MaxStates, 1, 256)
        });

        SnowflakeColony.SetNeighborhood(Neighborhood);
        SnowflakeColony.SetCyclic(Cyclic);

        if (Gradient)
        {
            SnowflakeColony.GradientPalette();
        }

        Colonies.Add(new Colony(x, y, SnowflakeColony));
    }

    public static void AddCyclicColony(List<Colony> Colonies, int width, int height, int x, int y, int maxStates, Color color, List<Cell> Neighborhood, bool Cyclic = false, bool Gradient = false)
    {
        var CyclicColony = new Cyclic(width, height, color);

        CyclicColony.SetParameters(new List<Parameter>{
            new Parameter("MaxStates", maxStates, 2, 256)
        });

        if (Gradient)
        {
            CyclicColony.GradientPalette();
        }

        CyclicColony.Randomize();
        CyclicColony.SetNeighborhood(Neighborhood);
        CyclicColony.SetCyclic(Cyclic);

        Colonies.Add(new Colony(x, y, CyclicColony));
    }

    public static void AddTuringMachineColony(List<Colony> Colonies, int width, int height, int x, int y, int Machines, string Source, int CellStates, Color color, List<Cell> Neighborhood, bool Cyclic = false, bool Gradient = false)
    {
        var TuringMachineColony = new TuringMachine(width, height, color);

        TuringMachineColony.SetNeighborhood(Neighborhood);
        TuringMachineColony.SetCyclic(Cyclic);

        TuringMachineColony.SetParameters(new List<Parameter>{
            new Parameter("Machines", Machines, 1, 1000),
            new Parameter("CellStates", CellStates, 2, 8),
            new Parameter("Source", Source)
        });

        if (Gradient)
        {
            TuringMachineColony.GradientPalette();
        }

        Colonies.Add(new Colony(x, y, TuringMachineColony));
    }
}
