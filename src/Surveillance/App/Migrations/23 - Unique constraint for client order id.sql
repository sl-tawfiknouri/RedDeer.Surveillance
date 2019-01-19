-- SQL MIGRATION SCRIPT --
-- AUTHOR : RYAN TRENCHARD
-- DATE : 19/01/2019
-- LAST MODIFIED: 19/01/2019 BY RYAN TRENCHARD
-- PURPOSE : Unique constraint for client order ids to ensure that we can do updates in sql repos easily

START TRANSACTION;

    INSERT INTO Migrations VALUES(23, "unique constraint for client order id.sql", now());

	SET FOREIGN_KEY_CHECKS=0;

	ALTER IGNORE TABLE DealerOrders ADD UNIQUE INDEX (ClientDealerOrderId);
	ALTER IGNORE TABLE Orders ADD UNIQUE INDEX (ClientOrderId);

	SET FOREIGN_KEY_CHECKS=1;     

	ALTER TABLE OrdersAllocation DROP INDEX unique_orders_allocation;

		-- add a unique constraint
	ALTER TABLE OrdersAllocation ADD CONSTRAINT unique_orders_allocation UNIQUE(OrderId, Fund, Strategy, ClientAccountId);

COMMIT;