-- INITIAL SQL MIGRATION SCRIPT --
-- AUTHOR : RYAN TRENCHARD
-- DATE : 05/12/2018
-- LAST MODIFIED: 05/12/2018 BY RYAN TRENCHARD
-- JIRA (RDPB-3252)
-- PURPOSE : Domain rebuild to take into account missing transaction data
START TRANSACTION;

 INSERT INTO Migrations VALUES(12, "Order table.sql", now());
 RENAME TABLE `MarketStockExchange` TO `Market`;
 ALTER TABLE MarketStockExchangeSecurities CHANGE MarketStockExchangeId MarketId int;
 RENAME TABLE `MarketStockExchangeSecurities` TO `FinancialInstruments`;

 CREATE TABLE Orders(Id INT NOT NULL AUTO_INCREMENT PRIMARY KEY, MarketId INT NOT NULL NULL, SecurityId INT NOT NULL, ClientOrderId NVARCHAR(255), PlacedDate DATETIME NULL, BookedDate DATETIME NULL, AmendedDate DATETIME NULL, RejectedDate DATETIME NULL, CancelledDate DATETIME NULL, FilledDate DATETIME NULL, OrderType INT NULL, Position INT NULL, Currency NVARCHAR(3) NOT NULL, LimitPrice DECIMAL(18, 3) NULL, AveragePrice DECIMAL(18,3) NULL, OrderedVolume BIGINT NULL, FilledVolume BIGINT NULL, PortfolioManager NVARCHAR(300) NULL, ExecutingBroker NVARCHAR(300) NULL, TraderId NVARCHAR(254) NULL,ClearingAgent NVARCHAR(300) NULL, DealingInstructions NVARCHAR(1000) NULL, Strategy NVARCHAR(2040) NULL, Rationale NVARCHAR(2040) NULL, Fund NVARCHAR(2040) NULL, ClientAccountAttributionId NVARCHAR(255) NULL, StatusChangedDate DATETIME NULL, FOREIGN KEY (MarketId) REFERENCES Market(Id), FOREIGN KEY (SecurityId) REFERENCES FinancialInstruments(Id), INDEX i_PlacedDate(PlacedDate), INDEX i_RejectedDate(RejectedDate), INDEX i_CancelledDate(CancelledDate), INDEX i_FilledDate(FilledDate));

 CREATE TABLE Trades(Id INT NOT NULL AUTO_INCREMENT PRIMARY KEY, OrderId INT NOT NULL, ClientTradeId NVARCHAR(255) NULL, PlacedDate DATETIME NULL, BookedDate DATETIME NULL, AmendedDate DATETIME NULL, RejectedDate DATETIME NULL, CancelledDate DATETIME NULL, FilledDate DATETIME NULL, TraderId NVARCHAR(255) NULL, TradeCounterParty NVARCHAR(2040) NULL, OrderType INT NULL, Position INT NULL, Currency NVARCHAR(3) NOT NULL, LimitPrice DECIMAL(18,3) NULL, AveragePrice DECIMAL(18,3) NULL, OrderedVolume BIGINT NULL, FilledVolume BIGINT NULL, OptionStrikePrice DECIMAL(18,3) NULL, OptionExpirationDate DATETIME NULL, OptionEuropeanAmerican NVARCHAR(12) NULL, FOREIGN KEY (OrderId) REFERENCES Orders(Id), INDEX i_PlacedDate(PlacedDate), INDEX i_RejectedDate(RejectedDate), INDEX i_CancelledDate(CancelledDate), INDEX i_FilledDate(FilledDate));

 CREATE TABLE Transactions(Id INT NOT NULL AUTO_INCREMENT PRIMARY KEY, TradeId INT NOT NULL, ClientTransactionId NVARCHAR(255), PlacedDate DATETIME NULL, BookedDate DATETIME NULL, AmendedDate DATETIME NULL, RejectedDate DATETIME NULL, CancelledDate DATETIME NULL, FilledDate DATETIME NULL, TraderId NVARCHAR(254) NULL, CounterParty NVARCHAR(254) NULL, OrderType INT NULL, Position INT NULL, Currency NVARCHAR(3), LimitPrice DECIMAL(18,3) NULL, AveragePrice DECIMAL(18,3) NULL, OrderedVolume BIGINT NULL, FilledVolume BIGINT NULL, FOREIGN KEY (TradeId) REFERENCES Trades(Id), INDEX i_PlacedDate(PlacedDate), INDEX i_RejectedDate(RejectedDate), INDEX i_CancelledDate(CancelledDate), INDEX i_FilledDate(FilledDate));

ALTER TABLE FinancialInstruments ADD InstrumentType INT NOT NULL;
ALTER TABLE FinancialInstruments ADD UnderlyingCfi NVARCHAR(6) NULL;
ALTER TABLE FinancialInstruments ADD UnderlyingName NVARCHAR(254) NULL;
ALTER TABLE FinancialInstruments ADD UnderlyingSedol NVARCHAR(7) NULL;
ALTER TABLE FinancialInstruments ADD UnderlyingIsin NVARCHAR(12) NULL;
ALTER TABLE FinancialInstruments ADD UnderlyingFigi NVARCHAR(20) NULL;
ALTER TABLE FinancialInstruments ADD UnderlyingCusip NVARCHAR(9) NULL;
ALTER TABLE FinancialInstruments ADD UnderlyingLei NVARCHAR(20) NULL;
ALTER TABLE FinancialInstruments ADD UnderlyingExchangeSymbol NVARCHAR(20) NULL;
ALTER TABLE FinancialInstruments ADD UnderlyingBloombergTicker NVARCHAR(254) NULL;
ALTER TABLE FinancialInstruments ADD UnderlyingClientIdentifier NVARCHAR(254) NULL;

CREATE TABLE InstrumentTypes(Id INT NOT NULL AUTO_INCREMENT PRIMARY KEY, Descriptions NVARCHAR(254));
INSERT INTO InstrumentTypes(Descriptions) VALUES ("None"), ("Market"), ("Limit");

COMMIT;