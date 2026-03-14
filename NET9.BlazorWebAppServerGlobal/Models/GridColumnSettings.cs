namespace NET9.BlazorWebAppServerGlobal.Models;

public class GridColumnSettings(string name, string header, bool visible)
{
    public string Name { get; set; } = name;
    public string Header { get; set; } = header;
    public bool Visible { get; set; } = visible;

    public GridColumnSettings(string name, string header) : this(name, header,true)
    {
    }
}