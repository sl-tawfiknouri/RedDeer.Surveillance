-- INITIAL SQL MIGRATION SCRIPT --
-- AUTHOR : RYAN TRENCHARD
-- DATE : 15/12/2018
-- LAST MODIFIED: 15/12/2018 BY RYAN TRENCHARD
-- PURPOSE : Responding to market data generation changes
START TRANSACTION;

 INSERT INTO Migrations VALUES(14, "InstrumentTable update.sql", now());

ALTER TABLE FinancialInstruments MODIFY COLUMN ExchangeSymbol VARCHAR(20);
ALTER TABLE FinancialInstruments MODIFY COLUMN BloombergTicker VARCHAR(2000);
ALTER TABLE FinancialInstruments MODIFY COLUMN UnderlyingBloombergTicker VARCHAR(2000);

COMMIT;