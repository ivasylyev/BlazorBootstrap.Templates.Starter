using BlazorBootstrap;
using Dapper;
using NET9.BlazorWebAppServerGlobal.Models;
using System.Data;
using System.Data.SqlClient;
using System.Text.Json;
using Newtonsoft.Json;
namespace NET9.BlazorWebAppServerGlobal.Services;

public class RatesService : IRatesService
{
  
    private readonly string _connectionString = "Server=S001ITD-0084;Database=mdm;Trusted_Connection=false;User ID=SVT;Password=SVTsrv1!;MultipleActiveResultSets=true;Application Name=mdm-api;Encrypt=False;TrustServerCertificate=True;Max Pool Size=1000;";


public async Task<Tuple<IEnumerable<RateDto>, int>> GetCustomersAsync(IEnumerable<FilterItem> filters, int pageNumber, int pageSize, string? sortKey, SortDirection sortDirection, CancellationToken cancellationToken = default)
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
