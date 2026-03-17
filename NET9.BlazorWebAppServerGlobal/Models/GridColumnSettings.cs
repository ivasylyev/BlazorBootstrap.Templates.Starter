using System.Linq.Expressions;

namespace NET9.BlazorWebAppServerGlobal.Models;

public class GridColumnSettings<T>
{

    public required string Name { get; set; }
    public required string Header { get; set; }
    public bool Visible { get; set; }
    public bool Filterable { get; set; } 
    

    public required Func<T, object> DisplaySelector { get; set; }

    public required Expression<Func<T, IComparable>> SortSelector { get; set; }

    public override string ToString()
    {
        return $"{Header} ({Name}), Visible = {Visible}, Filterable = {Filterable}";
    }
}