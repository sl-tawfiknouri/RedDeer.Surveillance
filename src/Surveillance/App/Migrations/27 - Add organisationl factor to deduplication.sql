-- SQL MIGRATION SCRIPT --
-- AUTHOR : RYAN TRENCHARD
-- DATE : 23/02/2019
-- LAST MODIFIED: 23/02/2019 BY RYAN TRENCHARD
-- PURPOSE : Add two columns to deduplication table so we can begin saving organisational factor and org factor value with breaches preventing over deduplication

START TRANSACTION;

    INSERT INTO Migrations VALUES(27, "Add organisationl factor to deduplication.sql", now());

	ALTER TABLE RuleBreach ADD OrganisationalFactorType INT NOT NULL DEFAULT 0;
	ALTER TABLE RuleBreach ADD OrganisationalFactorValue NVARCHAR(4095) DEFAULT "NONE";

COMMIT;