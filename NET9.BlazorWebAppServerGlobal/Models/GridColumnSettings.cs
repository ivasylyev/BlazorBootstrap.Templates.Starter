using System.Linq.Expressions;

namespace NET9.BlazorWebAppServerGlobal.Models;

public class GridColumnSettings<T>(
    string name,
    string header,
    Func<T, object> displaySelector,
    Expression<Func<T, IComparable>> sortSelector,
    bool filterable,
    bool visible) where T: class
{
    public GridColumnSettings(string name,
        string header,
        Func<T, object> displaySelector,
        Expression<Func<T, IComparable>> sortSelector)
        : this(name, header, displaySelector, sortSelector, true, true)
    {
    }

    public string Name { get; set; } = name;
    public string Header { get; set; } = header;
    public bool Visible { get; set; } = visible;
    public bool Filterable { get; set; } = filterable;
    

    public Func<T, object> DisplaySelector { get; set; } = displaySelector;

    public Expression<Func<T, IComparable>> SortSelector { get; set; } = sortSelector;
}