-- INITIAL SQL MIGRATION SCRIPT --
-- AUTHOR : RYAN TRENCHARD
-- DATE : 05/12/2018
-- LAST MODIFIED: 05/12/2018 BY RYAN TRENCHARD
-- JIRA (RDPB-3252)
-- PURPOSE : Domain rebuild to take into account missing transaction data

START TRANSACTION;

INSERT INTO migrations VALUES(12, "Order table.sql", now());

RENAME TABLE `marketstockexchange` TO `market`;
ALTER TABLE marketstockexchangesecurities CHANGE MarketStockExchangeId MarketId int;

CREATE TABLE orders(Id INT NOT NULL AUTO_INCREMENT PRIMARY KEY, MarketId INT NOT NULL NULL, SecurityId INT NOT NULL, ClientOrderId NVARCHAR(255), PlacedDate DATETIME NULL, BookedDate DATETIME NULL, AmendedDate DATETIME NULL, RejectedDate DATETIME NULL, CancelledDate DATETIME NULL, FilledDate DATETIME NULL, OrderType INT NULL, Position INT NULL, Currency NVARCHAR(3), LimitPrice DECIMAL(18, 3) NULL, AveragePrice DECIMAL(18,3) NULL, OrderedVolume BIGINT NULL, FilledVolume BIGINT NULL, PortfolioManager NVARCHAR, ExecutingBroker NVARCHAR, ClearingAgent NVARCHAR, DealingInstructions NVARCHAR, Strategy NVARCHAR, Rationale NVARCHAR, Fund NVARCHAR, ClientAccountAttributionId NVARCHAR);
-- ADD FOREIGN KEY SYNTAX!

CREATE TABLE trades(Id INT NOT NULL AUTO_INCREMENT PRIMARY KEY, OrderId INT NOT NULL, ClientTradeId NVARCHAR(255), PlacedDate DATETIME NULL, BookedDate DATETIME NULL, AmendedDate DATETIME NULL, RejectedDate DATETIME NULL, CancelledDate DATETIME NULL, FilledDate DATETIME NULL, TraderId NVARCHAR, TradeCounterParty NVARCHAR, OrderType INT NULL, Position INT NULL, Currency NVARCHAR(3), LimitPrice DECIMAL(18,3) NULL, AveragePrice DECIMAL(18,3) NULL, OrderedVolume BIGINT NULL, FilledVolume BIGINT NULL, OptionStrikePrice DECIMAL(18,3) NULL, OptionExpirationDate DATETIME NULL, OptionEuropeanAmerican NVARCHAR(12));
-- ADD FOREIGN KEY SYNTAX!

CREATE TABLE transactions(Id INT NOT NULL AUTO_INCREMENT PRIMARY KEY, TradeId INT NOT NULL, ClientTransactionId NVARCHAR(255), PlacedDate DATETIME NULL, BookedDate DATETIME NULL, AmendedDate DATETIME NULL, RejectedDate DATETIME NULL, CancelledDate DATETIME NULL, FilledDate DATETIME NULL, TraderId NVARCHAR, CounterParty NVARCHAR, OrderType INT NULL, Position INT NULL, Currency NVARCHAR(3), LimitPrice DECIMAL(18,3) NULL, AveragePrice DECIMAL(18,3) NULL, OrderedVolume BIGINT NULL, FilledVolume BIGINT NULL);
-- ADD FOREIGN KEY SYNTAX!

-- ok so the other big question is the security vs instrument side of things?
-- Looks like security needs (a) renaming to instrument (b) derivative fields added (c) we have a problem with an instrument having a market - maybe a listed exchange?b ut it's not going to be 1:1 yeah I think that's OK to have a listed exchange instead of MarketStockExchangeId? 
-- just rename it over =)

ALTER TABLE marketstockexchangesecurities CHANGE MarketStockExchangeId ListedExchangeId INT;
RENAME TABLE `marketstockexchangesecurities` TO `financialinstruments`;

ALTER TABLE financialinstruments ADD InstrumentType INT NOT NULL DEFAULT(0);
ALTER TABLE financialinstruments ADD UnderlyingCfi NVARCHAR(6) NULL;
ALTER TABLE financialinstruments ADD UnderlyingName NVARCHAR NULL;
ALTER TABLE financialinstruments ADD UnderlyingSedol NVARCHAR(7) NULL;
ALTER TABLE financialinstruments ADD UnderlyingIsin NVARCHAR(12) NULL;
ALTER TABLE financialinstruments ADD UnderlyingFigi NVARCHAR(20) NULL;
ALTER TABLE financialinstruments ADD UnderlyingCusip NVARCHAR(9) NULL;
ALTER TABLE financialinstruments ADD UnderlyingLei NVARCHAR(20) NULL;
ALTER TABLE financialinstruments ADD UnderlyingExchangeSymbol NVARCHAR(20) NULL;
ALTER TABLE financialinstruments ADD UnderlyingBloombergTicker NVARCHAR NULL;

COMMIT;