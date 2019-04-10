-- SQL MIGRATION SCRIPT --
-- AUTHOR : RYAN TRENCHARD
-- DATE : 10/04/2019
-- LAST MODIFIED: 10/04/2019 BY RYAN TRENCHARD
-- PURPOSE : Extend file type enum

START TRANSACTION;

    INSERT INTO Migrations VALUES(29, "Add file type enum value.sql", now());
	INSERT INTO SystemProcessOperationUploadFileType VALUES(3, "Etl Data File");

COMMIT;