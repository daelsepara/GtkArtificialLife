using Gdk;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

public static class ConvertImage
{
    static Random random = new Random(Guid.NewGuid().GetHashCode());

    public static ArtificialLife Convert(ColonyTypes.Type type, Gtk.Image image, int Width, int Height, List<Parameter> Parameters, Color color, List<Cell> Neighborhood, bool Cyclic = false, bool Gradient = false)
    {
        int population = 0;

        if (image.Pixbuf != null)
        {
            if (type == ColonyTypes.Type.YinYangFire)
            {
                var maxStates = (int)Utility.GetNumeric(Parameters, "MaxStates");

                var colony = new YinYangFire(Width, Height, color);

                colony.SetParameters(new List<Parameter>{
                    new Parameter("MaxStates", maxStates, 2, 256),
                    new Parameter("Density", 1, (double)1 / 100, 1)
                });

                if (Gradient)
                {
                    colony.GradientPalette();
                }

                Draw(image.Pixbuf, Width, Height, colony, maxStates, ref population);

                colony.SetNeighborhood(Neighborhood);
                colony.SetCyclic(Cyclic);

                colony.ApplyChanges();

                return colony;
            }

            if (type == ColonyTypes.Type.Zhabotinsky)
            {
                var colony = new Zhabotinsky(Width, Height, color);

                Draw(image.Pixbuf, Width, Height, colony, 256, ref population);

                colony.SetParameters(new List<Parameter>{
                    new Parameter("g", Utility.GetNumeric(Parameters, "g"), 1, 100),
                    new Parameter("k1", Utility.GetNumeric(Parameters, "k1"), 1, 100),
                    new Parameter("k2", Utility.GetNumeric(Parameters, "k2"), 1, 100),
                    new Parameter("Density", (double)population / (Width * Height), (double)1 / 100, 1)
                });

                if (Gradient)
                {
                    colony.GradientPalette();
                }

                colony.SetNeighborhood(Neighborhood);
                colony.SetCyclic(Cyclic);

                colony.ApplyChanges();

                return colony;
            }

            if (type == ColonyTypes.Type.Life)
            {
                var colony = new Life(Width, Height, color);

                Draw(image.Pixbuf, Width, Height, colony, 2, ref population);

                colony.SetParameters(new List<Parameter>
                {
                    new Parameter("Birth", Utility.GetString(Parameters, "Birth")),
                    new Parameter("Survival", Utility.GetString(Parameters, "Survival")),
                    new Parameter("Density", (double)population / (Width * Height), (double)1 / 100, 1)
                });

                colony.SetNeighborhood(Neighborhood);
                colony.SetCyclic(Cyclic);

                colony.ApplyChanges();

                return colony;
            }

            if (type == ColonyTypes.Type.ForestFire)
            {
                var colony = new ForestFire(Width, Height, color);

                Draw(image.Pixbuf, Width, Height, colony, 3, ref population);

                colony.SetParameters(new List<Parameter>
                {
                    new Parameter("P", Utility.GetNumeric(Parameters, "P"), 1, 1000),
                    new Parameter("F", Utility.GetNumeric(Parameters, "F"), 1, 1000),
                    new Parameter("Density", (double)population / (Width * Height), (double)1 / 100, 1)
                });

                colony.SetNeighborhood(Neighborhood);
                colony.SetCyclic(Cyclic);

                colony.ApplyChanges();

                return colony;
            }

            if (type == ColonyTypes.Type.LangtonAnt)
            {
                var rules = Utility.GetString(Parameters, "Rule");
                var ants = (int)Utility.GetNumeric(Parameters, "Ants");

                var colony = new LangtonAnt(Width, Height, color);

                colony.Random(!Gradient);
                colony.SetNeighborhood(Neighborhood);

                colony.SetParameters(new List<Parameter>
                {
                    new Parameter("Ants", ants, 1, 1000),
                    new Parameter("Rule", rules)
                });

                colony.SetCyclic(Cyclic);

                Draw(image.Pixbuf, Width, Height, colony, rules.Length, ref population);

                colony.ApplyChanges();

                return colony;
            }

            if (type == ColonyTypes.Type.Snowflake)
            {
                var maxStates = (int)Utility.GetNumeric(Parameters, "MaxStates");

                var colony = new Snowflake(Width, Height, color);

                colony.SetParameters(new List<Parameter>
                {
                    new Parameter("Growth", Utility.GetString(Parameters, "Growth")),
                    new Parameter("MaxStates", maxStates, 1, 256)
                });

                if (Gradient)
                {
                    colony.GradientPalette();
                }

                Draw(image.Pixbuf, Width, Height, colony, maxStates, ref population);

                colony.SetNeighborhood(Neighborhood);
                colony.SetCyclic(Cyclic);

                colony.ApplyChanges();

                return colony;
            }

            if (type == ColonyTypes.Type.Ice)
            {
                var colony = new Ice(Width, Height, color);

                Draw(image.Pixbuf, Width, Height, colony, 3, ref population);

                colony.SetParameters(new List<Parameter>
                {
                    new Parameter("Freeze", Utility.GetNumeric(Parameters, "Freeze"), 1, 1000),
                    new Parameter("Density", (double)population / (Width * Height), (double)1 / 100, 1)
                });

                colony.SetNeighborhood(Neighborhood);
                colony.SetCyclic(Cyclic);

                colony.ApplyChanges();

                return colony;
            }

            if (type == ColonyTypes.Type.Cyclic)
            {
                var maxstates = (int)Utility.GetNumeric(Parameters, "MaxStates");

                var colony = new Cyclic(Width, Height, color);

                colony.SetParameters(new List<Parameter>{
                    new Parameter("MaxStates", maxstates, 2, 256)
                });

                if (Gradient)
                {
                    colony.GradientPalette();
                }

                colony.SetNeighborhood(Neighborhood);
                colony.SetCyclic(Cyclic);

                Draw(image.Pixbuf, Width, Height, colony, maxstates, ref population);

                colony.ApplyChanges();

                return colony;
            }
        }

        return new EmptyArtificialLife();
    }

