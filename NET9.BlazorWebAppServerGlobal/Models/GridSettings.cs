namespace NET9.BlazorWebAppServerGlobal.Models;

public class GridSettings<T>(Dictionary<string, GridColumnSetting<T>> columnSettings) 
{
    public GridSettings() 
        : this(new Dictionary<string, GridColumnSetting<T>>())
    {
    }

    public Dictionary<string, GridColumnSetting<T>> ColumnSettings { get; set; } = columnSettings;
}