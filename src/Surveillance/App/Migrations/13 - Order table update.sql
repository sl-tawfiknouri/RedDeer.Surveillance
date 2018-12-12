-- INITIAL SQL MIGRATION SCRIPT --
-- AUTHOR : RYAN TRENCHARD
-- DATE : 12/12/2018
-- LAST MODIFIED: 12/12/2018 BY RYAN TRENCHARD
-- PURPOSE : Missing default value for instrument type
START TRANSACTION;

 INSERT INTO Migrations VALUES(13, "Order table update.sql", now());

ALTER TABLE FinancialInstruments ALTER InstrumentType SET DEFAULT 0;

COMMIT;