START TRANSACTION;

INSERT INTO Migrations 
VALUES(47, 'FinancialInstruments ExchangeSymbol update.sql', UTC_TIMESTAMP());

ALTER TABLE FinancialInstruments 
	MODIFY COLUMN ExchangeSymbol VARCHAR(100);

ALTER TABLE FinancialInstruments 
	MODIFY COLUMN UnderlyingExchangeSymbol VARCHAR(100);

COMMIT;