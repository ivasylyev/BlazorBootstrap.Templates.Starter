using BlazorBootstrap;
using Microsoft.AspNetCore.Components;
using NET9.BlazorWebAppServerGlobal.Models.Dto;
using NET9.BlazorWebAppServerGlobal.Services.Rates;
using NET9.BlazorWebAppServerGlobal.Services.Shared;

namespace NET9.BlazorWebAppServerGlobal.Components.Pages;

public partial class RatesGrid
{
    [Inject] 
    public IRatesDataService RatesDataService { get; set; } = default!;
    [Inject] 
    public PageTimingService TimingService { get; set; } = default!;

    private async Task<GridDataProviderResult<RateDto>> RatesDataProvider(GridDataProviderRequest<RateDto> request)
    {
        using (new StopwatchTransaction(TimingService))
        {
            return await RatesDataService.GetRatesAsync(request);
        }
    }
}