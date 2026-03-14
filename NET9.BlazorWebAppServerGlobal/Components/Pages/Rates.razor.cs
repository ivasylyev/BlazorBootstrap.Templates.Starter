using BlazorBootstrap;
using Microsoft.AspNetCore.Components;
using NET9.BlazorWebAppServerGlobal.Models;
using NET9.BlazorWebAppServerGlobal.Services;

namespace NET9.BlazorWebAppServerGlobal.Components.Pages;

public partial class Rates
{
    [Inject] public IRatesService RatesService { get; set; } = default!;
    private readonly Dictionary<string, RateGridColumnSettings> columnSettings;

    public Rates()
    {
        RateGridColumnSettings[] arr =
        [
            new RateGridColumnSettings("Code","Code", dto => dto.Code, dto => dto.Code ),
            new RateGridColumnSettings("IsDefRate","Дефлятор", dto => dto.IsDefRate, dto => dto.IsDefRate ),
            new RateGridColumnSettings("RateTypeName","Тип ставки", dto => dto.RateTypeName, dto => dto.RateTypeName, false, true),
            new RateGridColumnSettings("NodeFromNameRu", "Отправление", dto => dto.NodeFromNameRu, dto => dto.NodeFromNameRu),
            new RateGridColumnSettings("ProxyNodeNameRu", "Промежуточный", dto => dto.ProxyNodeNameRu, dto => dto.ProxyNodeNameRu),
            new RateGridColumnSettings("NodeToNameRu", "Назначение", dto => dto.NodeToNameRu, dto => dto.NodeToNameRu),
            new RateGridColumnSettings("ProductGroupName", "Группа продуктов", dto => dto.ProductGroupName, dto => dto.ProductGroupName),
            new RateGridColumnSettings("StartDate", "Начало", dto => dto.StartDate.ToShortDateString(), dto => dto.StartDate),
            new RateGridColumnSettings("EndDate", "Окончание", dto => dto.EndDate.ToShortDateString(), dto => dto.EndDate),
            new RateGridColumnSettings("CurrencyCode", "Валюта", dto => dto.CurrencyCode, dto => dto.CurrencyCode, false, true),
            new RateGridColumnSettings("TotalCostTon", "За тонну", dto => dto.TotalCostTon, dto => dto.TotalCostTon, false, true)
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