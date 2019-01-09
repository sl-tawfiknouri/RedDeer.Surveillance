-- INITIAL SQL MIGRATION SCRIPT --
-- AUTHOR : RYAN TRENCHARD
-- DATE : 15/12/2018
-- LAST MODIFIED: 15/12/2018 BY RYAN TRENCHARD
-- PURPOSE : Create a rule params table to link to rule run (sys proc ids)

START TRANSACTION;

 INSERT INTO Migrations VALUES(15, "Rule data request.sql", now());

 CREATE TABLE RuleDataRequest(Id INT auto_increment PRIMARY KEY, SystemProcessOperationRuleRunId INT NOT NULL, FinancialInstrumentId INT NOT NULL, MarketIdentifierCode nvarchar(20), StartTime DATETIME NOT NULL, EndTime DATETIME NOT NULL, Completed BIT NOT NULL, FOREIGN KEY (SystemProcessOperationRuleRunId) REFERENCES SystemProcessOperationRuleRun(Id), FOREIGN KEY (FinancialInstrumentId) REFERENCES FinancialInstruments(Id));

 	ALTER TABLE RuleDataRequest ADD UNIQUE INDEX (SystemProcessOperationRuleRunId, FinancialInstrumentId, MarketIdentifierCode, StartTime, EndTime);

    DROP TABLE MarketData;

COMMIT;