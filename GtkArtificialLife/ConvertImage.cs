using Gdk;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

public static class ConvertImage
{
    static Random random = new Random(Guid.NewGuid().GetHashCode());

    public static ArtificialLife Convert(ColonyTypes.Type type, Gtk.Image image, int Width, int Height, List<Parameter> Parameters, Color color, bool Gradient = false)
    {
        int population = 0;

        if (image.Pixbuf != null)
        {
            if (type == ColonyTypes.Type.YinYangFire)
            {
                var maxstates = (int)GetNumeric(Parameters, "MaxStates");

                var colony = new YinYangFire(Width, Height, maxstates, color);

                if (Gradient)
                {
                    colony.GradientPalette();
                }

                Draw(image.Pixbuf, Width, Height, colony, maxstates, ref population);

                colony.SetDensity(population);

                colony.ApplyChanges();

                return colony;
            }

            if (type == ColonyTypes.Type.Zhabotinsky)
            {
                var g = GetNumeric(Parameters, "g");
                var k1 = GetNumeric(Parameters, "k1");
                var k2 = GetNumeric(Parameters, "k2");

                var colony = new Zhabotinsky(Width, Height, color);

                colony.SetParameters(k1, k2, g);

                if (Gradient)
                {
                    colony.GradientPalette();
                }

                Draw(image.Pixbuf, Width, Height, colony, 256, ref population);

                colony.SetDensity(population);

                colony.ApplyChanges();

                return colony;
            }

            if (type == ColonyTypes.Type.Life)
            {
                var colony = new Life(Width, Height, color);

                Draw(image.Pixbuf, Width, Height, colony, 2, ref population);

                colony.SetDensity(population);

                colony.ApplyChanges();

                return colony;
            }

            if (type == ColonyTypes.Type.ForestFire)
            {
                var F = GetNumeric(Parameters, "F");
                var P = GetNumeric(Parameters, "P");

                var colony = new ForestFire(Width, Height, color);

                colony.SetParameters(F, P);

                Draw(image.Pixbuf, Width, Height, colony, 3, ref population);

                colony.SetDensity(population);

                colony.ApplyChanges();

                return colony;
            }

            if (type == ColonyTypes.Type.LangtonAnt)
            {
                var rules = GetString(Parameters, "Rule");

                var colony = new LangtonAnt(Width, Height, color);

                if (Gradient)
                {
                    colony.GradientPalette();
                }

                colony.SetRules(rules);
                colony.ParseRulesOnce();

                Draw(image.Pixbuf, Width, Height, colony, 4, ref population);

                colony.ApplyChanges();

                return colony;
            }
        }

        return new EmptyArtificialLife();
    }

    static double GetNumeric(List<Parameter> parameters, String name)
    {
        var item = parameters.Find(parameter => parameter.Name == name);

        return item.NumericValue;
    }

    static string GetString(List<Parameter> parameters, String name)
    {
        var item = parameters.Find(parameter => parameter.Name == name);

        return item.Value;
    }

    public static void Draw(Pixbuf pixbuf, int Width, int Height, ArtificialLife colony, int MaxStates, ref int population)
    {
        population = 0;

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

                var val = (int)(r * 0.299 + g * 0.587 + b * 0.114);

                if (colony is Life)
                {
                    (colony as Life).WriteCell(x, y, val & 1);

                    population += val & 1;
                }

                if (colony is YinYangFire)
                {
                    var value = (int)Mod(val, MaxStates);

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
                    (colony as LangtonAnt).WriteCell(x, y, (int)Mod(val, MaxStates));
                }

                if (colony is ForestFire)
                {
                    (colony as ForestFire).WriteCell(x, y, Math.Max(3, val));

                    population += val & 1;
                }
            }
        }
    }

    public static double Mod(double a, double m)
    {
        return a - m * Math.Floor(a / m);
    }
}
