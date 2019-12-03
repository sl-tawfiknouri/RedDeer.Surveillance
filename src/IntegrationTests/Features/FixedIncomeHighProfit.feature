Feature: FixedIncomeHighProfit
		Further description

Scenario: Basic fixed income high profit rule run
	Given the fixed income high profit core settings
	| ProfitThresholdPercent | PriceType | BackwardWindow | ForwardWindow |
	| 20                     | Close     | 1 day          | 1 day         |
	And the fixed income orders
	| OrderId | _Date               | _EquitySecurity | OrderDirection | OrderAverageFillPrice | _Volume |
	| 0       | 2018-01-01T15:00:00 | VODAFONE        | BUY            | 100                   | 100     |
	And the fixed income close prices
	| Date       | _EquitySecurity | ClosePrice |
	| 2018-01-01 | VODAFONE        | 130        |
	When the rule is run between "2018-01-01" and "2018-01-01"
	Then there should be a breach with order ids "0"