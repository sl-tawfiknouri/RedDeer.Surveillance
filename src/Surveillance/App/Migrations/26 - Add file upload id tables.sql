-- SQL MIGRATION SCRIPT --
-- AUTHOR : RYAN TRENCHARD
-- DATE : 14/02/2019
-- LAST MODIFIED: 15/02/2019 BY RYAN TRENCHARD
-- PURPOSE : Create two tables for orders and allocations to link ids to file upload guids

START TRANSACTION;

    INSERT INTO Migrations VALUES(26, "Add file upload id tables.sql", now());

	CREATE TABLE FileUploadOrders(Id INT NOT NULL PRIMARY KEY auto_increment, FileUploadId INT NOT NULL, OrderId INT NOT NULL, index i_UploadId(FileUploadId));
	CREATE TABLE FileUploadAllocations(Id INT NOT NULL PRIMARY KEY auto_increment, FileUploadId INT NOT NULL, OrderAllocationId INT NOT NULL, index i_UploadId(FileUploadId));

	ALTER TABLE FileUploadOrders ADD CONSTRAINT unique_file_upload_orders UNIQUE(FileUploadId, OrderId);
	ALTER TABLE FileUploadAllocations ADD CONSTRAINT unique_file_upload_order_allocations UNIQUE(FileUploadId, OrderAllocationId);

	ALTER TABLE Orders ADD Live bit NOT NULL DEFAULT 0;
	ALTER TABLE OrdersAllocation ADD Live bit NOT NULL DEFAULT 0;

	UPDATE Orders SET Live = 1;
	UPDATE OrdersAllocation SET Live = 1;

COMMIT;