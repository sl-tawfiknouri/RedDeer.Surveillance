-- SQL MIGRATION SCRIPT --
-- AUTHOR : RYAN TRENCHARD
-- DATE : 19/06/2019
-- LAST MODIFIED: 19/06/2019 BY RYAN TRENCHARD
-- PURPOSE : add column for data source

START TRANSACTION;

    INSERT INTO Migrations VALUES(37, "updatedatarequesttbl.sql", now());
	ALTER TABLE RuleDataRequest Add DataSource INT NOT NULL DEFAULT 0;

COMMIT;