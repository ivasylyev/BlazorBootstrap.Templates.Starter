USE mdm
GO

/*

DECLARE @json NVARCHAR(MAX) = N'[
    {"PropertType":null,"PropertyName":"NodeFromNameRu","Value":"каз","Operator":7,"StringComparison":5},
    {"PropertType":null,"PropertyName":"NodeToNameRu","Value":"омск","Operator":7,"StringComparison":5}
]';

SELECT 
    PropertType,
    PropertyName,
    Value,
    Operator,
    StringComparison
FROM OPENJSON(@json)
WITH (
    PropertType      INT            '$.PropertType',
    PropertyName     NVARCHAR(100)  '$.PropertyName',
    Value            NVARCHAR(MAX)  '$.Value',
    Operator         INT            '$.Operator',
    StringComparison INT            '$.StringComparison'
);

*/
CREATE OR ALTER PROCEDURE dbo.GetTransportRatesByFilters_v2
    @PageNumber INT = 1,
    @PageSize INT = 20,

    @SortKey NVARCHAR(100) = NULL,
    @SortDirection NVARCHAR(100) = NULL,

    @NodeFromNameEn NVARCHAR(100) = NULL,
    @NodeFromNameEn_Operator NVARCHAR(100) = NULL,
    @NodeFromNameRu NVARCHAR(100) = NULL,
    @NodeFromNameRu_Operator NVARCHAR(100) = NULL,
    @ProxyNodeNameEn NVARCHAR(100) = NULL,
    @ProxyNodeNameEn_Operator NVARCHAR(100) = NULL,
    @ProxyNodeNameRu NVARCHAR(100) = NULL,
    @ProxyNodeNameRu_Operator NVARCHAR(100) = NULL,
    @NodeToNameEn NVARCHAR(100) = NULL,
    @NodeToNameEn_Operator NVARCHAR(100) = NULL,
    @NodeToNameRu NVARCHAR(100) = NULL,
    @NodeToNameRu_Operator NVARCHAR(100) = NULL,
    @RateTypeName NVARCHAR(100) = NULL,
    @RateTypeName_Operator NVARCHAR(100) = NULL,
    @ProductGroupName NVARCHAR(100) = NULL,
    @ProductGroupName_Operator NVARCHAR(100) = NULL,
    @StartDate NVARCHAR(100) = NULL,
    @StartDate_Operator NVARCHAR(100) = NULL,
    @EndDate NVARCHAR(100) = NULL,
    @EndDate_Operator NVARCHAR(100) = NULL
     
