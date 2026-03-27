USE [mdm]
GO



CREATE NONCLUSTERED INDEX [ix_TransportRateSnapshot_StateId_IsDefRate] ON [dbo].TransportRateSnapshot
(
	[StateId],
    [IsDefRate]
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