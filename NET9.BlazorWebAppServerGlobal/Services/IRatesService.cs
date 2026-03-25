using BlazorBootstrap;
using NET9.BlazorWebAppServerGlobal.Models;

namespace NET9.BlazorWebAppServerGlobal.Services;

public interface IRatesService
{
    public Task<GridSettings<RateDto>> GetRatesGridColumnSettingsAsync();
    public Task ResetRatesGridColumnSettingsAsync();
    public Task PostRatesGridColumnSettingsAsync(GridSettings<RateDto> fullSettings);
    public Task<GridDataProviderResult<RateDto>> GetRatesAsync(GridDataProviderRequest<RateDto> request);
}