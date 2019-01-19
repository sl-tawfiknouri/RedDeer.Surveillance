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

	DROP TABLE TradeReddeer;

	CREATE TABLE OrderLifeCycleStatus (Id INT NOT NULL PRIMARY KEY, Description nvarchar(255));
	INSERT INTO OrderLifeCycleStatus(Id, Description) VALUES (0, "Unknown");
	INSERT INTO OrderLifeCycleStatus(Id, Description) VALUES (1, "Placed");
	INSERT INTO OrderLifeCycleStatus(Id, Description) VALUES (2, "Booked");
	INSERT INTO OrderLifeCycleStatus(Id, Description) VALUES (3, "Amended");
	INSERT INTO OrderLifeCycleStatus(Id, Description) VALUES (4, "Rejected");
	INSERT INTO OrderLifeCycleStatus(Id, Description) VALUES (5, "Cancelled");
	INSERT INTO OrderLifeCycleStatus(Id, Description) VALUES (6, "Filled");

COMMIT;