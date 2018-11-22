-- INITIAL SQL MIGRATION SCRIPT --
-- AUTHOR : RYAN TRENCHARD
-- DATE : 19/11/2018
-- LAST MODIFIED: 19/11/2018 BY RYAN TRENCHARD
-- JIRA (RDPB-3147)
-- PURPOSE : We would like to see which rule runs are driven by which distribute rule requests

START TRANSACTION;

INSERT INTO Migrations VALUES(10, "CorrelationId.sql", now());
ALTER TABLE SystemProcessOperationRuleRun ADD COLUMN CorrelationId nvarchar(255);

COMMIT;
