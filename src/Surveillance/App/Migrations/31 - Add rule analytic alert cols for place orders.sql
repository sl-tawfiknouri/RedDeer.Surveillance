-- SQL MIGRATION SCRIPT --
-- AUTHOR : RYAN TRENCHARD
-- DATE : 13/05/2019
-- LAST MODIFIED: 13/05/2019 BY RYAN TRENCHARD
-- PURPOSE : Extend rule analytics tables to include columns for placing orders without intent to execute

START TRANSACTION;

    INSERT INTO Migrations VALUES(31, "PlacingOrderAlerts.sql", now());

	ALTER TABLE RuleAnalyticsAlerts ADD PlacingOrdersAlertsRaw INT NOT NULL DEFAULT 0;
	ALTER TABLE RuleAnalyticsAlerts ADD PlacingOrdersAlertsAdjusted INT NOT NULL DEFAULT 0;

COMMIT;