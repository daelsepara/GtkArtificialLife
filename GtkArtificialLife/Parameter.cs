public class Parameter
{
    public string Name;
    public string Value;

    public bool IsNumeric;
    public double NumericValue;
    public double Min;
    public double Max;

    public Parameter()
    {
    }

    public Parameter(string Name, string Value)
    {
        this.Name = Name;
        this.Value = Value;
        IsNumeric = false;
        NumericValue = 0.0;
        Min = 0.0;
        Max = 0.0;
    }

    public Parameter(string Name, double NumericValue, double Min, double Max)
    {
        this.Name = Name;
        Value = "";
        IsNumeric = true;
        this.NumericValue = NumericValue;
        this.Min = Min;
        this.Max = Max;
    }

    public Parameter(string Name, string Value, bool IsNumeric, double NumericValue, double Min, double Max)
    {
        this.Name = Name;
        this.Value = Value;
        this.IsNumeric = IsNumeric;
        this.NumericValue = NumericValue;
        this.Min = Min;
        this.Max = Max;
    }
}
