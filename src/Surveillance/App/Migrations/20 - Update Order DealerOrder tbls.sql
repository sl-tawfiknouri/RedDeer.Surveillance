-- SQL MIGRATION SCRIPT --
-- AUTHOR : RYAN TRENCHARD
-- DATE : 12/01/2019
-- LAST MODIFIED: 12/01/2019 BY RYAN TRENCHARD
-- PURPOSE : 

START TRANSACTION;

    INSERT INTO Migrations VALUES(20, "update order dealerorder tbls.sql", now());

	DROP TABLE Transactions;
	DROP TABLE Trades;
	DROP TABLE Orders;

	CREATE TABLE Orders(Id INT NOT NULL AUTO_INCREMENT PRIMARY KEY, MarketId INT NOT NULL NULL, SecurityId INT NOT NULL, ClientOrderId NVARCHAR(255), OrderVersion NVARCHAR(255), OrderVersionLinkId NVARCHAR(255), OrderGroupId NVARCHAR(255), Weighting DECIMAL(18, 3) NULL, PlacedDate DATETIME NULL, BookedDate DATETIME NULL, AmendedDate DATETIME NULL, RejectedDate DATETIME NULL, CancelledDate DATETIME NULL, FilledDate DATETIME NULL, StatusChangedDate DATETIME NULL, OrderType INT NULL, Direction INT NULL, Currency NVARCHAR(3) NOT NULL, SettlementCurrency NVARCHAR(3), CleanDirty NVARCHAR(5), AccumulatedInterest DECIMAL(18, 3) NULL, LimitPrice DECIMAL(18, 3) NULL, AverageFillPrice DECIMAL(18, 3) NULL, OrderedVolume BIGINT NULL, FilledVolume BIGINT NULL, TraderId NVARCHAR(255), ClearingAgent NVARCHAR(255), DealingInstructions NVARCHAR(255), OptionStrikePrice DECIMAL(18, 3) NULL, OptionExpirationDate DATETIME NULL, OptionEuropeanAmerican NVARCHAR(255), FOREIGN KEY (MarketId) REFERENCES Market(Id), FOREIGN KEY (SecurityId) REFERENCES FinancialInstruments(Id), INDEX i_PlacedDate(PlacedDate), INDEX i_RejectedDate(RejectedDate), INDEX i_CancelledDate(CancelledDate), INDEX i_FilledDate(FilledDate));

	  CREATE TABLE DealerOrders(Id INT NOT NULL AUTO_INCREMENT PRIMARY KEY, OrderId INT NOT NULL, ClientDealerOrderId NVARCHAR(255) NULL, DealerOrderVersion NVARCHAR(255), DealerOrderVersionLinkId NVARCHAR(255), DealerOrderGroupId NVARCHAR(255), PlacedDate DATETIME NULL, BookedDate DATETIME NULL, AmendedDate DATETIME NULL, RejectedDate DATETIME NULL, CancelledDate DATETIME NULL, FilledDate DATETIME NULL, StatusChangedDate DATETIME NULL, DealerId NVARCHAR(255), Notes NVARCHAR(255), CounterParty NVARCHAR(255), OrderType INT NULL, Direction INT NULL, Currency NVARCHAR(3) NOT NULL, SettlementCurrency NVARCHAR(3), CleanDirty NVARCHAR(5), AccumulatedInterest DECIMAL(18, 3) NULL, LimitPrice DECIMAL(18, 3), AverageFillPrice DECIMAL(18, 3), OrderedVolume BIGINT NULL, FilledVolume BIGINT NULL, OptionStrikePrice DECIMAL(18, 3) NULL, OptionExpirationDate DATETIME NULL, OptionEuropeanAmerican NVARCHAR(255), FOREIGN KEY (OrderId) REFERENCES Orders(Id), INDEX i_PlacedDate(PlacedDate), INDEX i_RejectedDate(RejectedDate), INDEX i_CancelledDate(CancelledDate), INDEX i_FilledDate(FilledDate));

COMMIT;