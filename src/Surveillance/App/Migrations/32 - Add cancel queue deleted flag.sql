-- SQL MIGRATION SCRIPT --
-- AUTHOR : RYAN TRENCHARD
-- DATE : 20/05/2019
-- LAST MODIFIED: 20/05/2019 BY RYAN TRENCHARD
-- PURPOSE : cancel queue deleted flag added to system process for optimisation

START TRANSACTION;

    INSERT INTO Migrations VALUES(32, "CancelQueueDeletedFlag.sql", now());

	ALTER TABLE SystemProcess ADD CancelRuleQueueDeletedFlag BIT NOT NULL DEFAULT 1;

COMMIT;