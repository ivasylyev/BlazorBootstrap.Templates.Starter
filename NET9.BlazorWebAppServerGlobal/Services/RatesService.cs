using BlazorBootstrap;
using Blazored.LocalStorage;
using Dapper;
using NET9.BlazorWebAppServerGlobal.Models;
using Newtonsoft.Json;
using System.Data;
using System.Data.SqlClient;
namespace NET9.BlazorWebAppServerGlobal.Services;

public class RatesService(ILocalStorageService localStorage) : IRatesService
{
    private const string _connectionString = "Server=S001ITD-0084;Database=mdm;Trusted_Connection=false;User ID=SVT;Password=SVTsrv1!;MultipleActiveResultSets=true;Application Name=mdm-api;Encrypt=False;TrustServerCertificate=True;Max Pool Size=1000;";
    private const string StorageKey = "RatesGridColumnSettings";


    public async Task<Dictionary<string, RateGridColumnSettings>> GetRatesGridColumnSettingsAsync()
    {
        RateGridColumnSettings[] arr =
        [
            new RateGridColumnSettings
            {
                Name = "Code",
                Header = "Code",
                DisplaySelector = dto => dto.Code,
                SortSelector = dto => dto.Code,
                Filterable = true,
                Visible = true
            },
            new RateGridColumnSettings
            {
                Name = "IsDefRate",
                Header = "Дефлятор",
                DisplaySelector = dto => dto.IsDefRate,
                SortSelector = dto => dto.IsDefRate,
                Filterable = true,
                Visible = true
            },
            new RateGridColumnSettings
            {
                Name = "RateTypeName",
                Header = "Тип ставки",
                DisplaySelector = dto => dto.RateTypeName,
                SortSelector = dto => dto.RateTypeName,
                Filterable = false,
                Visible = true
            },
            new RateGridColumnSettings
            {
                Name = "NodeFromNameRu",
                Header = "Отправление",
                DisplaySelector = dto => dto.NodeFromNameRu,
                SortSelector = dto => dto.NodeFromNameRu,
                Filterable = true,
                Visible = true
            },
            new RateGridColumnSettings
            {
                Name = "NodeFromNameEn",
                Header = "Отправление (En)",
                DisplaySelector = dto => dto.NodeFromNameEn,
                SortSelector = dto => dto.NodeFromNameEn,
                Filterable = true,
                Visible = false
            },
            new RateGridColumnSettings
            {
                Name = "ProxyNodeNameRu",
                Header = "Промежуточный",
                DisplaySelector = dto => dto.ProxyNodeNameRu,
                SortSelector = dto => dto.ProxyNodeNameRu,
                Filterable = true,
                Visible = true
            },
            new RateGridColumnSettings
            {
                Name = "ProxyNodeNameEn",
                Header = "Промежуточный (En)", 
                DisplaySelector = dto => dto.ProxyNodeNameEn,
                SortSelector = dto => dto.ProxyNodeNameEn,
                Filterable = true, 
                Visible = false
            },
            new RateGridColumnSettings
            {
                Name = "NodeToNameRu",
                Header = "Назначение",
                DisplaySelector = dto => dto.NodeToNameRu,
                SortSelector = dto => dto.NodeToNameRu,
                Filterable = true,
                Visible = true
            },
            new RateGridColumnSettings
            {
                Name = "NodeToNameEn",
                Header = "Назначение (En)",
                DisplaySelector = dto => dto.NodeToNameEn,
                SortSelector = dto => dto.NodeToNameEn,
                Filterable = true,
                Visible = false
            },
            new RateGridColumnSettings
            {
                Name = "ProductGroupName",
                Header = "Группа продуктов",
                DisplaySelector = dto => dto.ProductGroupName,
                SortSelector = dto => dto.ProductGroupName,
                Filterable = true,
                Visible = true
            },
            new RateGridColumnSettings
            {
                Name = "StartDate",
                Header = "Начало",
                DisplaySelector = dto => dto.StartDate.ToShortDateString(),
                SortSelector = dto => dto.StartDate,
                Filterable = true,
                Visible = true
            },
            new RateGridColumnSettings
            {
                Name = "EndDate",
                Header = "Окончание",
                DisplaySelector = dto => dto.EndDate.ToShortDateString(),
                SortSelector = dto => dto.EndDate,
                Filterable = true,
                Visible = true
            },
            new RateGridColumnSettings
            {
                Name = "CurrencyCode",
                Header = "Валюта",
                DisplaySelector = dto => dto.CurrencyCode,
                SortSelector = dto => dto.CurrencyCode,
                Filterable = false,
                Visible = true
            },
            new RateGridColumnSettings
            {
                Name = "TotalCostTon",
                Header = "За тонну",
                DisplaySelector = dto => dto.TotalCostTon,
                SortSelector = dto => dto.TotalCostTon,
                Filterable = false,
                Visible = true
            },
            new RateGridColumnSettings
            {
                Name = "TotalCostTransport",
                Header = "За ТС",
                DisplaySelector = dto => dto.TotalCostTransport,
                SortSelector = dto => dto.TotalCostTransport,
                Filterable = false,
                Visible = false
            },
            new RateGridColumnSettings
            {
                Name = "CreationDate",
                Header = "Дата создания",
                DisplaySelector = dto => dto.CreationDate,
                SortSelector = dto => dto.CreationDate,
                Filterable = true,
                Visible = false
            },
            new RateGridColumnSettings
            {
                Name = "LastChangeDate",
                Header = "Дата изменения",
                DisplaySelector = dto => dto.LastChangeDate,
                SortSelector = dto => dto.LastChangeDate,
                Filterable = true,
                Visible = false
            }
        ];
        var fullSettings = arr.ToDictionary(s => s.Name, s => s);
      //  return fullSettings;
        var loadedSettings = await localStorage.GetItemAsync<Dictionary<string, bool>>(StorageKey) ?? new Dictionary<string, bool>();
        foreach (var loadedSetting in loadedSettings)
        {
            if (fullSettings.TryGetValue(loadedSetting.Key, out var currentFullSetting))
            {
                currentFullSetting.Visible = loadedSetting.Value;
            }
        }
        return fullSettings;
    }

