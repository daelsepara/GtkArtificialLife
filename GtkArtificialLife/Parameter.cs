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

    public Parameter(string name, string value)
    {
        Name = name;
        Value = value;
        IsNumeric = false;
        NumericValue = 0.0;
        Min = 0.0;
        Max = 0.0;
    }

    public Parameter(string name, double numericValue, double min, double max)
    {
        Name = name;
        Value = "";
        IsNumeric = true;
        NumericValue = numericValue;
        Min = min;
        Max = max;
    }

    public Parameter(string name, string value, bool isNumeric, double numericValue, double min, double max)
    {
        Name = name;
        Value = value;
        IsNumeric = isNumeric;
        NumericValue = numericValue;
        Min = min;
        Max = max;
    }
}
