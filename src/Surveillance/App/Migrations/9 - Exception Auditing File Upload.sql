-- EXCEPTION AUDITING FIELD UPDATE SCRIPT --
-- AUTHOR : RYAN TRENCHARD
-- DATE : 07/11/2018
-- LAST MODIFIED: 07/11/2018 BY RYAN TRENCHARD
-- JIRA (RDPB-3022)
-- PURPOSE : Need a column for the exceptions table to link back to the new type of operation (file upload)

START TRANSACTION;

INSERT INTO Migrations VALUES(9, "Exception Auditing File Upload.sql", now());

ALTER TABLE Exceptions ADD COLUMN SystemProcessOperationFileUploadId INT NULL;

COMMIT;

