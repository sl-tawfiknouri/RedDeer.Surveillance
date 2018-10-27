-- RELAY AUDIT SQL MIGRATION SCRIPT --
-- AUTHOR : RYAN TRENCHARD
-- DATE : 27/10/2018
-- LAST MODIFIED: 27/10/2018 BY RYAN TRENCHARD
-- JIRA (RDPB-3066)
-- PURPOSE : Allow for exceptions to be tracked within the relay service (may be called data import in future)

START TRANSACTION;

INSERT INTO Migrations VALUES(6, "Relay Audit.sql", now());

CREATE TABLE SystemProcessOperationUploadFileType(Id int primary key NOT NULL, FileType text);
INSERT INTO SystemProcessOperationUploadFileType (Id, FileType) VALUES (0, "Market Data File"), (1, "Trade Data File");

CREATE TABLE SystemProcessOperationUploadFile(Id int auto_increment primary key NOT NULL, FileType int, FilePath text, FOREIGN KEY (FileType) REFERENCES SystemProcessOperationUploadFileType(Id));

COMMIT;

