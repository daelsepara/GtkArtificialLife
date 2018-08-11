using Gdk;
using System.Collections.Generic;

public static class World
{
    public static void AddLifeColony(List<Colony> Colonies, int width, int height, int x, int y, double Density, Color color)
    {
        var LifeColony = new Life(width, height, color);
        var maxDensity = (int)(width * height * Density);

        LifeColony.Randomize(maxDensity);

        Colonies.Add(new Colony(x, y, LifeColony));
    }

    public static void AddZhabotinskyColony(List<Colony> Colonies, int width, int height, int x, int y, double Density, double k1, double k2, double g, Color color, bool Gradient = false)
    {
        var ZhabotinskyColony = new Zhabotinsky(width, height, color);
        var maxDensity = (int)(width * height * Density);

        if (Gradient)
        {
            ZhabotinskyColony.GradientPalette();
        }

        ZhabotinskyColony.SetParameters(k1, k2, g);
        ZhabotinskyColony.Randomize(maxDensity);

        Colonies.Add(new Colony(x, y, ZhabotinskyColony));
    }

    public static void AddLangtonAntColony(List<Colony> Colonies, int width, int height, int x, int y, int ants, string rules, Color color, bool Gradient = false)
    {
        var LangtonAnt = new LangtonAnt(width, height, color);

        LangtonAnt.SetRules(rules);

        if (Gradient)
        {
            LangtonAnt.GradientPalette();
        }

        LangtonAnt.Randomize(ants, rules, !Gradient);

        Colonies.Add(new Colony(x, y, LangtonAnt));
    }

    public static void AddYinYangFireColony(List<Colony> Colonies, int width, int height, int x, int y, double Density, int maxStates, Color color, bool Gradient = false)
    {
        var YinYangFireColony = new YinYangFire(width, height, color);
        var maxDensity = (int)(width * height * Density);

        if (Gradient)
        {
            YinYangFireColony.GradientPalette();
        }

        YinYangFireColony.Randomize(maxDensity, maxStates);

        Colonies.Add(new Colony(x, y, YinYangFireColony));
    }

    public static void AddForestFireColony(List<Colony> Colonies, int width, int height, int x, int y, double Density, double F, double P, Color color)
    {
        var ForestFireColony = new ForestFire(width, height, color);
        var maxDensity = (int)(width * height * Density);

        ForestFireColony.SetParameters(F, P);
        ForestFireColony.Randomize(maxDensity);

        Colonies.Add(new Colony(x, y, ForestFireColony));
    }

    public static void AddElementaryCA(List<Colony> Colonies, int width, int height, int x, int y, int rule, Color color)
    {
        var ElementaryCAColony = new ElementaryCA(width, height, color);

        ElementaryCAColony.SetRule(rule);

        Colonies.Add(new Colony(x, y, ElementaryCAColony));
    }

    public static void AddSnowflakeColony(List<Colony> Colonies, int width, int height, int x, int y, Color color, bool Gradient = false)
    {
        var SnowflakeColony = new Snowflake(width, height, color);

        if (Gradient)
        {
            SnowflakeColony.GradientPalette();
        }

        Colonies.Add(new Colony(x, y, SnowflakeColony));
    }
}
