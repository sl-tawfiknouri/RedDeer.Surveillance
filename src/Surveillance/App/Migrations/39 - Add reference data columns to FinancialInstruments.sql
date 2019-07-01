-- ADD REFERENCE DATA COLUMNS TO FINANCIALINSTRUMENTS --
-- AUTHOR : JEZZ GOODWIN
-- DATE : 27/06/2019
-- PURPOSE : Add Sector, Industry, Region and Country columns to FinancialInstruments to allow for filtering
START TRANSACTION;

 INSERT INTO Migrations VALUES(39, "Add reference data columns to FinancialInstruments.sql", now());

ALTER TABLE FinancialInstruments ADD SectorCode VARCHAR(20);
ALTER TABLE FinancialInstruments ADD IndustryCode VARCHAR(20);
ALTER TABLE FinancialInstruments ADD RegionCode VARCHAR(20);
ALTER TABLE FinancialInstruments ADD CountryCode VARCHAR(20);

COMMIT;