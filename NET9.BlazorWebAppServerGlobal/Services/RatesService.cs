using System.Data;
using System.Data.SqlClient;
using Microsoft.Extensions.Options;
using BlazorBootstrap;
using Blazored.LocalStorage;
using Dapper;
using NET9.BlazorWebAppServerGlobal.Models;
using Newtonsoft.Json;

namespace NET9.BlazorWebAppServerGlobal.Services;

public class RatesService(ILocalStorageService localStorage, IOptions<DatabaseOptions> options) : GridSettingsServiceBase<RateDto>(localStorage), IRatesService
{
    private readonly string connectionString = options.Value.MdmDb;

    protected override string StorageKey => "RatesGridColumnSettings";

    public Task<GridSettings<RateDto>> GetRatesGridColumnSettingsAsync()
    {
        return GetGridSettingsAsync();
    }

    public Task PostRatesGridColumnSettingsAsync(GridSettings<RateDto> settings)
    {
        return SaveGridSettingsAsync(settings);
    }

    public Task ResetRatesGridColumnSettingsAsync()
    {
        return ResetGridSettingsAsync();
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

        var result = await GetRatesFromDbAsync(request.Filters ?? new List<FilterItem>(), request.PageNumber, request.PageSize, sortString, sortDirection,
            request.CancellationToken);
        return await Task.FromResult(new GridDataProviderResult<RateDto>
        {
            Data = result.Item1,
            TotalCount = result.Item2
        });
    }


    protected override List<GridColumnSetting<RateDto>> GetDefaultSettings()
    {
        return
        [
            new GridColumnSetting<RateDto>
            {
                Name = "Code",
                Header = "Code",
                DisplaySelector = dto => dto.Code,
                SortSelector = dto => dto.Code,
                Filterable = true,
                Visible = true
            },
            new GridColumnSetting<RateDto>
            {
                Name = "IsDefRate",
                Header = "Дефлятор",
                DisplaySelector = dto => dto.IsDefRate,
                SortSelector = dto => dto.IsDefRate,
                Filterable = true,
                Visible = true
            },
            new GridColumnSetting<RateDto>
            {
                Name = "RateTypeName",
                Header = "Тип ставки",
                DisplaySelector = dto => dto.RateTypeName,
                SortSelector = dto => dto.RateTypeName,
                Filterable = false,
                Visible = true
            },
            new GridColumnSetting<RateDto>
            {
                Name = "NodeFromNameRu",
                Header = "Отправление",
                DisplaySelector = dto => dto.NodeFromNameRu,
                SortSelector = dto => dto.NodeFromNameRu,
                Filterable = true,
                Visible = true
            },
            new GridColumnSetting<RateDto>
            {
                Name = "NodeFromNameEn",
                Header = "Отправление (En)",
                DisplaySelector = dto => dto.NodeFromNameEn,
                SortSelector = dto => dto.NodeFromNameEn,
                Filterable = true,
                Visible = false
            },
            new GridColumnSetting<RateDto>
            {
                Name = "ProxyNodeNameRu",
                Header = "Промежуточный",
                DisplaySelector = dto => dto.ProxyNodeNameRu,
                SortSelector = dto => dto.ProxyNodeNameRu,
                Filterable = true,
                Visible = true
            },
            new GridColumnSetting<RateDto>
            {
                Name = "ProxyNodeNameEn",
                Header = "Промежуточный (En)",
                DisplaySelector = dto => dto.ProxyNodeNameEn,
                SortSelector = dto => dto.ProxyNodeNameEn,
                Filterable = true,
                Visible = false
            },
            new GridColumnSetting<RateDto>
            {
                Name = "NodeToNameRu",
                Header = "Назначение",
                DisplaySelector = dto => dto.NodeToNameRu,
                SortSelector = dto => dto.NodeToNameRu,
                Filterable = true,
                Visible = true
            },
            new GridColumnSetting<RateDto>
            {
                Name = "NodeToNameEn",
                Header = "Назначение (En)",
                DisplaySelector = dto => dto.NodeToNameEn,
                SortSelector = dto => dto.NodeToNameEn,
                Filterable = true,
                Visible = false
            },
            new GridColumnSetting<RateDto>
            {
                Name = "ProductGroupName",
                Header = "Группа продуктов",
                DisplaySelector = dto => dto.ProductGroupName,
                SortSelector = dto => dto.ProductGroupName,
                Filterable = true,
                Visible = true
            },
            new GridColumnSetting<RateDto>
            {
                Name = "StartDate",
                Header = "Начало",
                DisplaySelector = dto => dto.StartDate.ToShortDateString(),
                SortSelector = dto => dto.StartDate,
                Filterable = true,
                Visible = true
            },
            new GridColumnSetting<RateDto>
            {
                Name = "EndDate",
                Header = "Окончание",
                DisplaySelector = dto => dto.EndDate.ToShortDateString(),
                SortSelector = dto => dto.EndDate,
                Filterable = true,
                Visible = true
            },
            new GridColumnSetting<RateDto>
            {
                Name = "CurrencyCode",
                Header = "Валюта",
                DisplaySelector = dto => dto.CurrencyCode,
                SortSelector = dto => dto.CurrencyCode,
                Filterable = false,
                Visible = true
            },
            new GridColumnSetting<RateDto>
            {
                Name = "TotalCostTon",
                Header = "За тонну",
                DisplaySelector = dto => dto.TotalCostTon,
                SortSelector = dto => dto.TotalCostTon,
                Filterable = false,
                Visible = true
            },
            new GridColumnSetting<RateDto>
            {
                Name = "TotalCostTransport",
                Header = "За ТС",
                DisplaySelector = dto => dto.TotalCostTransport,
                SortSelector = dto => dto.TotalCostTransport,
                Filterable = false,
                Visible = false
            },
            new GridColumnSetting<RateDto>
            {
                Name = "CreationDate",
                Header = "Дата создания",
                DisplaySelector = dto => dto.CreationDate,
                SortSelector = dto => dto.CreationDate,
                Filterable = true,
                Visible = false
            },
            new GridColumnSetting<RateDto>
            {
                Name = "LastChangeDate",
                Header = "Дата изменения",
                DisplaySelector = dto => dto.LastChangeDate,
                SortSelector = dto => dto.LastChangeDate,
                Filterable = true,
                Visible = false
            }
        ];
    }

    private async Task<Tuple<IEnumerable<RateDto>, int>> GetRatesFromDbAsync(IEnumerable<FilterItem> filters, int pageNumber, int pageSize, string? sortKey,
        SortDirection sortDirection, CancellationToken cancellationToken = default)
    {
        try
        {
            await using var connection = new SqlConnection(connectionString);
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
            return new Tuple<IEnumerable<RateDto>, int>(rates, count);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}