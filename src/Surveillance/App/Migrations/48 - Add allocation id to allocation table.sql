-- Reducing nvarchar sizes of fields because of maximum mysql key size
-- Maximum key size is 3072 bytes (a nvarchar character takes up to 3 bytes) so the key size is limited to 1024 characters

START TRANSACTION;

    INSERT INTO Migrations VALUES(48, "Add allocation id to allocation table.sql", now());

	ALTER TABLE OrdersAllocation DROP INDEX unique_orders_allocation;

	ALTER TABLE OrdersAllocation CHANGE OrderId OrderId nvarchar(200);
    ALTER TABLE OrdersAllocation CHANGE Fund Fund nvarchar(200);
    ALTER TABLE OrdersAllocation CHANGE Strategy Strategy nvarchar(200);
    ALTER TABLE OrdersAllocation CHANGE ClientAccountId ClientAccountId nvarchar(200);
	ALTER TABLE OrdersAllocation ADD AllocationId nvarchar(200);

	ALTER TABLE OrdersAllocation ADD CONSTRAINT unique_orders_allocation UNIQUE(OrderId, Fund, Strategy, ClientAccountId, AllocationId);

COMMIT;