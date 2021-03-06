﻿-- SQL MIGRATION SCRIPT --
-- AUTHOR : RYAN TRENCHARD
-- DATE : 13/01/2019
-- LAST MODIFIED: 13/01/2019 BY RYAN TRENCHARD
-- PURPOSE : Add new fund accounting tables

START TRANSACTION;

    INSERT INTO Migrations VALUES(21, "add account allocation tbl.sql", now());
	   
	CREATE TABLE OrdersAllocation(Id INT NOT NULL AUTO_INCREMENT PRIMARY KEY, OrderId NVARCHAR(255) NOT NULL, Fund nvarchar(255), Strategy nvarchar(255), ClientAccountId nvarchar(255), OrderFilledVolume BIGINT, INDEX i_OrderId(OrderId));

	CREATE INDEX i_StatusChanged ON Orders (StatusChangedDate);
    CREATE INDEX i_StatusChanged ON DealerOrders (StatusChangedDate);
	
	-- add a unique constraint
	ALTER TABLE OrdersAllocation ADD CONSTRAINT unique_orders_allocation UNIQUE(OrderId, Fund, Strategy, ClientAccountId, OrderFilledVolume);

	INSERT INTO SystemProcessOperationUploadFileType (Id, FileType) VALUES (2, "Allocation Data File");

COMMIT;