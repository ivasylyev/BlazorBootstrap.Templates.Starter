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
            new GridColumnSettings("Code","Code", dto => dto.Code),
            new GridColumnSettings("IsDefRate","Дефлятор", dto => dto.IsDefRate),
            new GridColumnSettings("RateTypeName","Тип ставки", dto => dto.RateTypeName),
            new GridColumnSettings("NodeFromNameRu", "Отправление", dto => dto.NodeFromNameRu),
            new GridColumnSettings("ProxyNodeNameRu", "Промежуточный", dto => dto.ProxyNodeNameRu),
            new GridColumnSettings("NodeToNameRu", "Назначение", dto => dto.NodeToNameRu),
            new GridColumnSettings("ProductGroupName", "Группа продуктов", dto => dto.ProductGroupName),
            new GridColumnSettings("StartDate", "Начало", dto => dto.StartDate.ToShortDateString()),
            new GridColumnSettings("EndDate", "Окончание", dto => dto.EndDate?.ToShortDateString()),
            new GridColumnSettings("CurrencyCode", "Валюта", dto => dto.CurrencyCode),
            new GridColumnSettings("TotalCostTon", "За тонну", dto => dto.TotalCostTon)
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