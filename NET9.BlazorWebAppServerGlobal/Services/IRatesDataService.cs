using BlazorBootstrap;
using NET9.BlazorWebAppServerGlobal.Models;

namespace NET9.BlazorWebAppServerGlobal.Services;

public interface IRatesDataService 
{
    public Task<GridDataProviderResult<RateDto>> GetRatesAsync(GridDataProviderRequest<RateDto> request);
}