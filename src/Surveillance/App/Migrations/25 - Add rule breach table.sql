-- SQL MIGRATION SCRIPT --
-- AUTHOR : RYAN TRENCHARD
-- DATE : 24/01/2019
-- LAST MODIFIED: 24/01/2019 BY RYAN TRENCHARD
-- PURPOSE : remove use of SQS queue for transferring data beyond an id

START TRANSACTION;

    INSERT INTO Migrations VALUES(25, "Add rule breach table.sql", now());

	CREATE TABLE RuleBreach(Id INT NOT NULL PRIMARY KEY auto_increment, RuleId nvarchar(1023), CorrelationId nvarchar(1023), IsBackTest BIT NOT NULL, CreatedOn DATETIME NOT NULL, Title nvarchar(2048), Description nvarchar(8190), Venue nvarchar(1023), StartOfPeriodUnderInvestigation DATETIME NOT NULL, EndOfPeriodUnderInvestigation DATETIME NOT NULL, AssetCfi nvarchar(10), ReddeerEnrichmentId nvarchar(1023), SystemOperationId INT NOT NULL);

	CREATE TABLE RuleBreachOrders(RuleBreachId INT NOT NULL, OrderId INT NOT NULL);
	ALTER TABLE RuleBreachOrders ADD PRIMARY KEY(RuleBreachId, OrderId);

COMMIT;