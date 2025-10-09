using BlazorBootstrap;
using NET9.BlazorWebAppServerGlobal.Models;

namespace NET9.BlazorWebAppServerGlobal.Services;

public interface IRatesService
{
      public Task<Tuple<IEnumerable<RateDto>, int>> GetCustomersAsync(IEnumerable<FilterItem> filters, int pageNumber, int pageSize, string sortKey, SortDirection sortDirection, CancellationToken cancellationToken = default);
}
