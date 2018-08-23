using Gdk;
using System.Collections.Generic;

public static class Tests
{
    public static void LifeTest(List<Colony> Colonies)
    {
        Colonies.Clear();

        var w = 256;
        var h = 256;

        var neighborhood = World.MooreNeighborhood();

        var Birth = "3";
        var Survival = "2,3";

        World.AddLifeColony(Colonies, w, h, 0, 0, 0.40, Birth, Survival, new Color(255, 0, 0), neighborhood);
        World.AddLifeColony(Colonies, w, h, 256, 256, 0.40, Birth, Survival, new Color(0, 255, 0), neighborhood);
        World.AddLifeColony(Colonies, w, h, 512, 0, 0.40, Birth, Survival, new Color(0, 0, 255), neighborhood);
        World.AddLifeColony(Colonies, w, h, 256, 0, 0.40, Birth, Survival, new Color(255, 0, 255), neighborhood);
        World.AddLifeColony(Colonies, w, h, 0, 256, 0.40, Birth, Survival, new Color(255, 255, 0), neighborhood);
        World.AddLifeColony(Colonies, w, h, 512, 256, 0.40, Birth, Survival, new Color(0, 255, 255), neighborhood);

        AddSelection(w, h);
    }

    public static void ZhabotinskyTest(List<Colony> Colonies)
    {
        Colonies.Clear();

        var w = 256;
        var h = 256;

        var neighborhood = World.MooreNeighborhood();

        World.AddZhabotinskyColony(Colonies, w, h, 0, 0, 0.40, 1, 1, 10, new Color(255, 0, 0), neighborhood);
        World.AddZhabotinskyColony(Colonies, w, h, 256, 256, 0.40, 1, 1, 10, new Color(0, 255, 0), neighborhood);
        World.AddZhabotinskyColony(Colonies, w, h, 512, 0, 0.40, 1, 1, 10, new Color(0, 0, 255), neighborhood);
        World.AddZhabotinskyColony(Colonies, w, h, 256, 0, 0.40, 1, 1, 10, new Color(255, 0, 255), neighborhood);
        World.AddZhabotinskyColony(Colonies, w, h, 0, 256, 0.40, 1, 1, 10, new Color(255, 255, 0), neighborhood);
        World.AddZhabotinskyColony(Colonies, w, h, 512, 256, 0.40, 1, 1, 10, new Color(0, 255, 255), neighborhood);

        AddSelection(w, h);
    }

    public static void LangtonAntTest(List<Colony> Colonies)
    {
        Colonies.Clear();

        var w = 256;
        var h = 256;

        var neighborhood = World.VonNeumannNeighborhood();

        World.AddLangtonAntColony(Colonies, w, h, 0, 0, 40, "LR", new Color(255, 0, 0), neighborhood);
        World.AddLangtonAntColony(Colonies, w, h, 256, 256, 40, "LRL", new Color(0, 255, 0), neighborhood);
        World.AddLangtonAntColony(Colonies, w, h, 512, 0, 40, "RRLLLLRRRLLL", new Color(0, 0, 255), neighborhood);
        World.AddLangtonAntColony(Colonies, w, h, 256, 0, 40, "RRLRLRLLRL", new Color(255, 0, 255), neighborhood);
        World.AddLangtonAntColony(Colonies, w, h, 0, 256, 40, "RRRLLLRLLLRR", new Color(255, 255, 0), neighborhood);
        World.AddLangtonAntColony(Colonies, w, h, 512, 256, 40, "RRRLR", new Color(0, 255, 255), neighborhood);

        AddSelection(w, h);
    }

