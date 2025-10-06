using System.Linq.Expressions;
using BlazorBootstrap;
using NET9.BlazorWebAppServerGlobal.Models;

namespace NET9.BlazorWebAppServerGlobal.Services;

public class CustomerService : ICustomerService
{
    private readonly HttpClient _httpClient;

    public CustomerService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<IEnumerable<TransportRateDto>> GetCustomersAsync(FilterItem filter, CancellationToken cancellationToken = default)
    {
        var rateDtos = await _httpClient.GetFromJsonAsync<TransportRateDto[]>("sample-data/customer/customer.json", cancellationToken);
        if (rateDtos is null)
            return Enumerable.Empty<TransportRateDto>();

        var parameterExpression = Expression.Parameter(typeof(TransportRateDto)); // second param optional
        var lambda = ExpressionExtensions.GetExpressionDelegate<TransportRateDto>(parameterExpression, filter);
        return rateDtos.Where(lambda!.Compile()).OrderBy(rateDto => rateDto.RateCode);
    }

    public async Task<Tuple<IEnumerable<TransportRateDto>, int>> GetCustomersAsync(IEnumerable<FilterItem> filters, int pageNumber, int pageSize, string sortKey, SortDirection sortDirection, CancellationToken cancellationToken = default)
    {
        var max = 1000;
        var rateDtos = new List<TransportRateDto>(max);
        try
        {
            var repository = new TransportRateRepository("Server=S001ITD-0084;Database=mdm_prev;Trusted_Connection=false;User ID=SVT;Password=SVTsrv1!;MultipleActiveResultSets=true;Application Name=mdm-api;Encrypt=False;TrustServerCertificate=True;Max Pool Size=1000;");
            var results = await repository.GetRatesByFiltersAsync(
                nodeFromNameRu: "тобольск",
                productGroupName: "каучук"
            );
            rateDtos = results.ToList();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
       

        // apply filters
        if (filters is not null)
        {
            filters = filters.ToList();
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

                rateDtos = rateDtos.Where(lambda!.Compile()).ToList();
            }
        }
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
        return new(rateDtos, rateDtos.Count());
    }
}
