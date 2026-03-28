using BlazorBootstrap;
using Microsoft.Extensions.Options;
using NET9.BlazorWebAppServerGlobal.Models.Config;
using NET9.BlazorWebAppServerGlobal.Models.Dto;
using NET9.BlazorWebAppServerGlobal.Services.Shared;

namespace NET9.BlazorWebAppServerGlobal.Services.Rates;

public class RatesDataService(
    IOptions<DatabaseOptions> options,
    ILogger<RatesDataService> logger)
    : BaseGridDataService<RateDto>(options, logger), IRatesDataService
{
    protected override string StoredProcedureName
        => "dbo.GetTransportRates";

    public async Task<GridDataProviderResult<RateDto>> GetRatesAsync(GridDataProviderRequest<RateDto> request)
    {
        return await GetDataAsync(request);
    }
}