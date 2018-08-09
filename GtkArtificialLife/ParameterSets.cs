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
}
