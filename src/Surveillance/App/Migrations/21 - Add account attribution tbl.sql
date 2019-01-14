-- SQL MIGRATION SCRIPT --
-- AUTHOR : RYAN TRENCHARD
-- DATE : 13/01/2019
-- LAST MODIFIED: 13/01/2019 BY RYAN TRENCHARD
-- PURPOSE : Add new fund accounting tables

START TRANSACTION;

    INSERT INTO Migrations VALUES(21, "add account attribution tbl.sql", now());
	   
	CREATE TABLE OrdersAttribution(Id INT NOT NULL AUTO_INCREMENT PRIMARY KEY, OrderId INT NOT NULL, Fund nvarchar(255), Strategy nvarchar(255), OrderFilledVolume BIGINT);

	CREATE INDEX i_StatusChanged ON Orders (StatusChangedDate);
    CREATE INDEX i_StatusChanged ON DealerOrders (StatusChangedDate);

COMMIT;