    public static void YinYangFireTest(List<Colony> Colonies)
    {
        Colonies.Clear();

        var w = 256;
        var h = 256;

        var neighborhood = World.MooreNeighborhood();

        World.AddYinYangFireColony(Colonies, w, h, 0, 0, 0.40, 64, new Color(255, 0, 0), neighborhood);
        World.AddYinYangFireColony(Colonies, w, h, 256, 256, 0.40, 128, new Color(0, 255, 0), neighborhood);
        World.AddYinYangFireColony(Colonies, w, h, 512, 0, 0.40, 256, new Color(0, 0, 255), neighborhood);
        World.AddYinYangFireColony(Colonies, w, h, 256, 0, 0.40, 256, new Color(255, 0, 255), neighborhood);
        World.AddYinYangFireColony(Colonies, w, w, 0, 256, 0.40, 128, new Color(255, 255, 0), neighborhood);
        World.AddYinYangFireColony(Colonies, w, h, 512, 256, 0.40, 64, new Color(0, 255, 255), neighborhood);

        AddSelection(w, h);
    }

    public static void SnowflakeTest(List<Colony> Colonies)
    {
        Colonies.Clear();

        var w = 256;
        var h = 256;

        var neighborhood = World.HexNeighborhood();

        var Growth = "1,3,6";
        var MaxStates = 12;

        World.AddSnowflakeColony(Colonies, w, h, 0, 0, MaxStates, Growth, new Color(255, 0, 0), neighborhood, true);
        World.AddSnowflakeColony(Colonies, w, h, 256, 256, MaxStates, Growth, new Color(0, 255, 0), neighborhood, true);
        World.AddSnowflakeColony(Colonies, w, h, 512, 0, MaxStates, Growth, new Color(0, 0, 255), neighborhood, true);
        World.AddSnowflakeColony(Colonies, w, h, 256, 0, MaxStates, Growth, new Color(255, 0, 255), neighborhood, true);
        World.AddSnowflakeColony(Colonies, w, w, 0, 256, MaxStates, Growth, new Color(255, 255, 0), neighborhood, true);
        World.AddSnowflakeColony(Colonies, w, h, 512, 256, MaxStates, Growth, new Color(0, 255, 255), neighborhood, true);

        AddSelection(w, h);
    }

    public static void ForestFireTest(List<Colony> Colonies)
    {
        Colonies.Clear();

        var w = 256;
        var h = 256;

        var neighborhood = World.MooreNeighborhood();

        World.AddForestFireColony(Colonies, w, h, 0, 0, 0.4, 100, 1000, new Color(255, 0, 0), neighborhood);
        World.AddForestFireColony(Colonies, w, h, 256, 256, 0.4, 100, 1000, new Color(0, 255, 0), neighborhood);
        World.AddForestFireColony(Colonies, w, h, 512, 0, 0.4, 100, 1000, new Color(0, 0, 255), neighborhood);
        World.AddForestFireColony(Colonies, w, h, 256, 0, 0.4, 100, 1000, new Color(255, 0, 255), neighborhood);
        World.AddForestFireColony(Colonies, w, w, 0, 256, 0.4, 100, 1000, new Color(255, 255, 0), neighborhood);
        World.AddForestFireColony(Colonies, w, h, 512, 256, 0.4, 100, 1000, new Color(0, 255, 255), neighborhood);

        AddSelection(w, h);
    }

    public static void ElementaryCATest(List<Colony> Colonies)
    {
        Colonies.Clear();

        var w = 256;
        var h = 128;

        World.AddElementaryCA(Colonies, w, h, 0, 0, "30", new Color(255, 0, 0));
        World.AddElementaryCA(Colonies, w, h, 256, 256, "110", new Color(0, 255, 0));
        World.AddElementaryCA(Colonies, w, h, 512, 0, "75", new Color(0, 0, 255));
        World.AddElementaryCA(Colonies, w, h, 256, 0, "45", new Color(255, 0, 255));
        World.AddElementaryCA(Colonies, w, h, 0, 256, "18", new Color(255, 255, 0));
        World.AddElementaryCA(Colonies, w, h, 512, 256, "118", new Color(0, 255, 255));

        AddSelection(w, h);
    }

