-- TRADE SQL MIGRATION SCRIPT --
-- AUTHOR : RYAN TRENCHARD
-- DATE : 07/10/2018
-- LAST MODIFIED: 07/10/2018 BY RYAN TRENCHARD
-- JIRA (RDPB-2937)
-- PURPOSE : Migrate trade data from elastic search to Aurora
START TRANSACTION;

INSERT INTO Migrations VALUES(2, "Trade.sql", now(), 0);

CREATE TABLE TradeOrderType(Id INT NOT NULL PRIMARY KEY, Description nvarchar(255));
INSERT INTO TradeOrderType(Id, Description) VALUES(0, "Unknown"), (1, "Market"), (2, "Limit"), (3, "StopLoss");

CREATE TABLE TradeOrderPosition(Id INT NOT NULL PRIMARY KEY, Description nvarchar(255));
INSERT INTO TradeOrderPosition(Id, Description) VALUES(0, "Buy"), (1, "Sell");

CREATE TABLE TradeOrderStatus(Id INT NOT NULL PRIMARY KEY, Description nvarchar(255));
INSERT INTO TradeOrderStatus(Id, Description) VALUES (0, "Booked"), (1, "Cancelled"), (2, "PartialFulfilled"), (3, "Fulfilled"), (4, "CallAmended"), (5, "Rejected"), (6, "CancelledPostBooking");

-- All date times in UTC
CREATE TABLE TradeReddeer(Id INT NOT NULL AUTO_INCREMENT PRIMARY KEY, OrderTypeId INT NOT NULL, MarketId nvarchar(255) NOT NULL, MarketName nvarchar(1024) NOT NULL, SecurityClientIdentifier nvarchar(255) NULL, SecuritySedol nvarchar(255) NULL, SecurityIsin nvarchar(255) NULL, SecurityFigi nvarchar(255) NULL, SecurityCusip nvarchar(255) NULL, SecurityExchangeSymbol nvarchar(255) NULL, SecurityLei nvarchar(255) NULL, SecurityBloombergTicker nvarchar(255) NULL, SecurityName nvarchar(255) NOT NULL, SecurityCfi nvarchar(255), LimitPrice decimal(18, 3) DEFAULT NULL, LimitCurrency nvarchar(255) NULL, TradeSubmittedOn DateTime NOT NULL, StatusChangedOn DateTime NOT NULL, FilledVolume int NULL, OrderedVolume int NULL, OrderPositionId int NOT NULL, OrderStatusId int NOT NULL, OrderCurrency nvarchar(255), TraderId nvarchar(255) NULL, TradeClientAttributionId nvarchar(255) NULL, AccountId nvarchar(255) NULL, PartyBrokerId nvarchar(255) NULL, CounterPartyBrokerId nvarchar(255) NULL, SecurityIssuerIdentifier nvarchar(255) NULL, ExecutedPrice decimal(18, 3) NULL, DealerInstructions nvarchar(5000), TradeRationale nvarchar(5000), TradeStrategy nvarchar(255), INDEX i_MarketId (MarketId), INDEX i_SecuritySedol (SecuritySedol), INDEX i_SecurityIsin (SecurityIsin), INDEX i_SecurityExchangeSymbol (SecurityExchangeSymbol), INDEX i_TradeSubmittedOn (TradeSubmittedOn), Index i_StatusChangedOn (StatusChangedOn), Index i_TraderId (TraderId), foreign key (OrderTypeId) REFERENCES TradeOrderType(Id), foreign key (OrderStatusId) REFERENCES TradeOrderStatus(Id), foreign key (OrderPositionId) REFERENCES TradeOrderPosition(id));

COMMIT;

UPDATE Migrations SET Complete = 1 WHERE Id = 2;

COMMIT;
