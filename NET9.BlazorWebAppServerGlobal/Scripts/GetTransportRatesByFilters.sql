USE mdm
GO

CREATE OR ALTER PROCEDURE dbo.GetTransportRatesByFilters
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
   WHERE 1=1'
   
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
        SET @CTEs += ', 
        FilteredNodeFromRu AS (
            SELECT Id
            FROM [mdm].[dbo].[TransportRateSnapshot] (NOLOCK)
            WHERE CONTAINS(NodeFromNameRu, N''"' + @NodeFromNameRu + '*"'')
            AND StateId = 1
        )';
        SET @Joins += '
        INNER JOIN FilteredNodeFromRu fnfr ON fnfr.Id = tr.Id';
    END

    IF @NodeFromNameEn <> ''
    BEGIN
        SET @CTEs += ', 
        FilteredNodeFromEn AS (
            SELECT Id
            FROM [mdm].[dbo].[TransportRateSnapshot] (NOLOCK)
            WHERE CONTAINS(NodeFromNameEn, N''"' + @NodeFromNameEn + '*"'')
            AND StateId = 1
        )';
        SET @Joins +=  '
        INNER JOIN FilteredNodeFromEn fnfe ON fnfe.Id = tr.Id';
    END

    IF @ProxyNodeNameEn <> ''
    BEGIN
        SET @CTEs += ', 
        FilteredProxyNodeEn AS (
            SELECT Id
            FROM [mdm].[dbo].[TransportRateSnapshot] (NOLOCK)
            WHERE CONTAINS(ProxyNodeNameEn, N''"' + @ProxyNodeNameEn + '*"'')
            AND StateId = 1
        )';
        SET @Joins += '
        INNER JOIN FilteredProxyNodeEn fpne ON fpne.Id = tr.Id';
    END

    IF @ProxyNodeNameRu <> ''
    BEGIN
        SET @CTEs += ', 
        FilteredProxyNodeRu AS (
            SELECT Id
            FROM [mdm].[dbo].[TransportRateSnapshot] (NOLOCK)
            WHERE CONTAINS(ProxyNodeNameRu, N''"' + @ProxyNodeNameRu + '*"'')
            AND StateId = 1
        )';
        SET @Joins += '
        INNER JOIN FilteredProxyNodeRu fpnr ON fpnr.Id = tr.Id';
    END

    IF @NodeToNameEn <> ''
    BEGIN
        SET @CTEs += ', 
        FilteredNodeToEn AS (
            SELECT Id
            FROM [mdm].[dbo].[TransportRateSnapshot] (NOLOCK)
            WHERE CONTAINS(NodeToNameEn, N''"' + @NodeToNameEn + '*"'')
            AND StateId = 1
        )';
        SET @Joins +=  '
        INNER JOIN FilteredNodeToEn fnte ON fnte.Id = tr.Id';
    END

    IF @NodeToNameRu <> ''
    BEGIN
        SET @CTEs += ', 
        FilteredNodeToRu AS (
            SELECT Id
            FROM [mdm].[dbo].[TransportRateSnapshot] (NOLOCK)
            WHERE CONTAINS(NodeToNameRu, N''"' + @NodeToNameRu + '*"'')
            AND StateId = 1
        )';
        SET @Joins += '
        INNER JOIN FilteredNodeToRu fntr ON fntr.Id = tr.Id';
    
    END


    IF @RateTypeName <> ''
    BEGIN
        SET @CTEs += ', FilteredRateType AS (
            SELECT Id
            FROM [mdm].[dbo].[TransportRateSnapshot] (NOLOCK)
            WHERE CONTAINS(RateTypeName, N''"' + @RateTypeName + '*"'')
            AND StateId = 1
        )';
        SET @Joins += '
        INNER JOIN FilteredRateType frt ON frt.Id = tr.Id';
    END

    IF @ProductGroupName <> ''
    BEGIN
        SET @CTEs += ', FilteredProductGroup AS (
            SELECT Id
            FROM [mdm].[dbo].[TransportRateSnapshot] (NOLOCK)
            WHERE CONTAINS(ProductGroupName, N''"' + @ProductGroupName + '*"'')
            AND StateId = 1
        )';
        SET @Joins += '
        INNER JOIN FilteredProductGroup fpg ON fpg.Id = tr.Id';
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
    --PRINT @both_sql;

    EXEC sp_executesql @both_sql;
END
GO
