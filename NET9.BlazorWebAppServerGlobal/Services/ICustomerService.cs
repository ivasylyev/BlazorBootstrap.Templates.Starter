using BlazorBootstrap;
using NET9.BlazorWebAppServerGlobal.Models;

namespace NET9.BlazorWebAppServerGlobal.Services;

public interface ICustomerService
{
      public Task<Tuple<IEnumerable<TransportRateDto>, int>> GetCustomersAsync(IEnumerable<FilterItem> filters, int pageNumber, int pageSize, string sortKey, SortDirection sortDirection, CancellationToken cancellationToken = default);
}
