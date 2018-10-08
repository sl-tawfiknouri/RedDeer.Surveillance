-- TRADE SQL MIGRATION SCRIPT --
-- AUTHOR : RYAN TRENCHARD
-- DATE : 08/10/2018
-- LAST MODIFIED: 08/10/2018 BY RYAN TRENCHARD
-- JIRA (RDPB-2937)
-- PURPOSE : Migrate exchange data from elastic search to Aurora
USE dev_surveillance;

START TRANSACTION;

INSERT INTO Migrations VALUES(3, "Market.sql", now());

CREATE TABLE MarketStockExchange(Id int auto_increment primary key NOT NULL, MarketId nvarchar(16) NOT NULL, MarketName nvarchar(255) NOT NULL);

CREATE TABLE MarketStockExchangeSecurities(Id int auto_increment primary key NOT NULL, MarketStockExchangeId int NOT NULL, ClientIdentifier nvarchar(255) NULL, Sedol nvarchar(8) NULL, Isin nvarchar(20) NULL, Figi nvarchar(12) NULL, Cusip nvarchar(9) NULL, Lei nvarchar(20) NULL, ExchangeSymbol nvarchar(5) NULL, BloombergTicker nvarchar(5) NULL, SecurityName nvarchar(255) NOT NULL, Cfi nvarchar(6) NULL, IssuerIdentifier nvarchar(255) NULL, SecurityCurrency nvarchar(10) NULL, foreign key (MarketStockExchangeId) REFERENCES MarketStockExchange(Id));

CREATE TABLE MarketStockExchangePrices (Id int auto_increment primary key NOT NULL, SecurityId int NOT NULL, Epoch datetime NOT NULL, BidPrice decimal(18, 3) NULL, AskPrice decimal(18,3) NULL, MarketPrice decimal(18,3) NULL, OpenPrice decimal(18,3) NULL, ClosePrice decimal(18,3) NULL, HighIntradayPrice decimal(18,3) NULL, LowIntradayPrice decimal(18, 3) NULL, ListedSecurities bigint NULL, MarketCap decimal(18, 3) NULL, VolumeTradedInTick bigint NULL, DailyVolume bigint NULL, foreign key (SecurityId) REFERENCES MarketStockExchangeSecurities(Id));

COMMIT;