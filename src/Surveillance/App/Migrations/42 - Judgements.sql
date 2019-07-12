-- Add High Profit Order Analysis --
-- AUTHOR : RYAN TRENCHARD
-- DATE : 02/07/2019
-- PURPOSE : Add high profit order analysis table so we can identify threshold values for parameter setting
-- These are 'output parameters', rule parameters that are set for the outcomes of calculations
START TRANSACTION;

	INSERT INTO Migrations VALUES(42, "Add High Profit Analysis.sql", now());
	
	CREATE TABLE JudgementEquityHighProfitRule (Id INT auto_increment primary key NOT NULL, RuleRunId NVARCHAR(4095) NOT NULL, RuleRunCorrelationId NVARCHAR(4095) NOT NULL, OrderId NVARCHAR(255), ClientOrderId NVARCHAR(255), Parameter TEXT, AbsoluteHighProfit DECIMAL(25, 5) NULL, AbsoluteHighProfitCurrency NVARCHAR(255), PercentageHighProfit DECIMAL(25, 5) NULL, Analysis BIT, INDEX (OrderId));

--	CREATE TABLE JudgementEquityCancelledOrdersRule (Id INT auto_increment primary key NOT NULL, RuleRunId NVARCHAR(4095) NOT NULL, RuleRunCorrelationId NVARCHAR(4095) NOT NULL, CancelledOrderPercentagePositionThreshold DECIMAL(25, 5) NULL, CancelledOrderCountPercentageThreshold DECIMAL(25, 5) NULL);
--	CREATE TABLE JudgementEquityHighVolumeRule (Id INT auto_increment primary key NOT NULL, RuleRunId NVARCHAR(4095) NOT NULL, RuleRunCorrelationId NVARCHAR(4095) NOT NULL, DailyHighVolumePercentage DECIMAL(25, 5) NULL, WindowHighVolumePercentage DECIMAL(25, 5) NULL, MarketCapHighVolumePercentage DECIMAL(25, 5) NULL);
--	CREATE TABLE JudgementEquityLayeringRule (Id INT auto_increment primary key NOT NULL, RuleRunId NVARCHAR(4095) NOT NULL, RuleRunCorrelationId NVARCHAR(4095) NOT NULL, DailyPercentageMarketVolume DECIMAL(25, 5) NULL, WindowPercentageMarketVolume DECIMAL(25, 5) NULL);
--	CREATE TABLE JudgementEquityMarkingTheCloseRule (Id INT auto_increment primary key NOT NULL, RuleRunId NVARCHAR(4095) NOT NULL, RuleRunCorrelationId NVARCHAR(4095) NOT NULL, DailyPercentageMarketVolume DECIMAL(25, 5) NULL, WindowPercentageMarketVolume DECIMAL(25, 5) NULL, ThresholdOffTouchPercentage DECIMAL(25, 5) NULL);
--	CREATE TABLE JudgementEquityPlacingOrdersWithNoIntentToExecuteRule (Id INT auto_increment primary key NOT NULL, RuleRunId NVARCHAR(4095) NOT NULL, RuleRunCorrelationId NVARCHAR(4095) NOT NULL, Sigma DECIMAL(25, 5) NULL);
--	CREATE TABLE JudgementEquityRampingRule (Id INT auto_increment primary key NOT NULL, RuleRunId NVARCHAR(4095) NOT NULL, RuleRunCorrelationId NVARCHAR(4095) NOT NULL, AutoCorrelationCoefficient DECIMAL(25, 5) NULL);
--	CREATE TABLE JudgementEquitySpoofingRule (Id INT auto_increment primary key NOT NULL, RuleRunId NVARCHAR(4095) NOT NULL, RuleRunCorrelationId NVARCHAR(4095) NOT NULL, CancellationThreshold DECIMAL(25, 5) NULL, RelativeSizeSpoofExceedingReal DECIMAL(25, 5) NULL);

COMMIT;