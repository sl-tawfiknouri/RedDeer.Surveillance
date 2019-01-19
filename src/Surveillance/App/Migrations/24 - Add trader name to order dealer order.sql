-- SQL MIGRATION SCRIPT --
-- AUTHOR : RYAN TRENCHARD
-- DATE : 19/01/2019
-- LAST MODIFIED: 19/01/2019 BY RYAN TRENCHARD
-- PURPOSE : Add trader name to order / dealer order as per E.W. request.

START TRANSACTION;

    INSERT INTO Migrations VALUES(24, "Add trader name to order dealer order.sql", now());

	ALTER TABLE Orders ADD TraderName nvarchar(255);
	ALTER TABLE DealerOrders ADD TraderName nvarchar(255);

COMMIT;