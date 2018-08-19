using Gdk;
using System.Collections.Generic;

public static class World
{
    public static void AddLifeColony(List<Colony> Colonies, int width, int height, int x, int y, double Density, string Birth, string Survival, Color color, List<Cell> Neighborhood, bool Cyclic = false)
    {
        var LifeColony = new Life(width, height, color);
        var maxDensity = (int)(width * height * Density);

        LifeColony.Randomize(maxDensity);
        LifeColony.SetNeighborhood(Neighborhood);
        LifeColony.SetCyclic(Cyclic);
        LifeColony.SetParameters(Birth, Survival);
        LifeColony.AddRules();

        Colonies.Add(new Colony(x, y, LifeColony));
    }

    public static void AddZhabotinskyColony(List<Colony> Colonies, int width, int height, int x, int y, double Density, double k1, double k2, double g, Color color, List<Cell> Neighborhood, bool Cyclic = false, bool Gradient = false)
    {
        var ZhabotinskyColony = new Zhabotinsky(width, height, color);
        var maxDensity = (int)(width * height * Density);

        if (Gradient)
        {
            ZhabotinskyColony.GradientPalette();
        }

        ZhabotinskyColony.SetParameters(k1, k2, g);
        ZhabotinskyColony.Randomize(maxDensity);
        ZhabotinskyColony.SetNeighborhood(Neighborhood);
        ZhabotinskyColony.SetCyclic(Cyclic);

        Colonies.Add(new Colony(x, y, ZhabotinskyColony));
    }

    public static void AddLangtonAntColony(List<Colony> Colonies, int width, int height, int x, int y, int ants, string rules, Color color, bool Cyclic = false, bool Gradient = false)
    {
        var LangtonAntColony = new LangtonAnt(width, height, color);

        LangtonAntColony.SetRules(rules);

        if (Gradient)
        {
            LangtonAntColony.GradientPalette();
        }

        LangtonAntColony.Randomize(ants, rules, !Gradient);
        LangtonAntColony.SetCyclic(Cyclic);

        Colonies.Add(new Colony(x, y, LangtonAntColony));
    }

    public static void AddYinYangFireColony(List<Colony> Colonies, int width, int height, int x, int y, double Density, int maxStates, Color color, List<Cell> Neighborhood, bool Cyclic = false, bool Gradient = false)
    {
        var YinYangFireColony = new YinYangFire(width, height, color);
        var maxDensity = (int)(width * height * Density);

        if (Gradient)
        {
            YinYangFireColony.GradientPalette();
        }

        YinYangFireColony.Randomize(maxDensity, maxStates);
        YinYangFireColony.SetNeighborhood(Neighborhood);
        YinYangFireColony.SetCyclic(Cyclic);

        Colonies.Add(new Colony(x, y, YinYangFireColony));
    }

    public static void AddForestFireColony(List<Colony> Colonies, int width, int height, int x, int y, double Density, double F, double P, Color color, List<Cell> Neighborhood, bool Cyclic = false)
    {
        var ForestFireColony = new ForestFire(width, height, color);
        var maxDensity = (int)(width * height * Density);

        ForestFireColony.SetParameters(F, P);
        ForestFireColony.Randomize(maxDensity);
        ForestFireColony.SetNeighborhood(Neighborhood);
        ForestFireColony.SetCyclic(Cyclic);

        Colonies.Add(new Colony(x, y, ForestFireColony));
    }

    public static void AddIceColony(List<Colony> Colonies, int width, int height, int x, int y, double Density, double Freeze, Color color, List<Cell> Neighborhood, bool Cyclic = false)
    {
        var IceColony = new Ice(width, height, color);
        var maxDensity = (int)(width * height * Density);

        IceColony.SetParameters(Freeze);
        IceColony.Randomize(maxDensity);
        IceColony.SetNeighborhood(Neighborhood);
        IceColony.SetCyclic(Cyclic);

        Colonies.Add(new Colony(x, y, IceColony));
    }

    public static void AddElementaryCA(List<Colony> Colonies, int width, int height, int x, int y, int rule, Color color, bool Cyclic = false)
    {
        var ElementaryCAColony = new ElementaryCA(width, height, color);

        ElementaryCAColony.SetCyclic(Cyclic);
        ElementaryCAColony.SetRule(rule);

        Colonies.Add(new Colony(x, y, ElementaryCAColony));
    }

    public static void AddSnowflakeColony(List<Colony> Colonies, int width, int height, int x, int y, int MaxStates, string Growth, Color color, List<Cell> Neighborhood, bool Cyclic = false, bool Gradient = false)
    {
        var SnowflakeColony = new Snowflake(width, height, color);

        if (Gradient)
        {
            SnowflakeColony.GradientPalette();
        }

        SnowflakeColony.SetNeighborhood(Neighborhood);
        SnowflakeColony.SetCyclic(Cyclic);
        SnowflakeColony.SetParameters(Growth, MaxStates);
        SnowflakeColony.AddRules();

        Colonies.Add(new Colony(x, y, SnowflakeColony));
    }

    public static void AddCyclicColony(List<Colony> Colonies, int width, int height, int x, int y, int maxStates, Color color, List<Cell> Neighborhood, bool Cyclic = false, bool Gradient = false)
    {
        var CyclicColony = new Cyclic(width, height, color);

        if (Gradient)
        {
            CyclicColony.GradientPalette();
        }

        CyclicColony.Randomize(maxStates);
        CyclicColony.SetNeighborhood(Neighborhood);
        CyclicColony.SetCyclic(Cyclic);

        Colonies.Add(new Colony(x, y, CyclicColony));
    }
}
