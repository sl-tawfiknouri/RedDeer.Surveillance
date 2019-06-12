-- SQL MIGRATION SCRIPT --
-- AUTHOR : RYAN TRENCHARD
-- DATE : 12/06/2019
-- LAST MODIFIED: 12/06/2019 BY RYAN TRENCHARD
-- PURPOSE : add table for rescheduling

START TRANSACTION;

    INSERT INTO Migrations VALUES(36, "addscheduletasktable.sql", now());

	CREATE TABLE AdHocScheduleRequest (Id int auto_increment primary key NOT NULL, ScheduleFor date NOT NULL, QueueId int NOT NULL, JsonSqsMessage text, OriginatingService nvarchar(255) NOT NULL, Processed bit NOT NULL, INDEX(ScheduleFor), INDEX(Processed));

COMMIT;