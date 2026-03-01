USE mdm
GO



/*

 exec dbo.GetTransportRatesByFilters_v3 
     @PageNumber=1,
     @PageSize=10,
     @SortKey=N'NodeToNameRu',
     @SortDirection=N'ASC',
     @Filter=N'[{"PropertType":null,"PropertyName":"Code","Value":"2763157","Operator":7,"StringComparison":5},
        {"PropertType":null,"PropertyName":"NodeFromNameRu","Value":"каз","Operator":7,"StringComparison":5},
        {"PropertType":null,"PropertyName":"ProductGroupName","Value":"полиоле","Operator":7,"StringComparison":5},
        {"PropertType":null,"PropertyName":"StartDate","Value":"2026-02-28","Operator":4,"StringComparison":5}]'



*/
CREATE OR ALTER PROCEDURE dbo.GetTransportRatesByFilters_v3
    @PageNumber INT = 1,
    @PageSize INT = 20,

    @SortKey NVARCHAR(100) = NULL,
    @SortDirection NVARCHAR(100) = NULL,

    @Filter NVARCHAR(MAX) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @AllowedColumns TABLE
    (
        ColumnName NVARCHAR(100) NOT NULL,
        ColumnType NVARCHAR(100) NOT NULL
    )
    DECLARE @FilteredColumns TABLE
    (
        ColumnName  NVARCHAR(100) NOT NULL,
        ColumnType  NVARCHAR(100) NOT NULL,
        ColumnValue NVARCHAR(100) NULL,
        Operator    NVARCHAR(100) NULL
    )
    DECLARE
        @Offset INT = (@PageNumber - 1) * @PageSize,
        @CTEs NVARCHAR(MAX) = '',
        @JoinClause NVARCHAR(MAX) = '',
        @WhereClause NVARCHAR(MAX) = '',
        @MainSQL NVARCHAR(MAX),
        @TotalCountSQL NVARCHAR(MAX),
        @BothSQL NVARCHAR(MAX);

    SET @PageNumber = IIF(@PageNumber < 1, 1, @PageNumber);
    SET @PageSize = IIF(@PageSize < 1, 20, @PageSize);

    INSERT INTO @AllowedColumns (ColumnName, ColumnType)
    VALUES (N'StartDate', N'DATE'),
           (N'EndDate', N'DATE'),
           (N'NodeFromNameRu', N'NVARCHAR'),
           (N'NodeFromNameEn', N'NVARCHAR'),
           (N'ProxyNodeNameRu', N'NVARCHAR'),
           (N'ProxyNodeNameEn', N'NVARCHAR'),
           (N'NodeToNameRu', N'NVARCHAR'),
           (N'NodeToNameEn', N'NVARCHAR'),
           (N'RateTypeName', N'NVARCHAR'),
           (N'ProductGroupName', N'NVARCHAR')
    
    INSERT INTO @FilteredColumns (ColumnName, ColumnType, ColumnValue, Operator)
    SELECT 
        AC.ColumnName,
        AC.ColumnType,
        ISNULL(LTRIM(RTRIM(JS.[Value])), '') AS ColumnValue,
        JS.Operator
    FROM OPENJSON(@Filter)
    WITH (
        PropertType      INT            '$.PropertType',
        PropertyName     NVARCHAR(100)  '$.PropertyName',
        Value            NVARCHAR(MAX)  '$.Value',
        Operator         INT            '$.Operator',
        StringComparison INT            '$.StringComparison'
    ) JS
    JOIN @AllowedColumns AC
    ON JS.PropertyName = AC.ColumnName

    SELECT @WhereClause = STRING_AGG(
        CASE ColumnType
            -- Логика для СТРОК
            WHEN 'NVARCHAR' THEN 
                CASE WHEN LEN(ColumnValue) > 2 THEN
                    N'
                    AND CONTAINS(tr.' + ColumnName + ', N''"' + ColumnValue + '*"'') '
                ELSE '' 
                END

            -- Логика для ДАТ
            WHEN 'DATE' THEN 
                CASE WHEN ISDATE(ColumnValue) = 1
                THEN
                    ISNULL(N'
                    AND  tr.' + ColumnName + ' ' + dbo.fn_GetSqlOperator_v3(Operator) -- если функция вернет NULL, весь фильтр обнулится. И это правильное поведение
                    +''''+ ColumnValue + '''','')
                ELSE ''
                END

            -- Логика по умолчанию для остальных типов
            ELSE ''
        END, 
        ''
    )
    FROM @FilteredColumns;
    SET @WhereClause = CONCAT('WHERE tr.StateId = 1', @WhereClause)

    -- Основной SELECT с подставленными CTE и JOIN'ами
    SET @MainSQL = '
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
    ' + @JoinClause + ' 
    ' + @WhereClause + '
    ORDER BY '+ ISNULL(@SortKey + ' ' + @SortDirection + ', ', '')  + '  tr.Id DESC
    OFFSET ' + CAST(@Offset AS NVARCHAR(20)) + ' ROWS
    FETCH NEXT ' + CAST(@PageSize AS NVARCHAR(20)) + ' ROWS ONLY;
    ';

    SET @TotalCountSQL = '
    WITH CTE AS (SELECT 1 AS TST)
    ' + @CTEs + '
    SELECT COUNT(1) AS TotalCount
    FROM [mdm].[dbo].[TransportRateSnapshot] tr
    ' + @JoinClause + ' 
    ' + @WhereClause + ';';

    SET @BothSQL = @MainSQL + CHAR(13) + CHAR(10) + @TotalCountSQL;

    -- Для отладки можно раскомментировать:
    PRINT @BothSQL;

    EXEC sp_executesql @BothSQL;
    
END
GO
