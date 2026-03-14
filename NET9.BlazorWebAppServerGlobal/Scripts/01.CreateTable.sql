USE [mdm]
GO


CREATE OR ALTER FUNCTION dbo.fn_GetSqlOperator (@OperatorName NVARCHAR(50))
RETURNS NVARCHAR(2)
AS
BEGIN
    RETURN CASE @OperatorName
        WHEN 'Equals' THEN '='
        WHEN 'NotEquals' THEN '<>'
        WHEN 'LessThan' THEN '<'
        WHEN 'LessThanOrEquals' THEN '<='
        WHEN 'GreaterThan' THEN '>'
        WHEN 'GreaterThanOrEquals' THEN '>='
        ELSE NULL -- Или можно вернуть пустую строку '', если оператор не найден
    END
END
GO

CREATE OR ALTER FUNCTION dbo.fn_GetSqlOperator_v3 (@OperatorName NVARCHAR(50))
RETURNS NVARCHAR(2)
AS
BEGIN
    RETURN CASE @OperatorName
        WHEN '1' THEN '=' -- Equals
        WHEN '2' THEN '<>' -- NotEquals
        WHEN '3' THEN '<' --LessThan
        WHEN '4' THEN '<=' -- LessThanOrEquals
        WHEN '5' THEN '>' --GreaterThan
        WHEN '6' THEN '>=' --GreaterThanOrEquals
        ELSE NULL -- Или можно вернуть пустую строку '', если оператор не найден
    END
END
GO

DROP TABLE IF EXISTS dbo.TransportRateSnapshot
GO

CREATE TABLE dbo.TransportRateSnapshot (
    Id              [bigint] IDENTITY(1,1) NOT NULL,
    [StateId]           int NOT NULL,
    Code                NVARCHAR(50) NOT NULL,
    IsDefRate           BIT NOT NULL,
    StartDate           DATETIME NOT NULL,
    EndDate             DATETIME NOT NULL,
    CreationDate        DATETIME NOT NULL,
    LastChangeDate      DATETIME,
    TotalCostTon        DECIMAL(19, 2),
    TotalCostTransport  DECIMAL(19, 2),
    RateTypeCode        NVARCHAR(50),
    RateTypeName        NVARCHAR(255),
    NodeFromCode        NVARCHAR(50),
    NodeFromNameEn      NVARCHAR(255),
    NodeFromNameRu      NVARCHAR(255),
    ProxyNodeCode       NVARCHAR(50),
    ProxyNodeNameEn     NVARCHAR(255),
    ProxyNodeNameRu     NVARCHAR(255),
    NodeToCode          NVARCHAR(50),
    NodeToNameEn        NVARCHAR(255),
    NodeToNameRu        NVARCHAR(255),
    TransportKindCode   NVARCHAR(50),
    TransportKindNameRu NVARCHAR(255),
    TransportTypeCode   NVARCHAR(50),
    TransportTypeNameRu NVARCHAR(255),
    ProductGroupCode    NVARCHAR(50),
    ProductGroupName    NVARCHAR(255),
    ContractorCode      NVARCHAR(50),
    ContractorEGRUL     NVARCHAR(255),
    CurrencyCode        NVARCHAR(10),
    CurrencyName        NVARCHAR(50)
);

ALTER TABLE dbo.TransportRateSnapshot ADD  CONSTRAINT [PK_TransportRateSnapshot] PRIMARY KEY CLUSTERED 
(
	Id ASC
)
GO




ALTER TABLE dbo.TransportRateSnapshot ADD  CONSTRAINT [DF_TransportRateSnapshot_CreationDate]  DEFAULT (getdate()) FOR [CreationDate]
GO


CREATE NONCLUSTERED INDEX [ix_TransportRateSnapshot_StateId] ON [dbo].TransportRateSnapshot
(
	[StateId] ASC
)

GO


CREATE NONCLUSTERED  INDEX [ix_TransportRateSnapshot_code_include_id] ON [dbo].TransportRateSnapshot
(
	[Code] ASC
)
--INCLUDE([StateId])
GO


CREATE NONCLUSTERED  INDEX [ix_TransportRateSnapshot_NodeFromNameRu] ON [dbo].TransportRateSnapshot
(
	[NodeFromNameRu] ASC
)
--INCLUDE([StateId])
GO

--drop  INDEX [ix_TransportRateSnapshot_NodeToNameRu] ON [dbo].TransportRateSnapshot

CREATE NONCLUSTERED  INDEX ix_TransportRateSnapshot_NodeToNameRu ON [dbo].TransportRateSnapshot
(
	[NodeToNameRu] ASC
)
--INCLUDE([StateId])
GO

/*
CREATE NONCLUSTERED INDEX [ix_TransportRateSnapshot_NodeToNameRu] ON [dbo].TransportRateSnapshot
(
	[NodeToNameRu] ASC
)
INCLUDE([Id]
      ,[StateId]
      ,[Code]
      ,[IsDefRate]
      ,[StartDate]
      ,[EndDate]
      ,[CreationDate]
      ,[LastChangeDate]
      ,[TotalCostTon]
      ,[TotalCostTransport]
      ,[RateTypeCode]
      ,[RateTypeName]
      ,[NodeFromCode]
      ,[NodeFromNameEn]
      ,[NodeFromNameRu]
      ,[ProxyNodeCode]
      ,[ProxyNodeNameEn]
      ,[ProxyNodeNameRu]
      ,[NodeToCode]
      ,[NodeToNameEn]
      ,[TransportKindCode]
      ,[TransportKindNameRu]
      ,[TransportTypeCode]
      ,[TransportTypeNameRu]
      ,[ProductGroupCode]
      ,[ProductGroupName]
      ,[ContractorCode]
      ,[ContractorEGRUL]
      ,[CurrencyCode]
      ,[CurrencyName])
GO

*/
CREATE FULLTEXT CATALOG ftCatalog AS DEFAULT;

GO


-- Создать новый сразу с несколькими столбцами
CREATE FULLTEXT INDEX ON dbo.TransportRateSnapshot 
(
 
    Code               ,
    RateTypeCode       ,
    RateTypeName       ,
    NodeFromCode       ,
    NodeFromNameEn     ,
    NodeFromNameRu     ,
    ProxyNodeCode      ,
    ProxyNodeNameEn    ,
    ProxyNodeNameRu    ,
    NodeToCode         ,
    NodeToNameEn       ,
    NodeToNameRu       ,
    TransportKindCode  ,
    TransportKindNameRu,
    TransportTypeCode  ,
    TransportTypeNameRu,
    ProductGroupCode   ,
    ProductGroupName   ,
    ContractorCode     ,
    ContractorEGRUL    ,
    CurrencyCode       ,
    CurrencyName       


)
KEY INDEX PK_TransportRateSnapshot;
GO