AS
BEGIN
    SET NOCOUNT ON;

    -- Очищаем параметры
    SET @NodeFromNameEn    = ISNULL(LTRIM(RTRIM(@NodeFromNameEn)), '');
    SET @NodeFromNameRu    = ISNULL(LTRIM(RTRIM(@NodeFromNameRu)), '');
    SET @ProxyNodeNameEn   = ISNULL(LTRIM(RTRIM(@ProxyNodeNameEn)), '');
    SET @ProxyNodeNameRu   = ISNULL(LTRIM(RTRIM(@ProxyNodeNameRu)), '');
    SET @NodeToNameEn      = ISNULL(LTRIM(RTRIM(@NodeToNameEn)), '');
    SET @NodeToNameRu      = ISNULL(LTRIM(RTRIM(@NodeToNameRu)), '');
    SET @RateTypeName      = ISNULL(LTRIM(RTRIM(@RateTypeName)), '');
    SET @ProductGroupName  = ISNULL(LTRIM(RTRIM(@ProductGroupName)), '');

    SET @PageNumber = IIF(@PageNumber < 1, 1, @PageNumber);
    SET @PageSize = IIF(@PageSize < 1, 20, @PageSize);

    DECLARE
        @Offset INT = (@PageNumber - 1) * @PageSize,
        @CTEs NVARCHAR(MAX) = '',
        @Joins NVARCHAR(MAX) = '',
        @Filters NVARCHAR(MAX) = '',
        @sqlMain NVARCHAR(MAX),
        @sqlCount NVARCHAR(MAX),
        @both_sql NVARCHAR(MAX);

   
   SET @Filters = ' 
   WHERE     StateId = 1'
   
    IF (ISDATE(@StartDate) = 1)
    BEGIN
        SET @Filters += ISNULL(N'
        AND  tr.StartDate ' + dbo.fn_GetSqlOperator(@StartDate_Operator) -- если функция вернет NULL, весь фильтр обнулится. И это правильное поведение
        +''''+ @StartDate + '''','')
    END
    IF (ISDATE(@EndDate) = 1)
    BEGIN
        SET @Filters +=  ISNULL(N'
        AND  tr.EndDate ' + dbo.fn_GetSqlOperator(@EndDate_Operator) -- если функция вернет NULL, весь фильтр обнулится. И это правильное поведение
        +''''+ @EndDate + '''','')
    END


    IF @NodeFromNameRu <> ''
    BEGIN
        SET @Filters += '
        AND CONTAINS(NodeFromNameRu, N''"' + @NodeFromNameRu + '*"'')';
    END

    IF @NodeFromNameEn <> ''
    BEGIN
        SET @Filters +=  '
        AND CONTAINS(NodeFromNameEn, N''"' + @NodeFromNameEn + '*"'')';
    END

    IF @ProxyNodeNameEn <> ''
    BEGIN
        SET @Filters +=  '
        AND CONTAINS(ProxyNodeNameEn, N''"' + @ProxyNodeNameEn + '*"'')';
    END

    IF @ProxyNodeNameRu <> ''
    BEGIN
        SET @Filters += '
        AND CONTAINS(ProxyNodeNameRu, N''"' + @ProxyNodeNameRu + '*"'')';
    END

    IF @NodeToNameEn <> ''
    BEGIN
        SET @Filters += '
        AND CONTAINS(NodeToNameEn, N''"' + @NodeToNameEn + '*"'')';
    END

    IF @NodeToNameRu <> ''
    BEGIN
        SET @Filters += '
        AND CONTAINS(NodeToNameRu, N''"' + @NodeToNameRu + '*"'')';
    END

    IF @RateTypeName <> ''
    BEGIN
        SET @Filters += '
        AND CONTAINS(RateTypeName, N''"' + @RateTypeName + '*"'')';
    END

    IF @ProductGroupName <> ''
    BEGIN
        SET @Filters += '
        AND  CONTAINS(ProductGroupName, N''"' + @ProductGroupName + '*"'')';
    END

    -- Основной SELECT с подставленными CTE и JOIN'ами
    SET @sqlMain = '
    WITH CTE AS (SELECT 1 AS TST)
    ' + @CTEs + '
    SELECT
        tr.[Id],
        tr.[StateId],
        tr.[Code],
        tr.[IsDefRate],
        CAST(tr.[StartDate] AS DATE) StartDate,
        CAST(tr.[EndDate] AS DATE) EndDate,
        tr.[CreationDate],
        tr.[LastChangeDate],
        tr.[TotalCostTon],
        tr.[TotalCostTransport],
        tr.[RateTypeCode],
        tr.[RateTypeName],
        tr.[NodeFromCode],
        tr.[NodeFromNameEn],
        tr.[NodeFromNameRu],
        tr.[ProxyNodeCode],
        tr.[ProxyNodeNameEn],
        tr.[ProxyNodeNameRu],
        tr.[NodeToCode],
        tr.[NodeToNameEn],
        tr.[NodeToNameRu],
        tr.[TransportKindCode],
        tr.[TransportKindNameRu],
        tr.[TransportTypeCode],
        tr.[TransportTypeNameRu],
        tr.[ProductGroupCode],
        tr.[ProductGroupName],
        tr.[ContractorCode],
        tr.[ContractorEGRUL],
        tr.[CurrencyCode],
        tr.[CurrencyName]
    FROM [mdm].[dbo].[TransportRateSnapshot] tr
    ' + @Joins + ' 
    ' + @Filters + '
    ORDER BY '+ ISNULL(@SortKey + ' ' + @SortDirection + ', ', '')  + '  tr.Id DESC
    OFFSET ' + CAST(@Offset AS NVARCHAR(20)) + ' ROWS
    FETCH NEXT ' + CAST(@PageSize AS NVARCHAR(20)) + ' ROWS ONLY;
    ';

    SET @sqlCount = '
    WITH CTE AS (SELECT 1 AS TST)
    ' + @CTEs + '
    SELECT COUNT(1) AS TotalCount
    FROM [mdm].[dbo].[TransportRateSnapshot] tr
    ' + @Joins + ' 
    ' + @Filters + ';';

    SET @both_sql = @sqlMain + CHAR(13) + @sqlCount;

    -- Для отладки можно раскомментировать:
    PRINT @both_sql;

    EXEC sp_executesql @both_sql;
END
GO
