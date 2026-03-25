using BlazorBootstrap;
using Microsoft.AspNetCore.Components;
using NET9.BlazorWebAppServerGlobal.Components.Controls;
using NET9.BlazorWebAppServerGlobal.Models;
using NET9.BlazorWebAppServerGlobal.Services;

namespace NET9.BlazorWebAppServerGlobal.Components.Pages;

public partial class Rates
{
    private RatesSettingsModal settingsModal = default!;
    private GridSettings<RateDto>? gridSettings;

    [Inject]
    public IRatesService RatesService { get; set; } = default!;

    private async Task<GridDataProviderResult<RateDto>> RatesDataProvider(GridDataProviderRequest<RateDto> request)
    {
        var result = await RatesService.GetRatesAsync(request);
        return result;
    }

    protected override async Task OnInitializedAsync()
    {
        gridSettings = await RatesService.GetRatesGridColumnSettingsAsync();
    }

    private async Task ShowSettingsAsync()
    {
        await settingsModal.ShowAsync();
    }

    private async Task OnOkClick()
    {
        if (gridSettings is not null)
        {
            await RatesService.PostRatesGridColumnSettingsAsync(gridSettings);
        }
        StateHasChanged();
    }

    private async Task OnCancelClick()
    {
        gridSettings = await RatesService.GetRatesGridColumnSettingsAsync();
        StateHasChanged();
    }

    private async Task OnResetClick()
    {
        await RatesService.ResetRatesGridColumnSettingsAsync();
        gridSettings = await RatesService.GetRatesGridColumnSettingsAsync();
        StateHasChanged();
    }
}