using Gdk;
using System.Collections.Generic;

public static class Tests
{
    public static void LifeTest(List<Colony> Colonies)
    {
        var w = 256;
        var h = 56;

        World.AddLifeColony(Colonies, w, h, 0, 0, 0.40, new Color(255, 0, 0));
        World.AddLifeColony(Colonies, w, h, 256, 256, 0.40, new Color(0, 255, 0));
        World.AddLifeColony(Colonies, w, h, 512, 0, 0.40, new Color(0, 0, 255));
        World.AddLifeColony(Colonies, w, h, 256, 0, 0.40, new Color(255, 0, 255));
        World.AddLifeColony(Colonies, w, h, 0, 256, 0.40, new Color(255, 255, 0));
        World.AddLifeColony(Colonies, w, h, 512, 256, 0.40, new Color(0, 255, 255));
    }

    public static void ZhabotinskyTest(List<Colony> Colonies)
    {
        var w = 256;
        var h = 56;

        World.AddZhabotinskyColony(Colonies, w, h, 0, 0, 0.40, 1, 1, 10, new Color(255, 0, 0));
        World.AddZhabotinskyColony(Colonies, w, h, 256, 256, 0.40, 1, 1, 10, new Color(0, 255, 0));
        World.AddZhabotinskyColony(Colonies, w, h, 512, 0, 0.40, 1, 1, 10, new Color(0, 0, 255));
        World.AddZhabotinskyColony(Colonies, w, h, 256, 0, 0.40, 1, 1, 10, new Color(255, 0, 255));
        World.AddZhabotinskyColony(Colonies, w, h, 0, 256, 0.40, 1, 1, 10, new Color(255, 255, 0));
        World.AddZhabotinskyColony(Colonies, w, h, 512, 256, 0.40, 1, 1, 10, new Color(0, 255, 255));
    }


    public static void LangtonAntTest(List<Colony> Colonies)
    {
        var w = 256;
        var h = 56;

        World.AddLangtonAntColony(Colonies, w, h, 0, 0, 40, "LR", new Color(255, 0, 0));
        World.AddLangtonAntColony(Colonies, w, h, 256, 256, 40, "LRL", new Color(0, 255, 0));
        World.AddLangtonAntColony(Colonies, w, h, 512, 0, 40, "RRLLLLRRRLLL", new Color(0, 0, 255));
        World.AddLangtonAntColony(Colonies, w, h, 256, 0, 40, "RRLRLRLLRL", new Color(255, 0, 255));
        World.AddLangtonAntColony(Colonies, w, h, 0, 256, 40, "RRRLLLRLLLRR", new Color(255, 255, 0));
        World.AddLangtonAntColony(Colonies, w, h, 512, 256, 40, "RRRLR", new Color(0, 255, 255));
    }

    public static void YinYangFireTest(List<Colony> Colonies)
    {
        var w = 256;
        var h = 56;

        World.AddYinYangFireColony(Colonies, w, h, 0, 0, 0.40, 64, new Color(255, 0, 0));
        World.AddYinYangFireColony(Colonies, w, h, 256, 256, 0.40, 128, new Color(0, 255, 0));
        World.AddYinYangFireColony(Colonies, w, h, 512, 0, 0.40, 256, new Color(0, 0, 255));
        World.AddYinYangFireColony(Colonies, w, h, 256, 0, 0.40, 256, new Color(255, 0, 255));
        World.AddYinYangFireColony(Colonies, w, w, 0, 256, 0.40, 128, new Color(255, 255, 0));
        World.AddYinYangFireColony(Colonies, w, h, 512, 256, 0.40, 64, new Color(0, 255, 255));
    }

    public static void LifeZhabotinskyTest(List<Colony> Colonies)
    {
        var w = 256;
        var h = 56;

        World.AddLifeColony(Colonies, w, h, 0, 0, 0.40, new Color(255, 0, 0));
        World.AddLifeColony(Colonies, w, h, 256, 256, 0.40, new Color(0, 255, 0));
        World.AddLifeColony(Colonies, w, h, 512, 0, 0.40, new Color(0, 0, 255));
        World.AddZhabotinskyColony(Colonies, w, h, 256, 0, 0.40, 1, 1, 10, new Color(255, 0, 255));
        World.AddZhabotinskyColony(Colonies, w, h, 0, 256, 0.40, 1, 1, 10, new Color(255, 255, 0));
        World.AddZhabotinskyColony(Colonies, w, h, 512, 256, 0.40, 1, 1, 10, new Color(0, 255, 255));
    }
}
