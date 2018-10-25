-- TRADE SQL MIGRATION SCRIPT --
-- AUTHOR : RYAN TRENCHARD
-- DATE : 08/10/2018
-- LAST MODIFIED: 08/10/2018 BY RYAN TRENCHARD
-- JIRA (RDPB-2937)
-- PURPOSE : Migrate exchange data from elastic search to Aurora
START TRANSACTION;

INSERT INTO Migrations VALUES(3, "Market.sql", now());

CREATE TABLE MarketStockExchange(Id int auto_increment primary key NOT NULL, MarketId nvarchar(16) NOT NULL, MarketName nvarchar(255) NOT NULL, INDEX(MarketId));

CREATE TABLE MarketStockExchangeSecurities(Id int auto_increment primary key NOT NULL, MarketStockExchangeId int NOT NULL, ClientIdentifier nvarchar(255) NULL, Sedol nvarchar(8) NULL, Isin nvarchar(20) NULL, Figi nvarchar(12) NULL, Cusip nvarchar(9) NULL, Lei nvarchar(20) NULL, ExchangeSymbol nvarchar(5) NULL, BloombergTicker nvarchar(5) NULL, SecurityName nvarchar(255) NOT NULL, Cfi nvarchar(6) NULL, IssuerIdentifier nvarchar(255) NULL, SecurityCurrency nvarchar(10) NULL, foreign key (MarketStockExchangeId) REFERENCES MarketStockExchange(Id), INDEX(Sedol), INDEX(Isin));

CREATE TABLE MarketStockExchangePrices (Id int auto_increment primary key NOT NULL, SecurityId int NOT NULL, Epoch datetime NOT NULL, BidPrice decimal(18, 3) NULL, AskPrice decimal(18,3) NULL, MarketPrice decimal(18,3) NULL, OpenPrice decimal(18,3) NULL, ClosePrice decimal(18,3) NULL, HighIntradayPrice decimal(18,3) NULL, LowIntradayPrice decimal(18, 3) NULL, ListedSecurities bigint NULL, MarketCap decimal(18, 3) NULL, VolumeTradedInTick bigint NULL, DailyVolume bigint NULL, foreign key (SecurityId) REFERENCES MarketStockExchangeSecurities(Id), INDEX(Epoch), INDEX(SecurityId));

CREATE TABLE MarketData(MarketId nvarchar(16), MarketName nvarchar(255), ClientIdentifier nvarchar(255), Sedol nvarchar(8), Isin nvarchar(20), Figi nvarchar(12), Cusip nvarchar(9), Lei nvarchar(20), ExchangeSymbol nvarchar(5), BloombergTicker nvarchar(5), SecurityName nvarchar(255), Cfi nvarchar(6), IssuerIdentifier nvarchar(255), SecurityCurrency nvarchar(10), Epoch datetime, BidPrice decimal(18, 3), AskPrice decimal(18, 3), MarketPrice decimal(18, 3), OpenPrice decimal(18, 3), ClosePrice decimal(18, 3), HighIntradayPrice decimal(18, 3), LowIntradayPrice decimal(18, 3), ListedSecurities bigint, MarketCap decimal(18, 3), VolumeTradedInTick bigint, DailyVolume bigint, SecurityId bigint, INDEX(Epoch));

COMMIT;
