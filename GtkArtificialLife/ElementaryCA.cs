using Gdk;
using System.Collections.Generic;

public class ElementaryCA : ArtificialLife
{
    List<Pixel> PixelWriteBuffer = new List<Pixel>();
    List<Change> ChangeList = new List<Change>();

    int Rule;
    int Current;
    int[,] Grid;

    public ElementaryCA()
    {
        Current = 0;

        InitGrid(256, 256);

        ColonyColor = DefaultColor;

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

    protected void InitGrid(int width, int height)
    {
        Width = width;
        Height = height;

        Grid = new int[width, height];
    }

    public override void ClearPixelWriteBuffer()
    {
        PixelWriteBuffer.Clear();
    }

    public override List<Pixel> GetPixelWriteBuffer()
    {
        return new List<Pixel>(PixelWriteBuffer);
    }

    public void WriteCell(int x, int y, int val)
    {
        if (x >= 0 && x < Width && y >= 0 && y < Height)
        {
            PushPixel(new Pixel(x, y, val > 0 ? ColonyColor : EmptyColor));
            ChangeList.Add(new Change(x, y, val > 0 ? 1 : 0));
        }
    }

    protected void RemovePixel(int index)
    {
        if (PixelWriteBuffer.Count > 0 && index < PixelWriteBuffer.Count)
        {
            PixelWriteBuffer.RemoveAt(index);
        }
    }

    public void PushPixel(Pixel pixel)
    {
        if (pixel != null)
        {
            PixelWriteBuffer.Add(pixel);
        }
    }

    public Pixel PopPixel()
    {
        if (PixelWriteBuffer.Count < 1)
        {
            return null;
        }

        var pixel = PixelWriteBuffer[PixelWriteBuffer.Count - 1];

        RemovePixel(PixelWriteBuffer.Count - 1);

        return pixel;
    }

    protected bool IsAlive(int x, int y)
    {
        return Grid[x, y] != 0;
    }

    public override void Update()
    {
        Refresh();

        if (Current < Height - 1)
        {
            for (int x = 1; x < Width - 1; x++)
            {
                int count = 0;

                count += IsAlive(x, Current) ? 2 : 0;
                count += IsAlive(x - 1, Current) ? 4 : 0;
                count += IsAlive(x + 1, Current) ? 1 : 0;

                // Peform 1D Elementary Cellular Automata Update

                // Brilliant piece of code
                int Value = (1 << count) & Rule;

                WriteCell(x, Current + 1, Value > 0 ? 1 : 0);
            }

            ApplyChanges();

            Current++;
        }
    }

    public void ApplyChanges()
    {
        foreach (var change in ChangeList)
        {
            Grid[change.X, change.Y] = change.Value;
        }

        ChangeList.Clear();
    }

    public void Refresh()
    {
        for (int y = 0; y < Height; y++)
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
        var set = new List<Parameter>
        {
            new Parameter("Rule", Rule, 0, 255)
        };

        return set;
    }

    public void SetRule(int rule)
    {
        Rule = rule & 0xff;
    }

    public Color Color()
    {
        return ColonyColor;
    }
}
