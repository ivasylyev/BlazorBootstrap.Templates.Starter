using System.Collections;

namespace NET9.BlazorWebAppServerGlobal.Models;

public class GridSettings<T>(List<GridColumnSetting<T>> columnSettings) : IReadOnlyCollection<IGridColumnSetting>
{
    public GridSettings() : this(new List<GridColumnSetting<T>>())
    {
    }

    public List<GridColumnSetting<T>> ColumnSettings { get; set; } = columnSettings;
    public int Count => ColumnSettings.Count;

    public IEnumerator<IGridColumnSetting> GetEnumerator()
    {
        return ColumnSettings.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}