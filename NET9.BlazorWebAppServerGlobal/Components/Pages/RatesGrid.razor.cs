using BlazorBootstrap;
using Microsoft.AspNetCore.Components;
using NET9.BlazorWebAppServerGlobal.Models.Dto;
using NET9.BlazorWebAppServerGlobal.Services.Rates;

namespace NET9.BlazorWebAppServerGlobal.Components.Pages;

public partial class RatesGrid : BaseGridPage<RateDto>
{
    [Inject]
    public IRatesDataService RatesDataService { get; set; } = default!;

    protected override async Task<GridDataProviderResult<RateDto>> GetDataAsync(GridDataProviderRequest<RateDto> request)
    {
        return await RatesDataService.GetRatesAsync(request);
    }
}