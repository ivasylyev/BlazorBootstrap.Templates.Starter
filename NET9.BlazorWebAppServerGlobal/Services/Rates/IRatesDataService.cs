using BlazorBootstrap;
using NET9.BlazorWebAppServerGlobal.Models.Dto;

namespace NET9.BlazorWebAppServerGlobal.Services.Rates;

public interface IRatesDataService 
{
    public Task<GridDataProviderResult<RateDto>> GetRatesAsync(GridDataProviderRequest<RateDto> request);
}