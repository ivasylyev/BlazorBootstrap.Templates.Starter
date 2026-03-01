using BlazorBootstrap;
using Microsoft.AspNetCore.Components;
using NET9.BlazorWebAppServerGlobal.Models;
using NET9.BlazorWebAppServerGlobal.Services;

namespace NET9.BlazorWebAppServerGlobal.Components.Pages;

public partial class Rates
{
    [Inject] public IRatesService RatesService { get; set; } = default!;

    private async Task<GridDataProviderResult<RateDto>> CustomersDataProvider(GridDataProviderRequest<RateDto> request)
    {
        var result = await RatesService.CustomersDataProvider(request);
        return result;
    }
}