    public static void IceTest(List<Colony> Colonies)
    {
        Colonies.Clear();

        var w = 256;
        var h = 256;

        var neighborhood = World.MooreNeighborhood();

        World.AddIceColony(Colonies, w, h, 0, 0, 0.4, 30, new Color(255, 0, 0), neighborhood);
        World.AddIceColony(Colonies, w, h, 256, 256, 0.4, 30, new Color(0, 255, 0), neighborhood);
        World.AddIceColony(Colonies, w, h, 512, 0, 0.4, 30, new Color(0, 0, 255), neighborhood);
        World.AddIceColony(Colonies, w, h, 256, 0, 0.4, 30, new Color(255, 0, 255), neighborhood);
        World.AddIceColony(Colonies, w, w, 0, 256, 0.4, 30, new Color(255, 255, 0), neighborhood);
        World.AddIceColony(Colonies, w, h, 512, 256, 0.4, 30, new Color(0, 255, 255), neighborhood);

        AddSelection(w, h);
    }

    public static void CyclicTest(List<Colony> Colonies)
    {
        Colonies.Clear();

        var w = 256;
        var h = 256;

        var neighborhood = World.MooreNeighborhood();

        World.AddCyclicColony(Colonies, w, h, 0, 0, 4, new Color(255, 0, 0), neighborhood);
        World.AddCyclicColony(Colonies, w, h, 256, 256, 4, new Color(0, 255, 0), neighborhood);
        World.AddCyclicColony(Colonies, w, h, 512, 0, 4, new Color(0, 0, 255), neighborhood);
        World.AddCyclicColony(Colonies, w, h, 256, 0, 4, new Color(255, 0, 255), neighborhood);
        World.AddCyclicColony(Colonies, w, w, 0, 256, 4, new Color(255, 255, 0), neighborhood);
        World.AddCyclicColony(Colonies, w, h, 512, 256, 4, new Color(0, 255, 255), neighborhood);

        AddSelection(w, h);
    }

    public static void TuringMachineTest(List<Colony> Colonies)
    {
        Colonies.Clear();

        var w = 256;
        var h = 256;

        var neighborhood = World.VonNeumannNeighborhood();

        // see: https://github.com/rm-hull/turmites/blob/master/src/turmites/client/core.cljs#L33
        // see: http://mathworld.wolfram.com/Turmite.html
        // see: https://en.wikipedia.org/wiki/Turmite
        World.AddTuringMachineColony(Colonies, w, h, 0, 0, 100, "1BX2R,1XB2R,2BX1S,2XX2S", 2, new Color(255, 0, 0), neighborhood, true);
        World.AddTuringMachineColony(Colonies, w, h, 256, 256, 100, "1BX1R,1XX2R,2BB1S,2XB2S", 2, new Color(0, 255, 0), neighborhood, true);
        World.AddTuringMachineColony(Colonies, w, h, 512, 0, 100, "1BX2L,1XX2L,2BX2R,2XB1S", 2, new Color(0, 0, 255), neighborhood, true);
        World.AddTuringMachineColony(Colonies, w, h, 256, 0, 100, "1BX2R,1XB2L,2BX1S,2XB1S", 2, new Color(255, 0, 255), neighborhood, true);
        World.AddTuringMachineColony(Colonies, w, h, 0, 256, 100, "1BX2L,1XB2L,2BX2R,2XB1L", 2, new Color(255, 255, 0), neighborhood, true);
        World.AddTuringMachineColony(Colonies, w, h, 512, 256, 100, "1BX2S,1XB2R,2BX1L,2XX1R", 2, new Color(0, 255, 255), neighborhood, true);

        AddSelection(w, h);
    }

    public static void ClearSelection()
    {
        GtkSelection.Selection.Clear();
        GtkSelection.Selected = 0;
    }

    public static void AddBox(int x, int y, int w, int h)
    {
        GtkSelection.Selection.Add(x, y, x + w - 1, y + h - 1);
    }


    public static void AddSelection(int w, int h)
    {
        ClearSelection();

        AddBox(0, 0, w, h);
        AddBox(256, 256, w, h);
        AddBox(512, 0, w, h);
        AddBox(256, 0, w, h);
        AddBox(0, 256, w, h);
        AddBox(512, 256, w, h);
    }
}
