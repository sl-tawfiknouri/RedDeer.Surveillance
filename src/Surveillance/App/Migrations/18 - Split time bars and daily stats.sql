-- SQL MIGRATION SCRIPT --
-- AUTHOR : RYAN TRENCHARD
-- DATE : 04/01/2019
-- LAST MODIFIED: 04/01/2019 BY RYAN TRENCHARD
-- PURPOSE : Split time bars and daily stats

START TRANSACTION;

    INSERT INTO Migrations VALUES(18, "Split time bars and daily stats.sql", now());

	CREATE TABLE InstrumentEquityTimeBars (Id int auto_increment primary key NOT NULL, SecurityId int NOT NULL, Epoch datetime NOT NULL, BidPrice decimal(18, 3) NULL, AskPrice decimal(18,3) NULL, MarketPrice decimal(18,3) NULL, VolumeTraded bigint NULL, Currency nvarchar(255), foreign key (SecurityId) REFERENCES FinancialInstruments(Id), INDEX(Epoch), INDEX(SecurityId));

	CREATE TABLE InstrumentEquityDailySummary (Id int auto_increment primary key NOT NULL, SecurityId int NOT NULL, Epoch datetime NOT NULL, EpochDate datetime NOT NULL, OpenPrice decimal(18,3) NULL, ClosePrice decimal(18,3) NULL, HighIntradayPrice decimal(18,3) NULL, LowIntradayPrice decimal(18, 3) NULL, ListedSecurities bigint NULL, MarketCap decimal(18, 3) NULL, DailyVolume bigint NULL, Currency nvarchar(255), foreign key (SecurityId) REFERENCES FinancialInstruments(Id), INDEX(Epoch), INDEX(SecurityId));

	DROP TABLE MarketStockExchangePrices;

	ALTER TABLE InstrumentEquityDailySummary ADD UNIQUE INDEX (SecurityId, EpochDate);
	ALTER TABLE InstrumentEquityTimeBars ADD UNIQUE INDEX (SecurityId, Epoch);

COMMIT;