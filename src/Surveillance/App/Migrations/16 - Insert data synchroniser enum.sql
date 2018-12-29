-- INITIAL SQL MIGRATION SCRIPT --
-- AUTHOR : RYAN TRENCHARD
-- DATE : 29/12/2018
-- LAST MODIFIED: 29/12/2018 BY RYAN TRENCHARD
-- PURPOSE : FKEY required for enum table

START TRANSACTION;

 INSERT INTO Migrations VALUES(16, "Insert data synchroniser enum.sql", now());
 INSERT INTO SystemProcessType(Id, Description) VALUES(2, "Third Party Data Synchroniser");

COMMIT;