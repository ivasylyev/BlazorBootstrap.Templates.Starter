using BlazorBootstrap;
using Microsoft.AspNetCore.Components;
using NET9.BlazorWebAppServerGlobal.Models;
using NET9.BlazorWebAppServerGlobal.Services;

namespace NET9.BlazorWebAppServerGlobal.Components.Pages;

public partial class Rates
{
    [Inject] public IRatesService RatesService { get; set; } = default!;
    private readonly Dictionary<string, GridColumnSettings> columnSettings;

    public Rates()
    {
        GridColumnSettings[] arr =
        [
            new GridColumnSettings("Code","Code"),
            new GridColumnSettings("IsDefRate","Дефлятор"),
            new GridColumnSettings("RateTypeName","Тип ставки"),
            new GridColumnSettings("NodeFromNameRu", "Отправление"),
            new GridColumnSettings("ProxyNodeNameRu", "Промежуточный"),
            new GridColumnSettings("NodeToNameRu", "Назначение"),
            new GridColumnSettings("ProductGroupName", "Группа продуктов"),
            new GridColumnSettings("StartDate", "Начало"),
            new GridColumnSettings("EndDate", "Окончание"),
            new GridColumnSettings("CurrencyCode", "Валюта"),
            new GridColumnSettings("TotalCostTon", "За тонну")
        ];
        columnSettings = arr.ToDictionary(s => s.Name, s => s);
    }

    private async Task<GridDataProviderResult<RateDto>> CustomersDataProvider(GridDataProviderRequest<RateDto> request)
    {
        var result = await RatesService.CustomersDataProvider(request);
        return result;
    }
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {

        if (firstRender)
        {

            StateHasChanged();
            /*
            var saved = await localStorage.GetItemAsync<Dictionary<string, bool>>(StorageKey);
            if (saved != null)
            {
                columnVisibility = saved;
                StateHasChanged(); // Принудительно перерисовываем, т.к. данные пришли позже
            }
            */
        }
    }
}