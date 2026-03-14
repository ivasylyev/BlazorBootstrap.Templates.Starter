using System.Linq.Expressions;

namespace NET9.BlazorWebAppServerGlobal.Models;

public class GridColumnSettings(
    string name,
    string header,
    Func<RateDto, object> displaySelector,
    Expression<Func<RateDto, IComparable>> sortSelector,
    bool filterable,
    bool visible)
{
    public GridColumnSettings(string name,
        string header,
        Func<RateDto, object> displaySelector,
        Expression<Func<RateDto, IComparable>> sortSelector)
        : this(name, header, displaySelector, sortSelector, true, true)
    {
    }

    public string Name { get; set; } = name;
    public string Header { get; set; } = header;
    public bool Visible { get; set; } = visible;
    public bool Filterable { get; set; } = filterable;
    

    public Func<RateDto, object> DisplaySelector { get; set; } = displaySelector;

    public Expression<Func<RateDto, IComparable>> SortSelector { get; set; } = sortSelector;
}