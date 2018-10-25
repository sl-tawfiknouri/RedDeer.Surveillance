-- EXCEPTION AUDIT SQL MIGRATION SCRIPT --
-- AUTHOR : RYAN TRENCHARD
-- DATE : 24/10/2018
-- LAST MODIFIED: 24/10/2018 BY RYAN TRENCHARD
-- JIRA (RDPB-3022)
-- PURPOSE : Allow for exceptions to be tracked and linked back to operations where possible

START TRANSACTION;

INSERT INTO Migrations VALUES(4, "Exception Audit.sql", now());

CREATE TABLE Exceptions(Id int auto_increment primary key NOT NULL, Exception text, InnerException text, StackTrace text, SystemProcessId nvarchar(255) NULL, SystemProcessOperationId int NULL, SystemProcessOperationRuleRunId int NULL, SystemProcessOperationDistributeRuleId int NULL);

COMMIT;

