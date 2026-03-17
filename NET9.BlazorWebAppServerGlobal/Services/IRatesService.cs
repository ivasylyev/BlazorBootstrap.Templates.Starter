using BlazorBootstrap;
using NET9.BlazorWebAppServerGlobal.Models;

namespace NET9.BlazorWebAppServerGlobal.Services;

public interface IRatesService
{
    public Task<Dictionary<string, RateGridColumnSettings>> GetRatesGridColumnSettingsAsync();
    public Task ResetRatesGridColumnSettingsAsync();
    public Task PostRatesGridColumnSettingsAsync(Dictionary<string, RateGridColumnSettings> fullSettings);
    public Task<GridDataProviderResult<RateDto>> GetRatesAsync(GridDataProviderRequest<RateDto> request);
}