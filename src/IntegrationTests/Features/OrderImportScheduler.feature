Feature: OrderImportScheduler
	Test which order uploads end up triggering rule runs.
	The system should trigger a rule run when it receives new data. If the exact same order is uploaded again, then a rule run isn't necessary.

Background:
	Given the orders
	| OrderId | _Date               | _EquitySecurity | OrderDirection | OrderAverageFillPrice | _Volume |
	| 0       | 2018-01-02T15:00:00 | VODAFONE        | BUY            | 3                     | 100     |
	When the data importer is run
	Then there should be an order with id "0" and autoscheduled "false"
	When the auto scheduler is run
	Then there should be an order with id "0" and autoscheduled "true"

Scenario: Uploading the same order shouldn't re-autoschedule
	Given the orders
	| OrderId | _Date               | _EquitySecurity | OrderDirection | OrderAverageFillPrice | _Volume |
	| 0       | 2018-01-02T15:00:00 | VODAFONE        | BUY            | 3                     | 100     |
	When the data importer is run
	Then there should be an order with id "0" and autoscheduled "true"

Scenario: Uploading a modified order should re-autoschedule
	Given the orders
	| OrderId | _Date               | _EquitySecurity | OrderDirection | OrderAverageFillPrice | _Volume |
	| 0       | 2018-01-02T15:00:00 | VODAFONE        | BUY            | 4                     | 100     |
	When the data importer is run
	Then there should be an order with id "0" and autoscheduled "false"
