using BlazorBootstrap;
using NET9.BlazorWebAppServerGlobal.Models;

namespace NET9.BlazorWebAppServerGlobal.Services;

public interface IRatesService
{
    public Task<RateGridSettings> GetRatesGridColumnSettingsAsync();
    public Task ResetRatesGridColumnSettingsAsync();
    public Task PostRatesGridColumnSettingsAsync(RateGridSettings fullSettings);
    public Task<GridDataProviderResult<RateDto>> GetRatesAsync(GridDataProviderRequest<RateDto> request);
}