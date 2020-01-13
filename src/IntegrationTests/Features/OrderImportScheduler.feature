Feature: OrderImportScheduler
	Test which order/allocation uploads end up triggering rule runs.
	The system should trigger a rule run when it receives new data. If the exact same item is uploaded again, then a rule run isn't necessary.
	Autoscheduled = true means that the item has been included as part of a rule run request.
	If an autoscheduled order/allocation reverts to Autoscheduled = false, it means that it will get re-scheduled to run again.

Background:
	Create an order and allocation in the system which have been autoscheduled.
	Given the orders
	| OrderId | _Date               | _EquitySecurity | OrderDirection | OrderAverageFillPrice | _Volume |
	| 0       | 2018-01-02T15:00:00 | VODAFONE        | BUY            | 3                     | 100     |
	And the allocations
	| OrderId | OrderFilledVolume |
	| 0       | 1                 |
	When the data importer is run
	And the auto scheduler is run
	Then there should be a single order with id "0" and autoscheduled "true"
	And there should be a single allocation with OrderId "0" and autoscheduled "true"

Scenario: Uploading the same order shouldn't re-autoschedule
	Given the orders
	| OrderId | _Date               | _EquitySecurity | OrderDirection | OrderAverageFillPrice | _Volume |
	| 0       | 2018-01-02T15:00:00 | VODAFONE        | BUY            | 3                     | 100     |
	When the data importer is run
	Then there should be a single order with id "0" and autoscheduled "true"

Scenario: Uploading a modified order should re-autoschedule
	Given the orders
	| OrderId | _Date               | _EquitySecurity | OrderDirection | OrderAverageFillPrice | _Volume |
	| 0       | 2018-01-02T15:00:00 | VODAFONE        | BUY            | 4                     | 100     |
	When the data importer is run
	Then there should be a single order with id "0" and autoscheduled "false"

Scenario: Uploading the same allocation shouldn't re-autoschedule
	Given the allocations
	| OrderId | OrderFilledVolume |
	| 0       | 1                 |
	When the data importer is run
	Then there should be a single allocation with OrderId "0" and autoscheduled "true"

Scenario: Uploading a modified allocation should re-autoschedule
	Given the allocations
	| OrderId | OrderFilledVolume |
	| 0       | 2                 |
	When the data importer is run
	Then there should be a single allocation with OrderId "0" and autoscheduled "false"
