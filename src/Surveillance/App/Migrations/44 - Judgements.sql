-- Add High Profit Order Analysis --
-- AUTHOR : RYAN TRENCHARD
-- DATE : 02/07/2019
-- PURPOSE : Add high profit order analysis table so we can identify threshold values for parameter setting
-- These are 'output parameters', rule parameters that are set for the outcomes of calculations

START TRANSACTION;

INSERT INTO Migrations VALUES(44, "Add High Profit Analysis.sql", now());

CREATE TABLE JudgementEquityHighProfitRule (Id INT auto_increment primary key NOT NULL, RuleRunId NVARCHAR(4095) NOT NULL, RuleRunCorrelationId NVARCHAR(4095) NOT NULL, OrderId NVARCHAR(255), ClientOrderId NVARCHAR(255), Parameter TEXT, AbsoluteHighProfit DECIMAL(25, 5) NULL, AbsoluteHighProfitCurrency NVARCHAR(255), PercentageHighProfit DECIMAL(25, 5) NULL, Analysis BIT, INDEX (OrderId));

COMMIT;