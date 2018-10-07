-- INITIAL SQL MIGRATION SCRIPT --
-- AUTHOR : RYAN TRENCHARD
-- DATE : 04/10/2018
-- LAST MODIFIED: 07/10/2018 BY RYAN TRENCHARD
-- JIRA (RDPB-2930)
-- PURPOSE : Seed database with a migration script table to track migrations. Add initial system process tables for auditing and system monitoring purposes.

START TRANSACTION;

CREATE DATABASE dev_surveillance;
USE dev_surveillance;

-- A MIGRATION TABLE FOR TRACKING MIGRATIONS THAT HAVE BEEN EXECUTED ON THE LOCAL MACHINE
CREATE TABLE Migrations(Id INT NOT NULL PRIMARY KEY, Description NVARCHAR(255) NOT NULL, ExecutedOn DATETIME NOT NULL);
INSERT INTO Migrations VALUES(1, "Initial.sql", now());

-- THE SYSTEM PROCESS TYPE TABLE TO ALLOW FOR FURTHER TYPES OF PROCESSES TO BE ADDED TO SYSTEM PROCESS MONITORING WHILST MAINTAINING CLARITY ON THE PURPOSE OF EACH PROCESS
CREATE TABLE SystemProcessType(Id INT NOT NULL PRIMARY KEY, Description NVARCHAR(255) NOT NULL);
INSERT INTO SystemProcessType VALUES(0, "Surveillance Service");
INSERT INTO SystemProcessType VALUES(1, "Relay Service");

-- THE SYSTEM PROCESS TABLE RECORDING INSTANCES OF THE SURVEILLANCE SERVICE RUNNING
CREATE TABLE SystemProcess(Id NVARCHAR(255) NOT NULL PRIMARY KEY, InstanceInitiated DATETIME NOT NULL, Heartbeat DATETIME NOT NULL, MachineId NVARCHAR(255) NOT NULL, ProcessId NVARCHAR(255) NOT NULL, SystemProcessTypeId INT NOT NULL, FOREIGN KEY(SystemProcessTypeId) REFERENCES SystemProcessType(Id));

-- THE SYSTEM PROCESS OPERATION STATE TABLE IS USED TO DESCRIBE THE STATE OF AN OPERATION I.E. DID A RULE RUN COMPLETE, FAIL OR IN PROCESS?
CREATE TABLE SystemProcessOperationState(Id INT NOT NULL PRIMARY KEY, Description NVARCHAR(255) NOT NULL);
INSERT INTO SystemProcessOperationState VALUES (0, "InProcess");
INSERT INTO SystemProcessOperationState VALUES (1, "Failed");
INSERT INTO SystemProcessOperationState VALUES (2, "Completed");
INSERT INTO SystemProcessOperationState VALUES (3, "CompletedWithErrors");
INSERT INTO SystemProcessOperationState VALUES (4, "BlockedClientServiceDown");

-- THE SYSTEM PROCESS OPERATION TABLE IS USED TO CAPTURE OPERATIONS WHICH ARE DISCRETE SERIES OF EVENTS THAT HAPPEN WITHIN A PROCESS WITH A CLEAR START AND END
CREATE TABLE SystemProcessOperation(Id int(255) NOT NULL AUTO_INCREMENT PRIMARY KEY, SystemProcessId NVARCHAR(255) NOT NULL, OperationStart DATETIME NOT NULL, OperationEnd DATETIME NULL, OperationState INT NOT NULL, FOREIGN KEY (SystemProcessId) REFERENCES SystemProcess(Id) ,FOREIGN KEY (OperationState) REFERENCES SystemProcessOperationState(Id));

-- THE SYSTEM PROCESS OPERATION DISTRIBUTE RULE TABLE IS USED FOR CONTAINING FURTHER INFORMATION ABOUT AN OPERATION WHICH HAS A RULE DISTRIBUTION WITHIN IT
CREATE TABLE SystemProcessOperationDistributeRule(Id INT NOT NULL AUTO_INCREMENT PRIMARY KEY, SystemProcessOperationId INT NOT NULL, ScheduleRuleInitialStart DATETIME NOT NULL, ScheduleRuleInitialEnd DATETIME NOT NULL, RulesDistributed NVARCHAR(1024) NULL, FOREIGN KEY (SystemProcessOperationId) REFERENCES SystemProcessOperation(Id));

-- THE SYSTEM PROCESS OPERATION RULE RUN TABLE IS USED FOR CONTAINING FURTHER INFORMATION ABOUT AN OPERATION WHICH HAS A RULE EXECUTED WITHIN IT
CREATE TABLE SystemProcessOperationRuleRun(Id INT NOT NULL AUTO_INCREMENT PRIMARY KEY, SystemProcessOperationId INT NOT NULL, RuleDescription NVARCHAR(1024) NOT NULL, RuleVersion NVARCHAR(255) NOT NULL, ScheduleRuleStart DATETIME NOT NULL, ScheduleRuleEnd DATETIME NOT NULL, Alerts INT NOT NULL, FOREIGN KEY (SystemProcessOperationId) REFERENCES SystemProcessOperation(Id));

COMMIT;