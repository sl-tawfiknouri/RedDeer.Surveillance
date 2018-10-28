-- BLOOMBERG TICKER FIELD UPDATE SCRIPT --
-- AUTHOR : RYAN TRENCHARD
-- DATE : 28/10/2018
-- LAST MODIFIED: 28/10/2018 BY RYAN TRENCHARD
-- JIRA (RDPB-3898)
-- PURPOSE : Update bloomberg ticker field size as its been set too low

START TRANSACTION;

INSERT INTO Migrations VALUES(7, "Bloomberg Ticker.sql", now());


ALTER TABLE MarketStockExchangeSecurities MODIFY COLUMN BloombergTicker NVARCHAR(4090);
ALTER TABLE MarketData MODIFY COLUMN BloombergTicker NVARCHAR(4090);
ALTER TABLE TradeReddeer MODIFY COLUMN SecurityBloombergTicker NVARCHAR(4090);

COMMIT;

