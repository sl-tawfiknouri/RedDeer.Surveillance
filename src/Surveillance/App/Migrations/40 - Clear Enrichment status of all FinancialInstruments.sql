-- CLEAR ENRICHMENT STATUS OF ALL FINANCIALINSTRUMENTS --
-- AUTHOR : JEZZ GOODWIN
-- DATE : 28/06/2019
-- PURPOSE : Clear enrichment status, so that enrichment happens again for all instruments
START TRANSACTION;

INSERT INTO Migrations VALUES(40, "Clear Enrichment status of all FinancialInstruments.sql", now());

UPDATE financialinstruments SET Enrichment = NULL WHERE Enrichment IS NOT NULL;

COMMIT;