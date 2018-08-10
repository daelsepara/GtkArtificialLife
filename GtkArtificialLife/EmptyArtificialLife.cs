using Gdk;
using System.Collections.Generic;

public class EmptyArtificialLife : ArtificialLife
{
    public override List<Pixel> GetPixelWriteBuffer()
    {
        return null;
    }

    public override void ClearPixelWriteBuffer()
    {

    }

    public override void Update()
    {

    }

    public override List<Parameter> Parameters()
    {
        return new List<Parameter>();
    }

    public override void Refresh()
    {

    }

    public override Color Color()
    {
        return DefaultColor;
    }
}
