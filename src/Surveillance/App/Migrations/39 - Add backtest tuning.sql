﻿-- SQL MIGRATION SCRIPT --
-- AUTHOR : RYAN TRENCHARD
-- DATE : 26/06/2019
-- LAST MODIFIED: 26/06/2019 BY RYAN TRENCHARD
-- PURPOSE : add column to identify tuning rule breaches and tuning table

START TRANSACTION;

    INSERT INTO Migrations VALUES(39, "parametertuningtbl.sql", now());
	ALTER TABLE RuleBreach Add ParameterTuning bit NOT NULL DEFAULT 0;
	CREATE TABLE RuleParameterTuning (Id int auto_increment primary key NOT NULL);

COMMIT;