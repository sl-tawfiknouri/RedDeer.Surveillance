-- SQL MIGRATION SCRIPT --
-- AUTHOR : RYAN TRENCHARD
-- DATE : 06/03/2019
-- LAST MODIFIED: 06/03/2019 BY RYAN TRENCHARD
-- PURPOSE : Add fixed income alert counting columns to analytics table

START TRANSACTION;

    INSERT INTO Migrations VALUES(28, "Add fixed income columns to analytics.sql", now());

	ALTER TABLE RuleAnalyticsAlerts ADD FixedIncomeWashTradeAlertsRaw INT NOT NULL DEFAULT 0;
	ALTER TABLE RuleAnalyticsAlerts ADD FixedIncomeWashTradeAlertsAdjusted INT NOT NULL DEFAULT 0;
	
	ALTER TABLE RuleAnalyticsAlerts ADD FixedIncomeHighVolumeIssuanceAlertsRaw INT NOT NULL DEFAULT 0;
	ALTER TABLE RuleAnalyticsAlerts ADD FixedIncomeHighVolumeIssuanceAlertsAdjusted INT NOT NULL DEFAULT 0;
	
	ALTER TABLE RuleAnalyticsAlerts ADD FixedIncomeHighProfitAlertsRaw INT NOT NULL DEFAULT 0;
	ALTER TABLE RuleAnalyticsAlerts ADD FixedIncomeHighProfitAlertsAdjusted INT NOT NULL DEFAULT 0;

COMMIT;