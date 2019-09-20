-- Add High Profit Order Analysis --
-- AUTHOR : RYAN TRENCHARD
-- DATE : 19/09/2019
-- PURPOSE : Add high volume order analysis table so we can identify threshold values for parameter setting
-- These are 'output parameters', rule parameters that are set for the outcomes of calculations

START TRANSACTION;

INSERT INTO Migrations VALUES(45, "Add High Volume Analysis.sql", now());

CREATE TABLE JudgementEquityHighVolumeRule (Id INT auto_increment primary key NOT NULL, RuleRunId NVARCHAR(4095) NOT NULL, RuleRunCorrelationId NVARCHAR(4095) NOT NULL, OrderId NVARCHAR(255), ClientOrderId NVARCHAR(255), Parameter TEXT, Analysis BIT, WindowVolumeThresholdAmount DECIMAL(25, 5) NULL, WindowVolumeThresholdPercentage DECIMAL(25, 5) NULL, WindowVolumeTradedAmount DECIMAL(25, 5) NULL, WindowVolumeTradedPercentage DECIMAL(25, 5) NULL, WindowVolumeBreach BIT, DailyVolumeThresholdAmount DECIMAL(25, 5) NULL, DailyVolumeThresholdPercentage DECIMAL(25, 5) NULL, DailyVolumeTradedAmount DECIMAL(25, 5) NULL, DailyVolumeTradedPercentage DECIMAL(25, 5) NULL, DailyVolumeBreach BIT, INDEX (OrderId));

COMMIT;