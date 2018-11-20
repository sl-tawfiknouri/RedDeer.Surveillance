-- INITIAL SQL MIGRATION SCRIPT --
-- AUTHOR : RYAN TRENCHARD
-- DATE : 20/11/2018
-- LAST MODIFIED: 20/11/2018 BY RYAN TRENCHARD
-- JIRA (RDPB-3143)
-- PURPOSE : Initial creation of some analytics tables. This will drive the client service UI reporting on rules.

START TRANSACTION;

INSERT INTO Migrations VALUES(11, "Analytics.sql", now());

ALTER TABLE SystemProcessOperationRuleRun DROP COLUMN Alerts;

-- THE ALERT ANALYTICS TABLE
CREATE TABLE RuleAnalyticsAlerts(Id NVARCHAR(255) NOT NULL PRIMARY KEY, SystemProcessOperationRuleRunId INT NOT NULL, CancelledOrderAlertsRaw INT NOT NULL, CancelledOrderAlertsAdjusted INT NOT NULL, HighProfitAlertsRaw INT NOT NULL, HighProfitAlertsAdjusted INT NOT NULL, HighVolumeAlertsRaw INT NOT NULL, HighVolumeAlertsAdjusted INT NOT NULL, LayeringAlertsRaw INT NOT NULL, LayeringAlertsAdjusted INT NOT NULL, MarkingTheCloseAlertsRaw INT NOT NULL, MarkingTheCloseAlertsAdjusted INT NOT NULL, SpoofingAlertsRaw INT NOT NULL, SpoofingAlertsAdjusted INT NOT NULL, WashTradeAlertsRaw INT NOT NULL,  WashTradeAlertsAdjusted INT NOT NULL, FOREIGN KEY(SystemProcessOperationRuleRunId) REFERENCES SystemProcessOperationRuleRun(Id));

-- THE UNIVERSE ANALYTICS TABLE
CREATE TABLE RuleAnalyticsUniverse(Id NVARCHAR(255) NOT NULL PRIMARY KEY, SystemProcessOperationRuleRunId INT NOT NULL, GenesisEventCount INT NOT NULL, EchatonEvenCount INT NOT NULL, TradeReddeerCount INT NOT NULL, TradeReddeerSubmittedCount INT NOT NULL, StockTickReddeerCount INT NOT NULL, MarketOpenCount INT NOT NULL, MarketCloseCount INT NOT NULL, UniqueTradersCount INT NOT NULL, UniqueSecuritiesCount INT NOT NULL, UniqueMarketsTradedOnCount INT NOT NULL, FOREIGN KEY(SystemProcessOperationRuleRunId) REFERENCES SystemProcessOperationRuleRun(Id));

COMMIT;
