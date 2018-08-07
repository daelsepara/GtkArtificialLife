using Gdk;
using System.Collections.Generic;

public static class Tests
{
    public static void AddLifeColony(List<Colony> Colonies, int width, int height, int x, int y, double Density, Color color)
	{
		var LifeColony = new Life(width, height, color);
		int maxDensity = (int)(width * height * Density);

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

    public static void LifeTest(List<Colony> Colonies)
	{
		AddLifeColony(Colonies, 256, 256, 0, 0, 0.40, new Color(255, 0, 0));
		AddLifeColony(Colonies, 256, 256, 256, 256, 0.40, new Color(0, 255, 0));
		AddLifeColony(Colonies, 256, 256, 512, 0, 0.40, new Color(0, 0, 255));
		AddLifeColony(Colonies, 256, 256, 256, 0, 0.40, new Color(255, 0, 255));
		AddLifeColony(Colonies, 256, 256, 0, 256, 0.40, new Color(255, 255, 0));
		AddLifeColony(Colonies, 256, 256, 512, 256, 0.40, new Color(0, 255, 255));
	}

	public static void AddZhabotinskyColony(List<Colony> Colonies, int width, int height, int x, int y, double Density, double k1, double k2, double g, Color color)
	{
		var ZhabotinskyColony = new Zhabotinsky(width, height, color);
		int maxDensity = (int)(width * height * Density);

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

    public static void ZhabotinskyTest(List<Colony> Colonies)
	{
		AddZhabotinskyColony(Colonies, 256, 256, 0, 0, 0.40, 1, 1, 10, new Color(255, 0, 0));
		AddZhabotinskyColony(Colonies, 256, 256, 256, 256, 0.40, 1, 1, 10, new Color(0, 255, 0));
		AddZhabotinskyColony(Colonies, 256, 256, 512, 0, 0.40, 1, 1, 10, new Color(0, 0, 255));
		AddZhabotinskyColony(Colonies, 256, 256, 256, 0, 0.40, 1, 1, 10, new Color(255, 0, 255));
		AddZhabotinskyColony(Colonies, 256, 256, 0, 256, 0.40, 1, 1, 10, new Color(255, 255, 0));
		AddZhabotinskyColony(Colonies, 256, 256, 512, 256, 0.40, 1, 1, 10, new Color(0, 255, 255));
	}

	public static void AddLangtonAntColony(List<Colony> Colonies, int width, int height, int x, int y, int ants, string rules, Color color)
	{
		var LangtonAnt = new LangtonAnt(width, height, color);

		LangtonAnt.Randomize(ants, rules);

		Colonies.Add(new Colony(x, y, LangtonAnt));
	}

	public static void LangtonAntTest(List<Colony> Colonies)
	{
        AddLangtonAntColony(Colonies, 256, 256, 0, 0, 40, "LR", new Color(255, 0, 0));
		AddLangtonAntColony(Colonies, 256, 256, 256, 256, 40, "LRL", new Color(0, 255, 0));
		AddLangtonAntColony(Colonies, 256, 256, 512, 0, 40, "RRLLLLRRRLLL", new Color(0, 0, 255));
		AddLangtonAntColony(Colonies, 256, 256, 256, 0, 40, "RRLRLRLLRL", new Color(255, 0, 255));
		AddLangtonAntColony(Colonies, 256, 256, 0, 256, 40, "RRRLLLRLLLRR", new Color(255, 255, 0));
		AddLangtonAntColony(Colonies, 256, 256, 512, 256, 40, "RRRLR", new Color(0, 255, 255));
	}

	public static void AddYinYangFireColony(List<Colony> Colonies, int width, int height, int x, int y, double Density, int maxStates, Color color)
	{
        var YinYangFireColony = new YinYangFire(width, height, color);
		int maxDensity = (int)(width * height * Density);

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

	public static void YinYangFireTest(List<Colony> Colonies)
	{
		AddYinYangFireColony(Colonies, 256, 256, 0, 0, 0.40, 64, new Color(255, 0, 0));
		AddYinYangFireColony(Colonies, 256, 256, 256, 256, 0.40, 128, new Color(0, 255, 0));
		AddYinYangFireColony(Colonies, 256, 256, 512, 0, 0.40, 256, new Color(0, 0, 255));
		AddYinYangFireColony(Colonies, 256, 256, 256, 0, 0.40, 256, new Color(255, 0, 255));
		AddYinYangFireColony(Colonies, 256, 256, 0, 256, 0.40, 128, new Color(255, 255, 0));
		AddYinYangFireColony(Colonies, 256, 256, 512, 256, 0.40, 64, new Color(0, 255, 255));
	}

	public static void LifeZhabotinskyTest(List<Colony> Colonies)
	{
		AddLifeColony(Colonies, 256, 256, 0, 0, 0.40, new Color(255, 0, 0));
		AddLifeColony(Colonies, 256, 256, 256, 256, 0.40, new Color(0, 255, 0));
		AddLifeColony(Colonies, 256, 256, 512, 0, 0.40, new Color(0, 0, 255));
		AddZhabotinskyColony(Colonies, 256, 256, 256, 0, 0.40, 1, 1, 10, new Color(255, 0, 255));
		AddZhabotinskyColony(Colonies, 256, 256, 0, 256, 0.40, 1, 1, 10, new Color(255, 255, 0));
		AddZhabotinskyColony(Colonies, 256, 256, 512, 256, 0.40, 1, 1, 10, new Color(0, 255, 255));
	}

}
