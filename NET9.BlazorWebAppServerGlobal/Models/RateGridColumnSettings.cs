using System.Linq.Expressions;

namespace NET9.BlazorWebAppServerGlobal.Models;

public class RateGridColumnSettings : GridColumnSettings<RateDto>
{
    public RateGridColumnSettings(string name, string header, Func<RateDto, object> displaySelector, Expression<Func<RateDto, IComparable>> sortSelector) : base(name, header, displaySelector, sortSelector)
    {
    }

    public RateGridColumnSettings(string name, string header, Func<RateDto, object> displaySelector, Expression<Func<RateDto, IComparable>> sortSelector, bool filterable, bool visible) : base(name, header, displaySelector, sortSelector, filterable, visible)
    {
    }
}