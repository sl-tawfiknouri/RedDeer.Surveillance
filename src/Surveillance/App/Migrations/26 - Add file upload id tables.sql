-- SQL MIGRATION SCRIPT --
-- AUTHOR : RYAN TRENCHARD
-- DATE : 14/02/2019
-- LAST MODIFIED: 14/02/2019 BY RYAN TRENCHARD
-- PURPOSE : Create two tables for orders and allocations to link ids to file upload guids

START TRANSACTION;

    INSERT INTO Migrations VALUES(26, "Add file upload id tables.sql", now());

	CREATE TABLE FileUploadOrders(Id INT NOT NULL PRIMARY KEY auto_increment, FileUploadId INT NOT NULL, OrderId INT NOT NULL, index i_UploadId(FileUploadId));
	CREATE TABLE FileUploadAllocations(Id INT NOT NULL PRIMARY KEY auto_increment, FileUploadId INT NOT NULL, OrderAllocationId INT NOT NULL, index i_UploadId(FileUploadId));

COMMIT;