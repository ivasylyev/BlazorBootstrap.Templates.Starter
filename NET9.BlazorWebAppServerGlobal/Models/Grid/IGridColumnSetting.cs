namespace NET9.BlazorWebAppServerGlobal.Models.Grid;

public interface IGridColumnSetting
{
    public string Name { get; set; }
    public string Header { get; set; }
    public bool Visible { get; set; }
    public bool Filterable { get; set; }
}