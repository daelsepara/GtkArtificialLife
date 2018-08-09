using Gdk;
using System;
using System.Collections.Generic;

public static class Utility
{
    static Random random;

    public static void InitializeSeed()
    {
        if (random == null)
        {
            random = new Random(Guid.NewGuid().GetHashCode());
        }
    }

    public static List<Color> GenerateRandomColorPalette(Color color)
    {
        var ColorPalette = new List<Color>();

        InitializeSeed();

        for (int i = 0; i < 256; i++)
        {
            var red = random.Next(256);
            var green = random.Next(256);
            var blue = random.Next(256);

            // mix the color
            red = (red + color.Red) / 2;
            green = (green + color.Green) / 2;
            blue = (blue + color.Blue) / 2;

            ColorPalette.Add(new Color((byte)red, (byte)green, (byte)blue));
        }

        return ColorPalette;
    }

    public static List<Color> GreyPalette()
    {
        var ColorPalette = new List<Color>();

        for (int i = 0; i < 256; i++)
        {
            ColorPalette.Add(new Color((byte)i, (byte)i, (byte)i));
        }

        return ColorPalette;
    }

    public static List<Color> Gradient(Color color)
    {
        var ColorPalette = new List<Color>();

        double max = Math.Max(color.Red, Math.Max(color.Green, color.Blue));

        for (int i = 0; i < 256; i++)
        {
            var factor = (i / 255.0);

            var r = color.Red / max * factor * 255.0;
            var g = color.Green / max * factor * 255.0;
            var b = color.Blue / max * factor * 255.0;

            var red = (byte)r & 0xff;
            var green = (byte)g & 0xff;
            var blue = (byte)b & 0xff;

            ColorPalette.Add(new Color((byte)red, (byte)green, (byte)blue));
        }

        return ColorPalette;
    }
}
