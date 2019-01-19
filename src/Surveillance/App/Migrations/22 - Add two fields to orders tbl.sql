-- SQL MIGRATION SCRIPT --
-- AUTHOR : RYAN TRENCHARD
-- DATE : 19/01/2019
-- LAST MODIFIED: 19/01/2019 BY RYAN TRENCHARD
-- PURPOSE : Extend orders with two new fields to support the Client Service

START TRANSACTION;

    INSERT INTO Migrations VALUES(22, "Add two fields to orders tbl.sql", now());
	   
	ALTER TABLE Orders ADD CreatedDate DateTime NOT NULL DEFAULT now();
	ALTER TABLE Orders ADD LifeCycleStatus INT NOT NULL DEFAULT 0;

	ALTER TABLE DealerOrders ADD CreatedDate DateTime NOT NULL DEFAULT now();
	ALTER TABLE DealerOrders ADD LifeCycleStatus INT NOT NULL DEFAULT 0;

COMMIT;