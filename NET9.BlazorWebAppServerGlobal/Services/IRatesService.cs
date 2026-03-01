using BlazorBootstrap;
using NET9.BlazorWebAppServerGlobal.Models;

namespace NET9.BlazorWebAppServerGlobal.Services;

public interface IRatesService
{
      public Task<GridDataProviderResult<RateDto>> CustomersDataProvider(GridDataProviderRequest<RateDto> request);
}
