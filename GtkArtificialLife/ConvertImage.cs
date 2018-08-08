using Gdk;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

public static class ConvertImage
{
    static Random random = new Random(Guid.NewGuid().GetHashCode());

    public static ArtificialLife Convert(ColonyTypes.Type type, Gtk.Image image, List<Parameter> Parameters, Color color)
    {
        int population = 0;

        if (image.Pixbuf != null)
        {
            if (type == ColonyTypes.Type.YinYangFire)
            {
                var colony = new YinYangFire(image.Pixbuf.Width, image.Pixbuf.Height, color);

                var maxstates = (int)GetNumeric(Parameters, "MaxStates");

                Draw(image.Pixbuf, colony, maxstates, ref population);

                colony.SetDensity(population);
                colony.SetParameters(maxstates);

                colony.AddNeighbor(new Cell(-1, -1));
                colony.AddNeighbor(new Cell(0, -1));
                colony.AddNeighbor(new Cell(1, -1));
                colony.AddNeighbor(new Cell(-1, 0));
                colony.AddNeighbor(new Cell(1, 0));
                colony.AddNeighbor(new Cell(-1, 1));
                colony.AddNeighbor(new Cell(0, 1));
                colony.AddNeighbor(new Cell(1, 1));

                colony.ApplyChanges();

                return colony;
            }

            if (type == ColonyTypes.Type.Zhabotinsky)
            {
                var colony = new Zhabotinsky(image.Pixbuf.Width, image.Pixbuf.Height, color);

                var g = GetNumeric(Parameters, "g");
                var k1 = GetNumeric(Parameters, "k1");
                var k2 = GetNumeric(Parameters, "k2");

                Draw(image.Pixbuf, colony, 256, ref population);

                colony.SetParameters(k1, k2, g);
                colony.SetDensity(population);

                colony.AddNeighbor(new Cell(-1, -1));
                colony.AddNeighbor(new Cell(0, -1));
                colony.AddNeighbor(new Cell(1, -1));
                colony.AddNeighbor(new Cell(-1, 0));
                colony.AddNeighbor(new Cell(1, 0));
                colony.AddNeighbor(new Cell(-1, 1));
                colony.AddNeighbor(new Cell(0, 1));
                colony.AddNeighbor(new Cell(1, 1));

                colony.ApplyChanges();

                return colony;
            }

            if (type == ColonyTypes.Type.Life)
            {
                var colony = new Life(image.Pixbuf.Width, image.Pixbuf.Height, color);

                Draw(image.Pixbuf, colony, 2, ref population);

                colony.SetDensity(population);

                colony.AddNeighbor(new Cell(-1, -1));
                colony.AddNeighbor(new Cell(0, -1));
                colony.AddNeighbor(new Cell(1, -1));
                colony.AddNeighbor(new Cell(-1, 0));
                colony.AddNeighbor(new Cell(1, 0));
                colony.AddNeighbor(new Cell(-1, 1));
                colony.AddNeighbor(new Cell(0, 1));
                colony.AddNeighbor(new Cell(1, 1));

                colony.AddBirthRule(3);
                colony.AddSurvivalRule(2);
                colony.AddSurvivalRule(3);

                colony.ApplyChanges();

                return colony;
            }

            if (type == ColonyTypes.Type.ForestFire)
            {
                var F = GetNumeric(Parameters, "F");
                var P = GetNumeric(Parameters, "P");

                var colony = new ForestFire(image.Pixbuf.Width, image.Pixbuf.Height, color);

                colony.SetParameters(F, P);

                colony.AddNeighbor(new Cell(-1, -1));
                colony.AddNeighbor(new Cell(0, -1));
                colony.AddNeighbor(new Cell(1, -1));
                colony.AddNeighbor(new Cell(-1, 0));
                colony.AddNeighbor(new Cell(1, 0));
                colony.AddNeighbor(new Cell(-1, 1));
                colony.AddNeighbor(new Cell(0, 1));
                colony.AddNeighbor(new Cell(1, 1));

                Draw(image.Pixbuf, colony, 3, ref population);

                colony.SetDensity(population);
                colony.ApplyChanges();

                return colony;
            }

            if (type == ColonyTypes.Type.LangtonAnt)
            {
                var colony = new LangtonAnt(image.Pixbuf.Width, image.Pixbuf.Height, color);

                var rules = GetString(Parameters, "Rule");

                colony.SetRules(rules);

                Draw(image.Pixbuf, colony, 4, ref population);

                colony.ApplyChanges();

                return colony;
            }
        }

        return new EmptyArtificialLife();
    }

    static double GetNumeric(List<Parameter> parameters, String name)
    {
        var item = parameters.Find(parameter => parameter.Name == name);

        Console.WriteLine("{0} = {1}", name, item.NumericValue);

        return item.NumericValue;
    }

    static string GetString(List<Parameter> parameters, String name)
    {
        var item = parameters.Find(parameter => parameter.Name == name);

        Console.WriteLine("{0} = {1}", name, item.Value);

        return item.Value;
    }

    public static void Draw(Pixbuf pixbuf, ArtificialLife colony, int MaxStates, ref int population)
    {
        population = 0;

        for (int y = 0; y < pixbuf.Height; y++)
        {
            for (int x = 0; x < pixbuf.Width; x++)
            {
                var offset = pixbuf.Pixels + y * pixbuf.Rowstride + x * pixbuf.NChannels;

                var r = Marshal.ReadByte(offset);
                var g = r;
                var b = r;

                if (pixbuf.NChannels > 1)
                    g = Marshal.ReadByte(offset + 1);
                    
                if (pixbuf.NChannels > 2)
                    b = Marshal.ReadByte(offset + 2);

                var val = (int)(r * 0.299 + g * 0.587 + b * 0.114);

                if (colony is Life)
                {
                    (colony as Life).WriteGrid(x, y, val & 1);

                    population += val & 1;
                }

                if (colony is YinYangFire)
                {
                    (colony as YinYangFire).WriteGrid(x, y, (int)Mod(val, MaxStates));

                    population++;
                }

                if (colony is Zhabotinsky)
                {
                    (colony as Zhabotinsky).WriteGrid(x, y, val);

                    population++;
                }

                if (colony is LangtonAnt)
                {
                    (colony as LangtonAnt).WriteGrid(x, y, random.Next(0, MaxStates));
                }

                if (colony is ForestFire)
                {
                    (colony as ForestFire).WriteGrid(x, y, val & 1);

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
