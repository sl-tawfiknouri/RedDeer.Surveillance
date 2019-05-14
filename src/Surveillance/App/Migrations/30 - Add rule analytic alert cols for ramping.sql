-- SQL MIGRATION SCRIPT --
-- AUTHOR : RYAN TRENCHARD
-- DATE : 29/04/2019
-- LAST MODIFIED: 29/04/2019 BY RYAN TRENCHARD
-- PURPOSE : Extend rule analytics tables to include columns for ramping

START TRANSACTION;

    INSERT INTO Migrations VALUES(30, "Add file type enum value.sql", now());

	ALTER TABLE RuleAnalyticsAlerts ADD RampingAlertsRaw INT NOT NULL DEFAULT 0;
	ALTER TABLE RuleAnalyticsAlerts ADD RampingAlertsAdjusted INT NOT NULL DEFAULT 0;

COMMIT;
