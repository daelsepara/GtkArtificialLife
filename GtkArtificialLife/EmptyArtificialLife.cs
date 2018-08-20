using System.Collections.Generic;

public class EmptyArtificialLife : ArtificialLife
{
    public override void Update()
    {

    }

    public override void Refresh()
    {

    }

    public override List<Parameter> Parameters()
    {
        return new List<Parameter>();
    }

    public override void SetParameters(List<Parameter> parameters)
    {
        parameters.Clear();
    }
}
