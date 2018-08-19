using Gdk;
using System.Collections.Generic;

public class ElementaryCA : ArtificialLife
{
    int Rule;
    int Current;

    public ElementaryCA()
    {
        Current = 0;

        InitGrid(256, 256);

        ColonyColor = DefaultColor;

        WriteCell(128, 0, 1);

        ApplyChanges();
    }

    public ElementaryCA(int width, int height)
    {
        InitGrid(width, height);

        Current = 0;

        ColonyColor = DefaultColor;

        WriteCell(width / 2, 0, 1);

        ApplyChanges();
    }

    public ElementaryCA(int width, int height, Color color)
    {
        InitGrid(width, height);

        Current = 0;

        if (!color.Equal(EmptyColor))
        {
            ColonyColor.Red = color.Red;
            ColonyColor.Green = color.Green;
            ColonyColor.Blue = color.Blue;
        }
        else
        {
            ColonyColor = DefaultColor;
        }

        WriteCell(width / 2, 0, 1);

        ApplyChanges();
    }

    public void WriteCell(int x, int y, int val)
    {
        if (x >= 0 && x < Width && y >= 0 && y < Height)
        {
            PushPixel(new Pixel(x, y, val > 0 ? ColonyColor : EmptyColor));

            ChangeList.Add(new Change(x, y, val > 0 ? 1 : 0));
        }
    }

    bool IsAlive(int x, int y)
    {
        return (x >= 0 && x < Width && y >= 0 && y < Height) && Grid[x, y] != 0;
    }

    public override void Update()
    {
        Refresh();

        if (Current < Height - 1)
        {
            for (int x = 0; x < Width; x++)
            {
                var count = IsAlive(x, Current) ? 2 : 0;
                count += IsAlive(Cyclic ? World.Cyclic(x, -1, Width) : x - 1, Current) ? 4 : 0;
                count += IsAlive(Cyclic ? World.Cyclic(x, 1, Width) : x + 1, Current) ? 1 : 0;

                // Peform 1D Elementary Cellular Automata Update - Brilliant piece of code
                int Value = (1 << count) & Rule;

                WriteCell(x, Current + 1, Value > 0 ? 1 : 0);
            }

            ApplyChanges();

            Current++;
        }
    }

    public override void Refresh()
    {
        for (int y = 0; y < Current + 1; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                if (Grid[x, y] > 0)
                {
                    WriteCell(x, y, 1);
                }
            }
        }
    }

    public override List<Parameter> Parameters()
    {
        return new List<Parameter>
        {
            new Parameter("Rule", Rule, 0, 255)
        };
    }

    public void SetRule(int rule)
    {
        Rule = rule & 0xff;
    }
}
