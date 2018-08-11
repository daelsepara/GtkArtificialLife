using System.Collections.Generic;

public static class ParameterSets
{
    // Default values for parameter sets

    public static List<Parameter> Life()
    {
        var set = new List<Parameter>
        {
            new Parameter("Density", 0.4, 0.01, 1.0)
        };

        return set;
    }

    public static List<Parameter> Zhabotinsky()
    {
        var set = new List<Parameter>
        {
            new Parameter("Density", 0.4, 0.01, 1.0),
            new Parameter("g", 10, 1, 100),
            new Parameter("k1", 1, 1, 100),
            new Parameter("k2", 1, 1, 100)
        };

        return set;
    }

    public static List<Parameter> LangtonAnt()
    {
        var set = new List<Parameter>
        {
            new Parameter("Ants", 40, 1, 1000),
            new Parameter("Rule", "RRLLLLRRRLLL")
        };

        return set;
    }

    public static List<Parameter> YinYangFire()
    {
        var set = new List<Parameter>
        {
            new Parameter("Density", 0.4, 0.01, 1.0),
            new Parameter("MaxStates", 64, 2, 256)
        };

        return set;
    }

    public static List<Parameter> ForestFire()
    {
        var set = new List<Parameter>
        {
            new Parameter("Density", 0.4, 0.01, 1.0),
            new Parameter("P", 1000, 1, 1000),
            new Parameter("F", 100, 1, 1000)
        };

        return set;
    }

    public static List<Parameter> ElementaryCA()
    {
        var set = new List<Parameter>
        {
            new Parameter("Rule", 22, 0, 255),
        };

        return set;
    }

    public static List<Parameter> Snowflake()
    {
        return new List<Parameter>();
    }

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
        AddNeighbor(neighborhood, new Cell(-1, 0));
        AddNeighbor(neighborhood, new Cell(1, 0));
        AddNeighbor(neighborhood, new Cell(-1, 1));
        AddNeighbor(neighborhood, new Cell(0, 1));
        AddNeighbor(neighborhood, new Cell(1, 1));

        return neighborhood;
    }

    // 6 Neighbor approximation of the hexagonal lattice
    public static List<Cell>  HexNeighborhood()
    {
        var neighborhood = new List<Cell>();

        AddNeighbor(neighborhood, new Cell(-1, -1));
        AddNeighbor(neighborhood, new Cell(0, -1));
        AddNeighbor(neighborhood, new Cell(-1, 0));
        AddNeighbor(neighborhood, new Cell(1, 0));
        AddNeighbor(neighborhood, new Cell(0, 1));
        AddNeighbor(neighborhood, new Cell(1, 1));

        return neighborhood;
    }
}
