using BlazorBootstrap;
using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using NET9.BlazorWebAppServerGlobal.Models.Config;
using NET9.BlazorWebAppServerGlobal.Models.Dto;
using NET9.BlazorWebAppServerGlobal.Utils;
using Newtonsoft.Json;
using System.Data;
using System.Data.SqlClient;

namespace NET9.BlazorWebAppServerGlobal.Services.Rates;

public class RatesDataService(IOptions<DatabaseOptions> options, ILogger<RatesDataService> logger) : IRatesDataService
{
    private readonly string connectionString = options.Value.MdmDb;
    

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

            var loggingConnection = new DbConnectionLogDecorator(connection, logger);
            await using var multi = await loggingConnection.QueryMultipleAsync(
                "dbo.GetTransportRatesByFilters_v3",
                parameters,
                CommandType.StoredProcedure);

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