using Gdk;
using System.Collections.Generic;

public static class World
{
    public static void AddLifeColony(List<Colony> Colonies, int width, int height, int x, int y, double Density, Color color)
    {
        var LifeColony = new Life(width, height, color);
        var maxDensity = (int)(width * height * Density);

        LifeColony.Randomize(maxDensity);

        LifeColony.AddNeighbor(new Cell(-1, -1));
        LifeColony.AddNeighbor(new Cell(0, -1));
        LifeColony.AddNeighbor(new Cell(1, -1));
        LifeColony.AddNeighbor(new Cell(-1, 0));
        LifeColony.AddNeighbor(new Cell(1, 0));
        LifeColony.AddNeighbor(new Cell(-1, 1));
        LifeColony.AddNeighbor(new Cell(0, 1));
        LifeColony.AddNeighbor(new Cell(1, 1));

        LifeColony.AddBirthRule(3);
        LifeColony.AddSurvivalRule(2);
        LifeColony.AddSurvivalRule(3);

        Colonies.Add(new Colony(x, y, LifeColony));
    }

    public static void AddZhabotinskyColony(List<Colony> Colonies, int width, int height, int x, int y, double Density, double k1, double k2, double g, Color color)
    {
        var ZhabotinskyColony = new Zhabotinsky(width, height, color);
        var maxDensity = (int)(width * height * Density);

        ZhabotinskyColony.Randomize(maxDensity);

        ZhabotinskyColony.AddNeighbor(new Cell(-1, -1));
        ZhabotinskyColony.AddNeighbor(new Cell(0, -1));
        ZhabotinskyColony.AddNeighbor(new Cell(1, -1));
        ZhabotinskyColony.AddNeighbor(new Cell(-1, 0));
        ZhabotinskyColony.AddNeighbor(new Cell(1, 0));
        ZhabotinskyColony.AddNeighbor(new Cell(-1, 1));
        ZhabotinskyColony.AddNeighbor(new Cell(0, 1));
        ZhabotinskyColony.AddNeighbor(new Cell(1, 1));

        ZhabotinskyColony.SetParameters(k1, k2, g);

        Colonies.Add(new Colony(x, y, ZhabotinskyColony));
    }

    public static void AddLangtonAntColony(List<Colony> Colonies, int width, int height, int x, int y, int ants, string rules, Color color)
    {
        var LangtonAnt = new LangtonAnt(width, height, color);

        LangtonAnt.Randomize(ants, rules);

        Colonies.Add(new Colony(x, y, LangtonAnt));
    }

    public static void AddYinYangFireColony(List<Colony> Colonies, int width, int height, int x, int y, double Density, int maxStates, Color color)
    {
        var YinYangFireColony = new YinYangFire(width, height, color);
        var maxDensity = (int)(width * height * Density);

        YinYangFireColony.Randomize(maxDensity, maxStates);

        YinYangFireColony.AddNeighbor(new Cell(-1, -1));
        YinYangFireColony.AddNeighbor(new Cell(0, -1));
        YinYangFireColony.AddNeighbor(new Cell(1, -1));
        YinYangFireColony.AddNeighbor(new Cell(-1, 0));
        YinYangFireColony.AddNeighbor(new Cell(1, 0));
        YinYangFireColony.AddNeighbor(new Cell(-1, 1));
        YinYangFireColony.AddNeighbor(new Cell(0, 1));
        YinYangFireColony.AddNeighbor(new Cell(1, 1));

        Colonies.Add(new Colony(x, y, YinYangFireColony));
    }
}
