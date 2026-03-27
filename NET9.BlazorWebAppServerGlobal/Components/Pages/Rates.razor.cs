using BlazorBootstrap;
using Microsoft.AspNetCore.Components;
using NET9.BlazorWebAppServerGlobal.Components.Controls;
using NET9.BlazorWebAppServerGlobal.Models.Dto;
using NET9.BlazorWebAppServerGlobal.Models.Grid;
using NET9.BlazorWebAppServerGlobal.Services.Rates;
using NET9.BlazorWebAppServerGlobal.Services.Shared;

namespace NET9.BlazorWebAppServerGlobal.Components.Pages;

public partial class Rates
{
    private RatesSettingsModal settingsModal = default!;
    private GridSettings<RateDto>? gridSettings;
    [Inject]
    public ILogger<Rates> Logger { get; set; } = default!;
    [Inject] 
    public IRatesDataService RatesDataService { get; set; } = default!;
    [Inject] 
    public IGridSettingsService<RateDto> GridSettingsService { get; set; } = default!;

    private async Task<GridDataProviderResult<RateDto>> RatesDataProvider(GridDataProviderRequest<RateDto> request)
    {
        var result = await RatesDataService.GetRatesAsync(request);
        return result;
    }

    protected override async Task OnInitializedAsync()
    {
        Logger.LogInformation("Rates page initializing");

        gridSettings = await GridSettingsService.GetGridSettingsAsync();
    }

    private async Task ShowSettingsAsync()
    {
        await settingsModal.ShowAsync();
    }


    private async Task OnOkClick(IReadOnlyCollection<IGridColumnSetting> settings)
    {
        if (gridSettings is not null)
        {
            gridSettings.ApplyGridColumnSettings(settings);

            await GridSettingsService.SaveGridSettingsAsync(gridSettings);
        }

        StateHasChanged();
    }
    private async Task OnCancelClick()
    {
        gridSettings = await GridSettingsService.GetGridSettingsAsync();
        StateHasChanged();
    }

    private async Task OnResetClick()
    {
        await GridSettingsService.ResetGridSettingsAsync();
        gridSettings = await GridSettingsService.GetGridSettingsAsync();
        StateHasChanged();
    }
}