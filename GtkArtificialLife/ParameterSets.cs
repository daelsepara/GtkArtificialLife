using System.Collections.Generic;

public static class ParameterSets
{
    // Default values for parameter sets

    public static List<Parameter> Life()
    {
        return new List<Parameter>
        {
            new Parameter("Density", 0.4, 0.01, 1.0),
            new Parameter("Birth", "3"),
            new Parameter("Survival", "2,3")
        };
    }

    public static List<Parameter> Zhabotinsky()
    {
        return new List<Parameter>
        {
            new Parameter("Density", 0.4, 0.01, 1.0),
            new Parameter("g", 10, 1, 100),
            new Parameter("k1", 1, 1, 100),
            new Parameter("k2", 1, 1, 100)
        };
    }

    public static List<Parameter> LangtonAnt()
    {
        return new List<Parameter>
        {
            new Parameter("Ants", 40, 1, 1000),
            new Parameter("Rule", "RRLLLLRRRLLL")
        };
    }

    public static List<Parameter> YinYangFire()
    {
        return new List<Parameter>
        {
            new Parameter("Density", 0.4, 0.01, 1.0),
            new Parameter("MaxStates", 64, 2, 256)
        };
    }

    public static List<Parameter> ForestFire()
    {
        return new List<Parameter>
        {
            new Parameter("Density", 0.4, 0.01, 1.0),
            new Parameter("P", 1000, 1, 1000),
            new Parameter("F", 100, 1, 1000)
        };
    }

    public static List<Parameter> ElementaryCA()
    {
        return new List<Parameter>
        {
            new Parameter("Rule", "30")
        };
    }

    public static List<Parameter> Snowflake()
    {
        return new List<Parameter>
        {
            new Parameter("Growth", "1,3,6"),
            new Parameter("MaxStates", 12, 1, 256)
        };
    }

    public static List<Parameter> Ice()
    {
        return new List<Parameter>
        {
            new Parameter("Density", 0.01, 0.01, 1.0),
            new Parameter("Freeze", 30, 1, 1000)
        };
    }

    public static List<Parameter> Cyclic()
    {
        return new List<Parameter>
        {
            new Parameter("MaxStates", 8, 2, 256)
        };
    }

    public static List<Parameter> TuringMachine()
    {
        return new List<Parameter>
        {
            new Parameter("Machines", 32, 1, 10000),
            new Parameter("CellStates", 2, 2, 8),
            new Parameter("Source", "1BX2R,1XB2R,2BX1S,2XX2S")
        };
    }
}
