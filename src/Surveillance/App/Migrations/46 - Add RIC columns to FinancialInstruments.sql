-- ADD RIC TO FINANCIALINSTRUMENTS --
-- AUTHOR : JEZZ GOODWIN
-- DATE : 21/10/2019
-- PURPOSE : Add RIC columns for instrument identification
START TRANSACTION;

INSERT INTO Migrations VALUES(46, "Add Ric columns to FinancialInstruments.sql", now());

ALTER TABLE FinancialInstruments ADD Ric VARCHAR(30) AFTER BloombergTicker;
ALTER TABLE FinancialInstruments ADD UnderlyingRic VARCHAR(30) AFTER UnderlyingBloombergTicker;

COMMIT;