    public static void Draw(Pixbuf pixbuf, int Width, int Height, ArtificialLife colony, int MaxStates, ref int population)
    {
        population = 0;

        double delta = Math.Ceiling((double)256 / MaxStates);

        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                var offset = pixbuf.Pixels + y * pixbuf.Rowstride + x * pixbuf.NChannels;

                var r = Marshal.ReadByte(offset);
                var g = r;
                var b = r;

                if (pixbuf.NChannels > 1)
                {
                    g = Marshal.ReadByte(offset + 1);
                }

                if (pixbuf.NChannels > 2)
                {
                    b = Marshal.ReadByte(offset + 2);
                }

                var val = (int)((double)(r * 299 + g * 587 + b * 114) / 1000);

                if (colony is Life)
                {
                    (colony as Life).WriteCell(x, y, val);

                    population += val > 0 ? 1 : 0;
                }

                if (colony is YinYangFire)
                {
                    var value = (int)(val / delta);

                    (colony as YinYangFire).WriteCell(x, y, value);

                    population += value > 0 ? 1 : 0;
                }

                if (colony is Zhabotinsky)
                {
                    (colony as Zhabotinsky).WriteCell(x, y, val);

                    population += val > 0 ? 1 : 0;
                }

                if (colony is LangtonAnt)
                {
                    var value = (int)(val / delta);

                    (colony as LangtonAnt).WriteCell(x, y, value);
                }

                if (colony is ForestFire)
                {
                    (colony as ForestFire).WriteCell(x, y, Math.Max(3, val));

                    population += val & 1;
                }

                if (colony is Ice)
                {
                    (colony as Ice).WriteCell(x, y, Math.Max(3, val));

                    population += val & 1;
                }

                if (colony is Snowflake)
                {
                    var value = (int)(val / delta);

                    (colony as Snowflake).WriteCell(x, y, value);

                    population += val > 0 ? 1 : 0;
                }

                if (colony is Cyclic)
                {
                    var value = (int)(val / delta);

                    (colony as Cyclic).WriteCell(x, y, value);

                    population += value > 0 ? 1 : 0;
                }
            }
        }
    }
}
