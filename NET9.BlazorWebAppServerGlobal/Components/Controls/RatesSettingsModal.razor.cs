using BlazorBootstrap;
using Microsoft.AspNetCore.Components;
using NET9.BlazorWebAppServerGlobal.Models;

namespace NET9.BlazorWebAppServerGlobal.Components.Controls;

public partial class RatesSettingsModal
{
    private Modal modal = default!;

    [Parameter]
    public IEnumerable<IGridColumnSetting>? ColumnSettings { get; set; }

    [Parameter]
    public EventCallback OnOk { get; set; }

    [Parameter]
    public EventCallback OnCancel { get; set; }

    [Parameter]
    public EventCallback OnReset { get; set; }

    public async Task ShowAsync()
    {
        await modal.ShowAsync();
    }

    private async Task HandleOk()
    {
        await OnOk.InvokeAsync();
        await modal.HideAsync();
    }

    private async Task HandleCancel()
    {
        await OnCancel.InvokeAsync();
        await modal.HideAsync();
    }

    private async Task HandleReset()
    {
        await OnReset.InvokeAsync();
        await modal.HideAsync();
    }
}