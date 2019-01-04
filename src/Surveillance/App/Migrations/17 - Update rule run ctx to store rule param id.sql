-- SQL MIGRATION SCRIPT --
-- AUTHOR : RYAN TRENCHARD
-- DATE : 03/01/2019
-- LAST MODIFIED: 03/01/2019 BY RYAN TRENCHARD
-- PURPOSE : Rule Parameter Id required for rescheduling

START TRANSACTION;

    INSERT INTO Migrations VALUES(17, "Update Rule Run ctx to store rule param id.sql", now());

	ALTER TABLE SystemProcessOperationRuleRun ADD RuleParameterId nvarchar(255);
	ALTER TABLE SystemProcessOperationRuleRun ADD RuleTypeId int;
	ALTER TABLE SystemProcessOperationRuleRun ADD IsBackTest bit;

COMMIT;

START TRANSACTION;

DELETE FROM TradeOrderPosition WHERE Id >= 0;
INSERT INTO TradeOrderPosition(Id, Description) VALUES(0, "Unknown");
INSERT INTO TradeOrderPosition(Id, Description) VALUES(1, "Buy");
INSERT INTO TradeOrderPosition(Id, Description) VALUES(2, "Sell");
INSERT INTO TradeOrderPosition(Id, Description) VALUES(3, "Short");
INSERT INTO TradeOrderPosition(Id, Description) VALUES(4, "Cover");

COMMIT;
