using System.Collections.Generic;

public class Cell
{
	public int X;
	public int Y;

	public Cell()
	{
		
	}

	public Cell(int x, int y)
	{
		X = x;
		Y = y;
	}
}

abstract public class ArtificialLife
{
	public int Width;
	public int Height;
	public Gdk.Color ColonyColor;

	protected Gdk.Color DefaultColor = new Gdk.Color(0x00ff, 0x00ff, 0x00ff);
	protected Gdk.Color EmptyColor = new Gdk.Color(0x0000, 0x0000, 0x0000);

	// Rendering
	abstract public List<Pixel> GetPixelWriteBuffer();

	abstract public void ClearPixelWriteBuffer();

	abstract public void Update();
}
