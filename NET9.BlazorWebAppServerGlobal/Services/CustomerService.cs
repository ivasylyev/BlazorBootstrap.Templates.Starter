using System.Data;
using System.Data.SqlClient;
using System.Linq.Expressions;
using BlazorBootstrap;
using Dapper;
using NET9.BlazorWebAppServerGlobal.Models;

namespace NET9.BlazorWebAppServerGlobal.Services;

public class CustomerService : ICustomerService
{
  
    private readonly string _connectionString = "Server=S001ITD-0084;Database=mdm_prev;Trusted_Connection=false;User ID=SVT;Password=SVTsrv1!;MultipleActiveResultSets=true;Application Name=mdm-api;Encrypt=False;TrustServerCertificate=True;Max Pool Size=1000;";

    
    public async Task<Tuple<IEnumerable<TransportRateDto>, int>> GetCustomersAsync(IEnumerable<FilterItem> filters, int pageNumber, int pageSize, string? sortKey, SortDirection sortDirection, CancellationToken cancellationToken = default)
    {
        try
        {
            var (results, count) = await GetRatesByFiltersAsync(
                pageNumber:pageNumber,
                pageSize:pageSize,
                sortKey:sortKey,
                sortDirection:sortDirection == SortDirection.Descending?"DESC":"ASC"
                // nodeFromNameRu: "тобольск",
               // productGroupName: "каучук"
            );
            return new(results, count);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
       

        /*
        // apply filters
        if (filters is not null)
        {
            if (filters.Any())
            {
                var parameterExpression = Expression.Parameter(typeof(TransportRateDto)); // second param optional
                Expression<Func<TransportRateDto, bool>>? lambda = null;

                foreach (var filter in filters)
                {
                    if (lambda is null)
                        lambda = ExpressionExtensions.GetExpressionDelegate<TransportRateDto>(parameterExpression, filter)!;
                    else
                        lambda = lambda.And(ExpressionExtensions.GetExpressionDelegate<TransportRateDto>(parameterExpression, filter)!);
                }

            filters = filters.ToList();
                rateDtos = rateDtos.Where(lambda!.Compile()).ToList();
            }
        }
        */
        /*
        // apply sorting then paging
        if (string.IsNullOrEmpty(sortKey) || sortDirection == SortDirection.None)
        {
            return new(rateDtos.Skip((pageNumber - 1) * pageSize).Take(pageSize), rateDtos.Count());
        }
        else if (sortKey == "CustomerId")
        {
            if (sortDirection == SortDirection.Ascending)
                return new(rateDtos.OrderBy(e => e.CustomerId).Skip((pageNumber - 1) * pageSize).Take(pageSize), rateDtos.Count());
            else if (sortDirection == SortDirection.Descending)
                return new(rateDtos.OrderByDescending(e => e.CustomerId).Skip((pageNumber - 1) * pageSize).Take(pageSize), rateDtos.Count());
        }
        else if (sortKey == "CustomerName")
        {
            if (sortDirection == SortDirection.Ascending)
                return new(rateDtos.OrderBy(e => e.CustomerName).Skip((pageNumber - 1) * pageSize).Take(pageSize), rateDtos.Count());
            else if (sortDirection == SortDirection.Descending)
                return new(rateDtos.OrderByDescending(e => e.CustomerName).Skip((pageNumber - 1) * pageSize).Take(pageSize), rateDtos.Count());
        }
        else if (sortKey == "Phone")
        {
            if (sortDirection == SortDirection.Ascending)
                return new(rateDtos.OrderBy(e => e.Phone).Skip((pageNumber - 1) * pageSize).Take(pageSize), rateDtos.Count());
            else if (sortDirection == SortDirection.Descending)
                return new(rateDtos.OrderByDescending(e => e.Phone).Skip((pageNumber - 1) * pageSize).Take(pageSize), rateDtos.Count());
        }
        else if (sortKey == "Email")
        {
            if (sortDirection == SortDirection.Ascending)
                return new(rateDtos.OrderBy(e => e.Email).Skip((pageNumber - 1) * pageSize).Take(pageSize), rateDtos.Count());
            else if (sortDirection == SortDirection.Descending)
                return new(rateDtos.OrderByDescending(e => e.Email).Skip((pageNumber - 1) * pageSize).Take(pageSize), rateDtos.Count());
        }
        else if (sortKey == "Address")
        {
            if (sortDirection == SortDirection.Ascending)
                return new(rateDtos.OrderBy(e => e.Address).Skip((pageNumber - 1) * pageSize).Take(pageSize), rateDtos.Count());
            else if (sortDirection == SortDirection.Descending)
                return new(rateDtos.OrderByDescending(e => e.Address).Skip((pageNumber - 1) * pageSize).Take(pageSize), rateDtos.Count());
        }
        else if (sortKey == "PostalZip")
        {
            if (sortDirection == SortDirection.Ascending)
                return new(rateDtos.OrderBy(e => e.PostalZip).Skip((pageNumber - 1) * pageSize).Take(pageSize), rateDtos.Count());
            else if (sortDirection == SortDirection.Descending)
                return new(rateDtos.OrderByDescending(e => e.PostalZip).Skip((pageNumber - 1) * pageSize).Take(pageSize), rateDtos.Count());
        }
        else if (sortKey == "Country")
        {
            if (sortDirection == SortDirection.Ascending)
                return new(rateDtos.OrderBy(e => e.Country).Skip((pageNumber - 1) * pageSize).Take(pageSize), rateDtos.Count());
            else if (sortDirection == SortDirection.Descending)
                return new(rateDtos.OrderByDescending(e => e.Country).Skip((pageNumber - 1) * pageSize).Take(pageSize), rateDtos.Count());
        }
        else
            return new(rateDtos.Skip((pageNumber - 1) * pageSize).Take(pageSize), rateDtos.Count());
        */

    }

    public async Task<(List<TransportRateDto> Items, int TotalCount)> GetRatesByFiltersAsync(
        int pageNumber,
        int pageSize,
        string? sortKey,
        string? sortDirection,
        string? nodeFromNameEn = null,
        string? nodeFromNameRu = null,
        string? proxyNodeNameEn = null,
        string? proxyNodeNameRu = null,
        string? nodeToNameEn = null,
        string? nodeToNameRu = null,
        string? rateTypeName = null,
        string? productGroupName = null)
    {
        using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();

        var parameters = new DynamicParameters();
        parameters.Add("PageNumber", pageNumber);
        parameters.Add("PageSize", pageSize);
        parameters.Add("SortKey", sortKey);
        parameters.Add("SortDirection", sortDirection);
        parameters.Add("PageNumber", pageNumber);
        parameters.Add("NodeFromNameEn", nodeFromNameEn);
        parameters.Add("NodeFromNameRu", nodeFromNameRu);
        parameters.Add("ProxyNodeNameEn", proxyNodeNameEn);
        parameters.Add("ProxyNodeNameRu", proxyNodeNameRu);
        parameters.Add("NodeToNameEn", nodeToNameEn);
        parameters.Add("NodeToNameRu", nodeToNameRu);
        parameters.Add("RateTypeName", rateTypeName);
        parameters.Add("ProductGroupName", productGroupName);

        using var multi = await connection.QueryMultipleAsync(
            "dbo.GetTransportRatesByFilters",
            parameters,
            commandType: CommandType.StoredProcedure);

        var rates = (await multi.ReadAsync<TransportRateDto>()).ToList();
        var count = (await multi.ReadFirstOrDefaultAsync<TransportRateCountDto>())?.TotalCount ?? 0;

        return (rates, count);
    }

}
