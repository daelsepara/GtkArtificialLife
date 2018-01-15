public class Colony
{
	public int X;
	public int Y;

	public ArtificialLife ArtificialLife;

	public Colony()
	{
		X = 0;
		Y = 0;

		ArtificialLife = new EmptyArtificialLife();
	}

	public Colony(int x, int y, ArtificialLife artificialLife)
	{
		X = x;
		Y = y;

		ArtificialLife = artificialLife;
	}
}
