-- BLOOMBERG TICKER FIELD UPDATE SCRIPT --
-- AUTHOR : RYAN TRENCHARD
-- DATE : 05/11/2018
-- LAST MODIFIED: 05/11/2018 BY RYAN TRENCHARD
-- JIRA (RDPB-3125)
-- PURPOSE : Normalise database by swapping out direct security data in trade table with a foreign key to market security

START TRANSACTION;

INSERT INTO Migrations VALUES(8, "Trade security to foreign key.sql", now());

ALTER TABLE TradeReddeer ADD COLUMN SecurityId INT DEFAULT(1), ADD FOREIGN KEY SecurityId(SecurityId) REFERENCES MarketStockExchangeSecurities(Id);

COMMIT;

