-- SQL MIGRATION SCRIPT --
-- AUTHOR : RYAN TRENCHARD
-- DATE : 11/01/2019
-- LAST MODIFIED: 11/01/2019 BY RYAN TRENCHARD
-- PURPOSE : Store rule run id in audit tbl

START TRANSACTION;

    INSERT INTO Migrations VALUES(19, "Store rule run id in audit tbl.sql", now());

	ALTER TABLE SystemProcessOperationRuleRun ADD IsForceRun bit;

COMMIT;