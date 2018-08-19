using Gdk;
using System;
using System.Collections.Generic;

public static class Utility
{
    static Random random = new Random(Guid.NewGuid().GetHashCode());

    public static List<Color> GenerateRandomColorPalette(Color color)
    {
        var ColorPalette = new List<Color>();

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

    public static List<Color> Gradient(Color color, int states = 255)
    {
        var ColorPalette = new List<Color>();

        var max = Math.Max(color.Red, Math.Max(color.Green, color.Blue));

        var delta = (256 / states);

        for (int i = 0; i < 256; i++)
        {
            int block = (int)(i / delta);
            var factor = (double)block * delta / 255;

            var r = (double)color.Red / max * factor * 255.0;
            var g = (double)color.Green / max * factor * 255.0;
            var b = (double)color.Blue / max * factor * 255.0;

            var red = (byte)r & 0xff;
            var green = (byte)g & 0xff;
            var blue = (byte)b & 0xff;

            ColorPalette.Add(new Color((byte)red, (byte)green, (byte)blue));
        }

        return ColorPalette;
    }
}
