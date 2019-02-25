-- SQL MIGRATION SCRIPT --
-- AUTHOR : RYAN TRENCHARD
-- DATE : 19/01/2019
-- LAST MODIFIED: 19/01/2019 BY RYAN TRENCHARD
-- PURPOSE : Unique constraint for client order ids to ensure that we can do updates in sql repos easily

START TRANSACTION;

    INSERT INTO Migrations VALUES(23, "unique constraint for client order id.sql", now());

	ALTER TABLE OrdersAllocation DROP INDEX unique_orders_allocation;
	ALTER TABLE OrdersAllocation ADD CONSTRAINT unique_orders_allocation UNIQUE(OrderId, Fund, Strategy, ClientAccountId);

	ALTER TABLE DealerOrders ADD CONSTRAINT unique_dealer_order_client_id UNIQUE (ClientDealerOrderId);
	ALTER TABLE Orders ADD CONSTRAINT unique_order_client_id UNIQUE(ClientOrderId);

COMMIT;