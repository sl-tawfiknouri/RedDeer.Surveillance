-- SQL MIGRATION SCRIPT --
-- AUTHOR : RYAN TRENCHARD
-- DATE : 03/01/2019
-- LAST MODIFIED: 03/01/2019 BY RYAN TRENCHARD
-- PURPOSE : Rule Parameter Id required for rescheduling

START TRANSACTION;

	ALTER TABLE SystemProcessOperationRuleRun ADD RuleParameterId nvarchar(255);
	ALTER TABLE SystemProcessOperationRuleRun ADD RuleTypeId int;
	ALTER TABLE SystemProcessOperationRuleRun ADD IsBackTest bit;

COMMIT;