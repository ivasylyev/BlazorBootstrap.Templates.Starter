using System.Data;
using System.Data.SqlClient;
using System.Linq.Expressions;
using BlazorBootstrap;
using Dapper;
using NET9.BlazorWebAppServerGlobal.Models;

namespace NET9.BlazorWebAppServerGlobal.Services;

public class RatesService : IRatesService
{
  
    private readonly string _connectionString = "Server=S001ITD-0084;Database=mdm;Trusted_Connection=false;User ID=SVT;Password=SVTsrv1!;MultipleActiveResultSets=true;Application Name=mdm-api;Encrypt=False;TrustServerCertificate=True;Max Pool Size=1000;";

    
    public async Task<Tuple<IEnumerable<RateDto>, int>> GetCustomersAsync(IEnumerable<FilterItem> filters, int pageNumber, int pageSize, string? sortKey, SortDirection sortDirection, CancellationToken cancellationToken = default)
    {
        try
        {
            var filtersDict = filters.ToDictionary(f => f.PropertyName, f => f.Value);
           

            var (results, count) = await GetRatesByFiltersAsync(
                pageNumber:pageNumber,
                pageSize:pageSize,
                sortKey:sortKey,
                sortDirection:sortDirection == SortDirection.Descending?"DESC":"ASC",
                filtersDict
            );
            return new(results, count);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }

    }

    public async Task<(List<RateDto> Items, int TotalCount)> GetRatesByFiltersAsync(
        int pageNumber,
        int pageSize,
        string? sortKey,
        string? sortDirection,
        Dictionary<string, string> filters)
    {
        using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();

        var parameters = new DynamicParameters();
        parameters.Add("PageNumber", pageNumber);
        parameters.Add("PageSize", pageSize);
        parameters.Add("SortKey", sortKey);
        parameters.Add("SortDirection", sortDirection);
        parameters.Add("PageNumber", pageNumber);
        foreach (var filter in filters)
        {
            parameters.Add(filter.Key,filter.Value);
        }
       

        using var multi = await connection.QueryMultipleAsync(
            "dbo.GetTransportRatesByFilters",
            parameters,
            commandType: CommandType.StoredProcedure);

        var rates = (await multi.ReadAsync<RateDto>()).ToList();
        var count = (await multi.ReadFirstOrDefaultAsync<RateCountDto>())?.TotalCount ?? 0;

        return (rates, count);
    }

}
