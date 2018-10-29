-- SECURITY ENRICHMENT SQL MIGRATION SCRIPT --
-- AUTHOR : RYAN TRENCHARD
-- DATE : 25/10/2018
-- LAST MODIFIED: 25/10/2018 BY RYAN TRENCHARD
-- JIRA (RDPB-2990)
-- PURPOSE : Enable security data enrichment processes

START TRANSACTION;

INSERT INTO Migrations VALUES(5, "Security Enrichment.sql", now());

ALTER TABLE MarketStockExchangeSecurities ADD ReddeerId NVARCHAR(20) NULL;
ALTER TABLE MarketStockExchangeSecurities ADD Enrichment DATETIME NULL, ADD INDEX (Enrichment);

COMMIT;