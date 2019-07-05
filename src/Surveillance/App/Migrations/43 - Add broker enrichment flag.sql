-- CLEAR ENRICHMENT STATUS OF ALL FINANCIALINSTRUMENTS --
-- AUTHOR : Ryan Trenchard
-- DATE : 02/07/2019
-- PURPOSE : Add live flag to broker field to show data that has or hasn't been enriched
START TRANSACTION;

INSERT INTO Migrations VALUES(43, "Add broker enrichment flag.sql", now());

	ALTER TABLE Brokers ADD Live bit;
	ALTER TABLE Brokers ADD UNIQUE (Name);
	ALTER TABLE Brokers ADD Updated date NULL;

COMMIT;