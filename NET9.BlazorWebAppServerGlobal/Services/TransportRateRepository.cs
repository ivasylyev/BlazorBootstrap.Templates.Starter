using Dapper;
using NET9.BlazorWebAppServerGlobal.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace NET9.BlazorWebAppServerGlobal.Services
{


    public class TransportRateRepository
    {
        private readonly string _connectionString;

        public TransportRateRepository(string connectionString)
        {
            _connectionString = connectionString;
        }


        public async Task<IEnumerable<TransportRateDto>> GetRatesByFiltersAsync(
            string nodeFromNameEn = null,
            string nodeFromNameRu = null,
            string proxyNodeNameEn = null,
            string proxyNodeNameRu = null,
            string nodeToNameEn = null,
            string nodeToNameRu = null,
            string rateTypeName = null,
            string productGroupName = null)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@NodeFromNameEn", nodeFromNameEn);
            parameters.Add("@NodeFromNameRu", nodeFromNameRu);
            parameters.Add("@ProxyNodeNameEn", proxyNodeNameEn);
            parameters.Add("@ProxyNodeNameRu", proxyNodeNameRu);
            parameters.Add("@NodeToNameEn", nodeToNameEn);
            parameters.Add("@NodeToNameRu", nodeToNameRu);
            parameters.Add("@RateTypeName", rateTypeName);
            parameters.Add("@ProductGroupName", productGroupName);

            using var connection = new SqlConnection(_connectionString);
            return await connection.QueryAsync<TransportRateDto>(
                "dbo.GetTransportRatesByFilters",
                parameters,
                commandType: CommandType.StoredProcedure);
        }


        public async Task<IEnumerable<TransportRateDto>> GetTransportRatesAsync(
            string nodeFromNameEn = null,
            string nodeFromNameRu = null,
            string proxyNodeNameEn = null,
            string proxyNodeNameRu = null,
            string nodeToNameEn = null,
            string nodeToNameRu = null,
            string rateTypeName = null,
            string productGroupName = null)
        {
            const string query = @"SELECT TOP 30 
    r.Code AS RateCode,
    r.IsDefRate AS IsDefRate,
    rt.Code AS RateTypeCode,
    rt.[Name] AS RateTypeName,
    nf.Code AS NodeFromCode,
    nf.a_2123 AS NodeFromNameEn,
    nf.a_1020 AS NodeFromNameRu,
    np.a_2123 AS ProxyNodeCode,
    np.a_1020 AS ProxyNodeNameEn,
    nt.a_2123 AS ProxyNodeNameRu,
    nt.a_1020 AS NodeToCode,
    tk.NameEnRu AS NodeToNameEn,
    tt.NameEnRu AS NodeToNameRu,
    r.StartDate,
    r.EndDate,
    r.CreationDate,
    r.LastChangeDate,
    pg.Code AS ProductGroupCode,
    pg.[Name] AS ProductGroupName,
    cn.ShortNameEGRUL,
    r.TotalCostTon,
    r.TotalCostTransport,
    cur.Code AS CurrencyCode,
    cur.[Name] AS CurrencyName

FROM vw_TransportRate r (NOLOCK)
JOIN PrimitiveEntityData_1014 nf (NOLOCK) ON r.NodeFrom = nf.PrimitiveEntityItemId
JOIN PrimitiveEntityData_1014 nt (NOLOCK) ON r.NodeTo = nt.PrimitiveEntityItemId
LEFT JOIN PrimitiveEntityData_1014 np (NOLOCK) ON r.ProxyNode = np.Id
JOIN vw_ProductGroup pg (NOLOCK) ON r.ProductGroup = pg.Id
JOIN vw_RateType rt (NOLOCK) ON r.RateType = rt.Id
JOIN vw_TransportKind tk (NOLOCK) ON r.TransportKind = tk.Id
JOIN vw_TransportType_level_3 tt (NOLOCK) ON r.TransportType = tt.Id
LEFT JOIN vw_Contractor cn (NOLOCK) ON r.Counterparty = cn.Id
JOIN vw_Currency cur (NOLOCK) ON r.CurrencyStandard = cur.Id

WHERE
    (@NodeFromNameEn IS NULL OR CONTAINS(nf.a_2123, @NodeFromNameEn)) AND
    (@NodeFromNameRu IS NULL OR CONTAINS(nf.a_1020, @NodeFromNameRu)) AND
    (@ProxyNodeNameEn IS NULL OR CONTAINS(np.a_2123, @ProxyNodeNameEn)) AND
    (@ProxyNodeNameRu IS NULL OR CONTAINS(np.a_1020, @ProxyNodeNameRu)) AND
    (@NodeToNameEn IS NULL OR CONTAINS(nt.a_2123, @NodeToNameEn)) AND
    (@NodeToNameRu IS NULL OR CONTAINS(nt.a_1020, @NodeToNameRu)) AND
    (@RateTypeName IS NULL OR rt.[Name] LIKE @RateTypeName) AND
    (@ProductGroupName IS NULL OR pg.[Name] LIKE @ProductGroupName)

ORDER BY StartDate, EndDate DESC
";

            var parameters = new DynamicParameters();

            // CONTAINS parameters — строго оборачиваем в кавычки!
            parameters.Add("NodeFromNameEn", FormatContains(nodeFromNameEn));
            parameters.Add("NodeFromNameRu", FormatContains(nodeFromNameRu));
            parameters.Add("ProxyNodeNameEn", FormatContains(proxyNodeNameEn));
            parameters.Add("ProxyNodeNameRu", FormatContains(proxyNodeNameRu));
            parameters.Add("NodeToNameEn", FormatContains(nodeToNameEn));
            parameters.Add("NodeToNameRu", FormatContains(nodeToNameRu));

            // LIKE parameters
            parameters.Add("RateTypeName", string.IsNullOrWhiteSpace(rateTypeName) ? null : $"%{rateTypeName}%");
            parameters.Add("ProductGroupName", string.IsNullOrWhiteSpace(productGroupName) ? null : $"%{productGroupName}%");

            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            var result = await connection.QueryAsync<TransportRateDto>(query, parameters);
            return result;
        }

        private string FormatContains(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return null;

            // Добавляем звёздочку и оборачиваем в двойные кавычки
            return $"\"{input.Trim()}*\"";
        }

        
    }

}
