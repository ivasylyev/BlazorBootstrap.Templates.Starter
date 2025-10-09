use mdm_prev
GO


  CREATE OR ALTER PROCEDURE dbo.GetTransportRatesByFilters
    @PageNumber INT = 1,
    @PageSize INT = 20,

    @SortKey NVARCHAR(100) = NULL,
    @SortDirection NVARCHAR(100) = NULL,

    @NodeFromNameEn NVARCHAR(100) = NULL,
    @NodeFromNameRu NVARCHAR(100) = NULL,
    @ProxyNodeNameEn NVARCHAR(100) = NULL,
    @ProxyNodeNameRu NVARCHAR(100) = NULL,
    @NodeToNameEn NVARCHAR(100) = NULL,
    @NodeToNameRu NVARCHAR(100) = NULL,
    @RateTypeName NVARCHAR(100) = NULL,
    @ProductGroupName NVARCHAR(100) = NULL


AS
BEGIN
    SET NOCOUNT ON;

    DECLARE 
        @sql NVARCHAR(MAX) = '',
        @sqlCount NVARCHAR(MAX) = '',
        @where NVARCHAR(MAX) = 'WHERE 1=1',
        @order NVARCHAR(MAX) = ' r.Code DESC',
        @joins NVARCHAR(MAX),
        @joinsCount  NVARCHAR(MAX),
        @Offset INT;

    -- Очистка параметров от пробелов
    SET @NodeFromNameEn    = LTRIM(RTRIM(@NodeFromNameEn));
    SET @NodeFromNameRu    = LTRIM(RTRIM(@NodeFromNameRu));
    SET @ProxyNodeNameEn   = LTRIM(RTRIM(@ProxyNodeNameEn));
    SET @ProxyNodeNameRu   = LTRIM(RTRIM(@ProxyNodeNameRu));
    SET @NodeToNameEn      = LTRIM(RTRIM(@NodeToNameEn));
    SET @NodeToNameRu      = LTRIM(RTRIM(@NodeToNameRu));
    SET @RateTypeName      = LTRIM(RTRIM(@RateTypeName));
    SET @ProductGroupName  = LTRIM(RTRIM(@ProductGroupName));

    -- Параметры OFFSET
    SET @PageNumber = IIF(@PageNumber < 1, 1, @PageNumber);
    SET @PageSize = IIF(@PageSize < 1, 20, @PageSize);
    SET @Offset = (@PageNumber - 1) * @PageSize;


     SET @joinsCount = '
    FROM vw_TransportRate r (NOLOCK)
    '

 
  


    -- CONTAINS filters
    IF ISNULL(@NodeFromNameEn, '') <> '' OR  ISNULL(@NodeFromNameEn, '') <> ''
    BEGIN
        SET @joinsCount += ' JOIN PrimitiveEntityData_1014 nf (NOLOCK) ON r.NodeFrom = nf.PrimitiveEntityItemId 
        ';   
    END
    IF ISNULL(@NodeFromNameEn, '') <> ''
    BEGIN
        SET @joinsCount += ' JOIN PrimitiveEntityData_1014 nf (NOLOCK) ON r.NodeFrom = nf.PrimitiveEntityItemId 
        ';
        SET @where += ' AND CONTAINS(nf.a_2123, @NodeFromNameEn)';
        SET @NodeFromNameEn = '"' + @NodeFromNameEn + '*"';
    END

    IF ISNULL(@NodeFromNameRu, '') <> ''
    BEGIN
        SET @where += ' AND CONTAINS(nf.a_1020, @NodeFromNameRu)';
        SET @NodeFromNameRu = '"' + @NodeFromNameRu + '*"';
    END

    IF ISNULL(@ProxyNodeNameEn, '') <> '' OR  ISNULL(@ProxyNodeNameRu, '') <> ''
    BEGIN
        SET @joinsCount += ' LEFT JOIN PrimitiveEntityData_1014 np (NOLOCK) ON r.ProxyNode = np.PrimitiveEntityItemId 
        '   
    END
    IF ISNULL(@ProxyNodeNameEn, '') <> ''
    BEGIN
        SET @where += ' AND CONTAINS(np.a_2123, @ProxyNodeNameEn)';
        SET @ProxyNodeNameEn = '"' + @ProxyNodeNameEn + '*"';
    END

    IF ISNULL(@ProxyNodeNameRu, '') <> ''
    BEGIN
        SET @where += ' AND CONTAINS(np.a_1020, @ProxyNodeNameRu)';
        SET @ProxyNodeNameRu = '"' + @ProxyNodeNameRu + '*"';
    END
    
    IF ISNULL(@NodeToNameEn, '') <> '' OR  ISNULL(@NodeToNameRu, '') <> ''
    BEGIN
        SET @joinsCount += ' JOIN PrimitiveEntityData_1014 nt (NOLOCK) ON r.NodeTo = nt.PrimitiveEntityItemId 
        ';   
    END
    IF ISNULL(@NodeToNameEn, '') <> ''
    BEGIN
        SET @where += ' AND CONTAINS(nt.a_2123, @NodeToNameEn)';
        SET @NodeToNameEn = '"' + @NodeToNameEn + '*"';
    END

    IF ISNULL(@NodeToNameRu, '') <> ''
    BEGIN
        SET @where += ' AND CONTAINS(nt.a_1020, @NodeToNameRu)';
        SET @NodeToNameRu = '"' + @NodeToNameRu + '*"';
    END

    -- LIKE filters
    IF ISNULL(@RateTypeName, '') <> ''
    BEGIN
        SET @joinsCount += ' JOIN vw_RateType rt (NOLOCK) ON r.RateType = rt.Id '; 
      
        SET @where += ' AND rt.[Name] LIKE @RateTypeName';
        SET @RateTypeName = '%' + @RateTypeName + '%';
    END

    IF ISNULL(@ProductGroupName, '') <> ''
    BEGIN
        SET @joinsCount += ' JOIN vw_ProductGroup pg (NOLOCK) ON r.ProductGroup = pg.Id '; 
        
        SET @where += ' AND pg.[Name] LIKE @ProductGroupName';
        SET @ProductGroupName = '%' + @ProductGroupName + '%';
    END

    -- JOINs
    SET @joins = '
    FROM vw_TransportRate r (NOLOCK)
    JOIN PrimitiveEntityData_1014 nf (NOLOCK) ON r.NodeFrom = nf.PrimitiveEntityItemId
    JOIN PrimitiveEntityData_1014 nt (NOLOCK) ON r.NodeTo = nt.PrimitiveEntityItemId
    LEFT JOIN PrimitiveEntityData_1014 np (NOLOCK) ON r.ProxyNode = np.PrimitiveEntityItemId
    JOIN vw_ProductGroup pg (NOLOCK) ON r.ProductGroup = pg.Id
    JOIN vw_RateType rt (NOLOCK) ON r.RateType = rt.Id
    JOIN vw_TransportKind tk (NOLOCK) ON r.TransportKind = tk.Id
    JOIN vw_TransportType_level_3 tt (NOLOCK) ON r.TransportType = tt.Id
    LEFT JOIN vw_Contractor cn (NOLOCK) ON r.Counterparty = cn.Id
    JOIN vw_Currency cur (NOLOCK) ON r.CurrencyStandard = cur.Id
    ';

    -- Основной SELECT с OFFSET-FETCH
    SET @sql = '
    SELECT 
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
    ' + @joins + CHAR(10) + @where + '
    ORDER BY '+ ISNULL(@SortKey + ' ' + @SortDirection , ' r.Id')  + ' 
    OFFSET ' + CAST(@Offset AS NVARCHAR) + ' ROWS
    FETCH NEXT ' + CAST(@PageSize AS NVARCHAR) + ' ROWS ONLY;
    ';

    -- Подсчёт общего количества
    SET @sqlCount = '
    SELECT COUNT(1) AS TotalCount
    ' + @joins + CHAR(10) + @where + ';
    ';

    
    -- Выполнить оба запроса
    DECLARE @both_sql NVARCHAR(MAX) = @sql + @sqlCount;
    print @both_sql
    EXEC sp_executesql 
        @both_sql,
        N'
        @NodeFromNameEn NVARCHAR(100),
        @NodeFromNameRu NVARCHAR(100),
        @ProxyNodeNameEn NVARCHAR(100),
        @ProxyNodeNameRu NVARCHAR(100),
        @NodeToNameEn NVARCHAR(100),
        @NodeToNameRu NVARCHAR(100),
        @RateTypeName NVARCHAR(100),
        @ProductGroupName NVARCHAR(100)
        ',
        @NodeFromNameEn = @NodeFromNameEn,
        @NodeFromNameRu = @NodeFromNameRu,
        @ProxyNodeNameEn = @ProxyNodeNameEn,
        @ProxyNodeNameRu = @ProxyNodeNameRu,
        @NodeToNameEn = @NodeToNameEn,
        @NodeToNameRu = @NodeToNameRu,
        @RateTypeName = @RateTypeName,
        @ProductGroupName = @ProductGroupName;
END