    public async Task ResetRatesGridColumnSettingsAsync()
    {
        await PostRatesGridColumnSettingsAsync(new Dictionary<string, RateGridColumnSettings>());
    }

    public async Task PostRatesGridColumnSettingsAsync(Dictionary<string, RateGridColumnSettings> fullSettings)
    {
        var visibilitySettings = fullSettings.Values.ToDictionary(v => v.Name, v => v.Visible);
        await localStorage.SetItemAsync(StorageKey, visibilitySettings);
    }


    public async Task<GridDataProviderResult<RateDto>> GetRatesAsync(GridDataProviderRequest<RateDto> request)
    {
        string? sortString = null;
        var sortDirection = SortDirection.None;

        if (request.Sorting is not null && request.Sorting.Any())
        {
            // Note: Multi column sorting is not supported at this moment
            sortString = request.Sorting.FirstOrDefault()!.SortString;
            sortDirection = request.Sorting.FirstOrDefault()!.SortDirection;
        }
        var result = await GetRatesFromDbAsync(request.Filters ?? new List<FilterItem>(), request.PageNumber, request.PageSize, sortString, sortDirection, request.CancellationToken);
        return await Task.FromResult(new GridDataProviderResult<RateDto>
        {
            Data = result.Item1,
            TotalCount = result.Item2
        });
    }

    private async Task<Tuple<IEnumerable<RateDto>, int>> GetRatesFromDbAsync(IEnumerable<FilterItem> filters, int pageNumber, int pageSize, string? sortKey, SortDirection sortDirection, CancellationToken cancellationToken = default)
    {
        try
        {
            await using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync(cancellationToken);

            var parameters = new DynamicParameters();
            parameters.Add("PageNumber", pageNumber);
            parameters.Add("PageSize", pageSize);
            parameters.Add("SortKey", sortKey);
            parameters.Add("SortDirection", sortDirection == SortDirection.Descending ? "DESC" : "ASC");
            parameters.Add("PageNumber", pageNumber);
            var jsonFilter = JsonConvert.SerializeObject(filters, new JsonSerializerSettings
            {
                StringEscapeHandling = StringEscapeHandling.Default
            });
            parameters.Add("Filter", jsonFilter);

            await using var multi = await connection.QueryMultipleAsync(
                "dbo.GetTransportRatesByFilters_v3",
                parameters,
                commandType: CommandType.StoredProcedure);

            var rates = (await multi.ReadAsync<RateDto>()).ToList();
            var count = (await multi.ReadFirstOrDefaultAsync<RateCountDto>())?.TotalCount ?? 0;
            return new(rates, count);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}
