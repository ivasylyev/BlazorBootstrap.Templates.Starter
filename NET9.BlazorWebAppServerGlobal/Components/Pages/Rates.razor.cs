using BlazorBootstrap;
using Microsoft.AspNetCore.Components;
using NET9.BlazorWebAppServerGlobal.Models;
using NET9.BlazorWebAppServerGlobal.Services;

namespace NET9.BlazorWebAppServerGlobal.Components.Pages;

public partial class Rates
{
    private Modal modal = default!;
    private Dictionary<string, RateGridColumnSettings>? columnSettings;

    [Inject]
    public IRatesService RatesService { get; set; } = default!;

    private async Task<GridDataProviderResult<RateDto>> RatesDataProvider(GridDataProviderRequest<RateDto> request)
    {
        var result = await RatesService.GetRatesAsync(request);
        return result;
    }

    protected override async Task OnInitializedAsync()
    {
        columnSettings = await RatesService.GetRatesGridColumnSettingsAsync();
    }


    private async Task ShowSettingsAsync()
    {
        await modal.ShowAsync();
    }

    private async Task OnOkClick()
    {
        if (columnSettings is not null)
        {
            await RatesService.PostRatesGridColumnSettingsAsync(columnSettings);
        }

        await modal.HideAsync();
    }

    private async Task OnCancelClick()
    {
        columnSettings = await RatesService.GetRatesGridColumnSettingsAsync();
        await modal.HideAsync();
    }

    private async Task OnResetClick()
    {
        await RatesService.ResetRatesGridColumnSettingsAsync();
        columnSettings = await RatesService.GetRatesGridColumnSettingsAsync();

        await modal.HideAsync();
    }
}