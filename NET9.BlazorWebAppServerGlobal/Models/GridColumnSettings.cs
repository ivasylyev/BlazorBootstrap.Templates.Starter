namespace NET9.BlazorWebAppServerGlobal.Models;

public class GridColumnSettings(string name, string header, Func<RateDto, object> displaySelector, bool visible)
{
    public GridColumnSettings(string name, string header, Func<RateDto, object> displaySelector) : this(name, header, displaySelector, true)
    {
    }

    public string Name { get; set; } = name;
    public string Header { get; set; } = header;
    public bool Visible { get; set; } = visible;

    public Func<RateDto, object> DisplaySelector { get; set; } = displaySelector;
}