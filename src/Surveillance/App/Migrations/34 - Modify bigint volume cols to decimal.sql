-- SQL MIGRATION SCRIPT --
-- AUTHOR : RYAN TRENCHARD
-- DATE : 28/05/2019
-- LAST MODIFIED: 28/05/2019 BY RYAN TRENCHARD
-- PURPOSE : update filled volume and ordered volume fields to decimal instead of long

START TRANSACTION;

    INSERT INTO Migrations VALUES(34, "modifybigintvolumecolstodecimal.sql", now());

	ALTER TABLE Orders MODIFY OrderedVolume DECIMAL(25, 5);
	ALTER TABLE Orders MODIFY FilledVolume DECIMAL(25, 5);

	ALTER TABLE DealerOrders MODIFY OrderedVolume DECIMAL(25, 5);
	ALTER TABLE DealerOrders MODIFY FilledVolume DECIMAL(25, 5);

	ALTER TABLE OrdersAllocation MODIFY OrderFilledVolume DECIMAL(25, 5);

COMMIT;