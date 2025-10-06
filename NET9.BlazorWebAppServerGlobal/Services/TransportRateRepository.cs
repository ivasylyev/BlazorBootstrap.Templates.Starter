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

      

        public async Task<(List<TransportRateDto> Items, int TotalCount)> GetRatesByFiltersAsync(
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


        private string FormatContains(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return null;

            // Добавляем звёздочку и оборачиваем в двойные кавычки
            return $"\"{input.Trim()}*\"";
        }

        
    }

}
