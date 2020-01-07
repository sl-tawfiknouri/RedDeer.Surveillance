Feature: AllocationImportUniqueness
	Testing what fields are used for uniqueness when importing allocations.

Background: 
	Given the high volume core settings
	| VolumeType | VolumePercentage |
	| Daily      | 10               |
	And the orders
	| OrderId | _Date               | _EquitySecurity | OrderDirection | OrderAverageFillPrice | _Volume |
	| 0       | 2018-01-01T15:00:00 | VODAFONE        | BUY            | 100                   | 100     |
	And the equity close prices
	| Date       | _EquitySecurity | ClosePrice | DailyVolume |
	| 2018-01-01 | VODAFONE        | 130        | 1000        |

Scenario: Basic no-breach test
	Check that the rule will not trigger with a volume of 99
	Given the allocations
	| OrderId | OrderFilledVolume |
	| 0       | 99                |
	When the rule is run between "2018-01-01" and "2018-01-01"
	Then there should be no breaches

Scenario: Basic breach test
	Check that the rule will trigger with a volume of 100
	And the allocations
	| OrderId | OrderFilledVolume |
	| 0       | 100               |
	When the rule is run between "2018-01-01" and "2018-01-01"
	Then there should be a breach with order ids "0"

Scenario: Duplicate allocation rows
	If these allocations were unique, then there would be a breach as 99 + 99 >= 100. But if they are detected as duplicates, then there is no breach.
	Given the allocations
	| OrderId | OrderFilledVolume |
	| 0       | 99                |
	| 0       | 99                |
	When the rule is run between "2018-01-01" and "2018-01-01"
	Then there should be no breaches

Scenario: Unique allocations by fund
	Because these allocations have unique funds, they are treated as separate allocation rows. Together they should breach.
	And the allocations
	| OrderId | Fund | OrderFilledVolume |
	| 0       | A    | 40                |
	| 0       | B    | 60                |
	When the rule is run between "2018-01-01" and "2018-01-01"
	Then there should be a breach with order ids "0"

Scenario: Unique allocations by allocation id
	Because these allocations have unique funds, they are treated as separate allocation rows. Together they should breach.
	And the allocations
	| OrderId | AllocationId | OrderFilledVolume |
	| 0       | 0            | 40                |
	| 0       | 1            | 60                |
	When the rule is run between "2018-01-01" and "2018-01-01"
	Then there should be a breach with order ids "0"