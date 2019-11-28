Feature: WashTrade
	Further description

Scenario: Basic wash trade rule run
	Given the wash trade core settings
	| TimeWindow | AnalysePositionsBy | MinNumberOfTrades | MaxValueChangePercent |
	| 1 day      | Clustering         | 2                 | 1                     |
	And the orders
	| OrderId | _Date               | _EquitySecurity | OrderDirection | OrderAverageFillPrice | _Volume |
	| 0       | 2018-01-02T15:00:00 | VODAFONE        | BUY            | 3                     | 100     |
	| 1       | 2018-01-02T15:00:00 | VODAFONE        | SELL           | 3                     | 100     |
	When the rule is run between "2018-01-01" and "2018-01-02"
	Then there should be a breach with order ids "0, 1"
