-- SQL MIGRATION SCRIPT --
-- PURPOSE : Add Broker table

START TRANSACTION;

    INSERT INTO Migrations 
		VALUES(38, "Add Broker table", now());
	
	CREATE TABLE Brokers
	( 
		Id INT NOT NULL AUTO_INCREMENT,
		ExternalId NVARCHAR(64) NULL,
		Name NVARCHAR(1024) NULL,
		PRIMARY KEY (Id)
	);
		
	ALTER TABLE Orders 
		ADD BrokerId INT NULL;

	ALTER TABLE Orders 
		ADD CONSTRAINT fk_Orders_Brokers_BrokerId 
		FOREIGN KEY (BrokerId) REFERENCES Brokers(Id);

COMMIT;