-- INITIAL SQL MIGRATION SCRIPT --
-- AUTHOR : RYAN TRENCHARD
-- DATE : 15/12/2018
-- LAST MODIFIED: 15/12/2018 BY RYAN TRENCHARD
-- PURPOSE : Create a rule params table to link to rule run (sys proc ids)

START TRANSACTION;

 INSERT INTO Migrations VALUES(15, "Rule data request.sql", now());

 CREATE TABLE RuleDataRequest(Id INT NOT NULL PRIMARY KEY, SystemProcessOperationRuleRunId INT NOT NULL, FinancialInstrumentId INT NOT NULL, StartTime DATETIME NOT NULL, EndTime DATETIME NOT NULL, Completed BIT NOT NULL);

 DROP TABLE MarketData;

COMMIT;