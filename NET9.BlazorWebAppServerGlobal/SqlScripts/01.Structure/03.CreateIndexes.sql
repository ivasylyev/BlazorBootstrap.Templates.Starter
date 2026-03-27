USE [mdm]
GO



CREATE NONCLUSTERED INDEX [ix_TransportRateSnapshot_IsArchive_IsDefRate] ON [dbo].TransportRateSnapshot
(
	[IsArchive],
    [IsDefRate]
)
GO


CREATE NONCLUSTERED  INDEX [ix_TransportRateSnapshot_code_include_id] ON [dbo].TransportRateSnapshot
(
	[Code] ASC
)
--INCLUDE([IsArchive])
GO


CREATE NONCLUSTERED  INDEX [ix_TransportRateSnapshot_NodeFromNameRu] ON [dbo].TransportRateSnapshot
(
	[NodeFromNameRu] ASC
)
--INCLUDE([IsArchive])
GO

--drop  INDEX [ix_TransportRateSnapshot_NodeToNameRu] ON [dbo].TransportRateSnapshot

CREATE NONCLUSTERED  INDEX ix_TransportRateSnapshot_NodeToNameRu ON [dbo].TransportRateSnapshot
(
	[NodeToNameRu] ASC
)
--INCLUDE([IsArchive])
GO

/*
CREATE NONCLUSTERED INDEX [ix_TransportRateSnapshot_NodeToNameRu] ON [dbo].TransportRateSnapshot
(
	[NodeToNameRu] ASC
)
INCLUDE([Id]
      ,[IsArchive]
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
--DROP FULLTEXT CATALOG ftCatalog ;
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