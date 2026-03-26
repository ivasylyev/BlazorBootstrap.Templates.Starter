using System.Collections;

namespace NET9.BlazorWebAppServerGlobal.Models;

public class GridSettings<T>(Dictionary<string, GridColumnSetting<T>> columnSettings) :IEnumerable<IGridColumnSetting>
{
    public GridSettings() 
        : this(new Dictionary<string, GridColumnSetting<T>>())
    {
    }

    public Dictionary<string, GridColumnSetting<T>> ColumnSettings { get; set; } = columnSettings;
    public IEnumerator<IGridColumnSetting> GetEnumerator()
    {
        return ColumnSettings.Values.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}