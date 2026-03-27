USE [mdm]
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

