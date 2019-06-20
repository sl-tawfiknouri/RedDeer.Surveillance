@highprofit
@highprofitabsolutecurrency
@highprofitabsolutesensitive
Feature: HighProfit Absolute Sensitive Currency Parameters
	In order to meet MAR compliance requirements
	I need to be able to detect when traders are executing trades
	Which generate unusual levels of profits
	By measuring their security trade profits as an absolute currency adjusted return

#High profit rule alerts have twice as many alerts to account for market closure analysis
#This is deduplicated elsewhere in the rules process

Background:
	Given I have the high profit rule parameter values
	| WindowHours | FutureHours | HighProfitPercentage | HighProfitAbsolute | HighProfitCurrency | HighProfitUseCurrencyConversions | PerformHighProfitWindowAnalysis | PerformHighProfitDailyAnalysis |
	| 1           |				|                      | 1000               | GBX                | true                             | true                            | false                          |

Scenario: Empty Universe yields no alerts
	Given I have the orders for a universe from 01/01/2018 to 03/01/2018 :
	| SecurityName | OrderId | PlacedDate          | BookedDate | AmendedDate | RejectedDate | CancelledDate | FilledDate          | Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
	When I run the high profit rule
	Then I will have 0 high profit alerts

Scenario: Single order yields no alerts
	Given I have the orders for a universe from 01/01/2018 to 03/01/2018 :
	| SecurityName | OrderId | PlacedDate          | BookedDate | AmendedDate | RejectedDate | CancelledDate | FilledDate          | Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
	| Vodafone     | 0       | 01/01/2018 09:30:00 |            |             |              |               | 01/01/2018 09:30:00 | MARKET | BUY       | USD      |            | 10               | 100           | 100          |
	When I run the high profit rule
	Then I will have 0 high profit alerts

Scenario: Buy Sell orders within the time window yields one alerts
	Given I have the orders for a universe from 01/01/2018 to 03/01/2018 :
	| SecurityName | OrderId | PlacedDate          | BookedDate | AmendedDate | RejectedDate | CancelledDate | FilledDate          | Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
	| Vodafone     | 0       | 01/01/2018 09:30:00 |            |             |              |               | 01/01/2018 09:30:00 | MARKET | BUY       | USD      |            | 10               | 1000           | 1000        |
	| Vodafone     | 1       | 01/01/2018 09:45:00 |            |             |              |               | 01/01/2018 09:45:00 | MARKET | SELL      | USD      |            | 12               | 1000           | 1000        |
	When I run the high profit rule
	Then I will have 2 high profit alerts

Scenario: Buy Sell orders next day but within the time window yields one alerts
	Given I have the high profit rule parameter values
	| WindowHours | HighProfitPercentage | HighProfitAbsolute | HighProfitCurrency | HighProfitUseCurrencyConversions | PerformHighProfitWindowAnalysis | PerformHighProfitDailyAnalysis |
	| 25           |                      | 1000               | GBX                | true	                          | true  | false |
	And I have the orders for a universe from 01/01/2018 to 03/01/2018 :
	| SecurityName | OrderId | PlacedDate          | BookedDate | AmendedDate | RejectedDate | CancelledDate | FilledDate          | Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
	| Vodafone     | 0       | 01/01/2018 09:30:00 |            |             |              |               | 01/01/2018 09:30:00 | MARKET | BUY       | USD      |            | 10               | 1000           | 1000        |
	| Vodafone     | 1       | 01/02/2018 09:45:00 |            |             |              |               | 01/02/2018 09:45:00 | MARKET | SELL      | USD      |            | 12               | 1000           | 1000        |
	When I run the high profit rule
	Then I will have 2 high profit alerts

Scenario: Buy Sell orders next day and outside the time window yields zero alerts
	Given I have the high profit rule parameter values
	| WindowHours | HighProfitPercentage | HighProfitAbsolute | HighProfitCurrency | HighProfitUseCurrencyConversions |
	| 23           |                      | 1000               | GBX                | true	                          |
	And I have the orders for a universe from 01/01/2018 to 03/01/2018 :
	| SecurityName | OrderId | PlacedDate          | BookedDate | AmendedDate | RejectedDate | CancelledDate | FilledDate          | Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
	| Vodafone     | 0       | 01/01/2018 09:30:00 |            |             |              |               | 01/01/2018 09:30:00 | MARKET | BUY       | USD      |            | 10               | 1000           | 1000        |
	| Vodafone     | 1       | 01/02/2018 09:45:00 |            |             |              |               | 01/02/2018 09:45:00 | MARKET | SELL      | USD      |            | 12               | 1000           | 1000        |
	When I run the high profit rule
	Then I will have 0 high profit alerts
	

Scenario: Buy Sell orders before market open within the time window yields one alerts
	Given I have the orders for a universe from 01/01/2018 to 03/01/2018 :
	| SecurityName | OrderId | PlacedDate          | BookedDate | AmendedDate | RejectedDate | CancelledDate | FilledDate          | Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
	| Vodafone     | 0       | 01/01/2018 05:30:00 |            |             |              |               | 01/01/2018 05:30:00 | MARKET | BUY       | USD      |            | 10               | 1000           | 1000        |
	| Vodafone     | 1       | 01/01/2018 05:45:00 |            |             |              |               | 01/01/2018 05:45:00 | MARKET | SELL      | USD      |            | 12               | 1000           | 1000        |
	When I run the high profit rule
	Then I will have 2 high profit alerts

Scenario: Buy Sell orders after market close within the time window yields one alerts
	Given I have the orders for a universe from 01/01/2018 to 03/01/2018 :
	| SecurityName | OrderId | PlacedDate          | BookedDate | AmendedDate | RejectedDate | CancelledDate | FilledDate          | Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
	| Vodafone     | 0       | 01/01/2018 19:30:00 |            |             |              |               | 01/01/2018 19:30:00 | MARKET | BUY       | USD      |            | 10               | 1000           | 1000        |
	| Vodafone     | 1       | 01/01/2018 19:45:00 |            |             |              |               | 01/01/2018 19:45:00 | MARKET | SELL      | USD      |            | 12               | 1000           | 1000        |
	When I run the high profit rule
	Then I will have 2 high profit alerts

Scenario: Buy Sell orders with losses within the time window yields zero alerts
	Given I have the orders for a universe from 01/01/2018 to 03/01/2018 :
	| SecurityName | OrderId | PlacedDate          | BookedDate | AmendedDate | RejectedDate | CancelledDate | FilledDate          | Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
	| Vodafone     | 0       | 01/01/2018 09:30:00 |            |             |              |               | 01/01/2018 09:30:00 | MARKET | BUY       | USD      |            | 10               | 1000           | 1000        |
	| Vodafone     | 1       | 01/01/2018 09:45:00 |            |             |              |               | 01/01/2018 09:45:00 | MARKET | SELL      | USD      |            | 2                | 1000           | 1000        |
	When I run the high profit rule
	Then I will have 0 high profit alerts

Scenario: Buy Sell partially filled orders within the time window yields one alerts
	Given I have the orders for a universe from 01/01/2018 to 03/01/2018 :
	| SecurityName | OrderId | PlacedDate          | BookedDate | AmendedDate | RejectedDate | CancelledDate | FilledDate          | Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
	| Vodafone     | 0       | 01/01/2018 09:30:00 |            |             |              |               | 01/01/2018 09:30:00 | MARKET | BUY       | USD      |            | 10               | 10000         | 1000        |
	| Vodafone     | 1       | 01/01/2018 09:45:00 |            |             |              |               | 01/01/2018 09:45:00 | MARKET | SELL      | USD      |            | 12               | 10000         | 1000        |
	When I run the high profit rule
	Then I will have 2 high profit alerts

Scenario: Buy Sell unfilled orders within the time window yields zero alerts
	Given I have the orders for a universe from 01/01/2018 to 03/01/2018 :
	| SecurityName | OrderId | PlacedDate          | BookedDate | AmendedDate | RejectedDate | CancelledDate | FilledDate          | Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
	| Vodafone     | 0       | 01/01/2018 09:30:00 |            |             |              |               |					   | MARKET | BUY       | USD      |            | 10               | 1000           |	          |
	| Vodafone     | 1       | 01/01/2018 09:45:00 |            |             |              |               |					   | MARKET | SELL      | USD      |            | 12               | 1000           |	          |
	When I run the high profit rule
	Then I will have 0 high profit alerts

Scenario: Buy Sell many orders within the time window yields two alerts
	Given I have the orders for a universe from 01/01/2018 to 03/01/2018 :
	| SecurityName | OrderId | PlacedDate          | BookedDate | AmendedDate | RejectedDate | CancelledDate | FilledDate          | Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
	| Vodafone     | 0       | 01/01/2018 09:30:00 |            |             |              |               | 01/01/2018 09:30:00 | MARKET | BUY       | USD      |            | 10               | 1000           | 1000        |
	| Vodafone     | 1       | 01/01/2018 09:45:00 |            |             |              |               | 01/01/2018 09:45:00 | MARKET | SELL      | USD      |            | 12               | 1000           | 1000        |
	| Nvidia       | 2       | 01/01/2018 09:30:00 |            |             |              |               | 01/01/2018 09:30:00 | MARKET | BUY       | USD      |            | 10               | 1000           | 1000        |
	| Nvidia       | 3       | 01/01/2018 09:45:00 |            |             |              |               | 01/01/2018 09:45:00 | MARKET | SELL      | USD      |            | 12               | 1000           | 1000        |
	| Barclays     | 4       | 01/01/2018 09:30:00 |            |             |              |               | 01/01/2018 09:30:00 | MARKET | BUY       | USD      |            | 10               | 1000           | 1000        |
	| Barclays     | 5       | 01/01/2018 09:45:00 |            |             |              |               | 01/01/2018 09:45:00 | MARKET | SELL      | USD      |            | 12               | 1000           | 1000        |
	When I run the high profit rule
	Then I will have 4 high profit alerts

Scenario: Buy Sell orders different exchange within the time window yields one alerts
	Given I have the orders for a universe from 01/01/2018 to 03/01/2018 :
	| SecurityName | OrderId | PlacedDate          | BookedDate | AmendedDate | RejectedDate | CancelledDate | FilledDate          | Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
	| Nvidia     | 0       | 01/01/2018 09:30:00 |            |             |              |               | 01/01/2018 09:30:00 | MARKET | BUY       | USD      |            | 10               | 1000           | 1000        |
	| Nvidia     | 1       | 01/01/2018 09:45:00 |            |             |              |               | 01/01/2018 09:45:00 | MARKET | SELL      | USD      |            | 12               | 1000           | 1000        |
	When I run the high profit rule
	Then I will have 2 high profit alerts

Scenario: Buy just buy orders within the time window yields one alerts
	Given I have the orders for a universe from 01/01/2018 to 03/01/2018 :
	| SecurityName | OrderId | PlacedDate          | BookedDate | AmendedDate | RejectedDate | CancelledDate | FilledDate          | Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
	| Vodafone     | 0       | 01/01/2018 09:30:00 |            |             |              |               | 01/01/2018 09:30:00 | MARKET | BUY       | USD      |            | 10               | 1000           | 1000        |
	| Vodafone     | 1       | 01/01/2018 09:45:00 |            |             |              |               | 01/01/2018 09:45:00 | MARKET | BUY      | USD      |            | 12               | 1000           | 1000        |
	And With the intraday market data :
	| SecurityName | Epoch	             | Bid | Ask | Price | Currency | Volume      |
	| Vodafone     | 01/01/2018 09:30:00 | 101 | 101 | 110   | USD      | 10000		  |
	When I run the high profit rule
	Then I will have 4 high profit alerts

Scenario: Cover just cover orders within the time window yields one alerts
	Given I have the orders for a universe from 01/01/2018 to 03/01/2018 :
	| SecurityName | OrderId | PlacedDate          | BookedDate | AmendedDate | RejectedDate | CancelledDate | FilledDate          | Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
	| Vodafone     | 0       | 01/01/2018 09:30:00 |            |             |              |               | 01/01/2018 09:30:00 | MARKET | COVER       | USD      |            | 10               | 1000           | 1000        |
	| Vodafone     | 1       | 01/01/2018 09:45:00 |            |             |              |               | 01/01/2018 09:45:00 | MARKET | COVER      | USD      |            | 12               | 1000           | 1000        |
	And With the intraday market data :
	| SecurityName | Epoch	             | Bid | Ask | Price | Currency | Volume      |
	| Vodafone     | 01/01/2018 09:30:00 | 101 | 101 | 110   | USD      | 10000		  |
	When I run the high profit rule
	Then I will have 4 high profit alerts

Scenario: Sell just sell orders within the time window yields zero alerts
	Given I have the orders for a universe from 01/01/2018 to 03/01/2018 :
	| SecurityName | OrderId | PlacedDate          | BookedDate | AmendedDate | RejectedDate | CancelledDate | FilledDate          | Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
	| Vodafone     | 0       | 01/01/2018 09:30:00 |            |             |              |               | 01/01/2018 09:30:00 | MARKET | SELL       | USD      |            | 10               | 1000           | 1000        |
	| Vodafone     | 1       | 01/01/2018 09:45:00 |            |             |              |               | 01/01/2018 09:45:00 | MARKET | SELL      | USD      |            | 12               | 1000           | 1000        |
	And With the intraday market data :
	| SecurityName | Epoch	             | Bid | Ask | Price | Currency | Volume      |
	| Vodafone     | 01/01/2018 09:30:00 | 101 | 101 | 110   | USD      | 10000		  |
	When I run the high profit rule
	Then I will have 0 high profit alerts

Scenario: Short just short orders within the time window yields zero alerts
	Given I have the orders for a universe from 01/01/2018 to 03/01/2018 :
	| SecurityName | OrderId | PlacedDate          | BookedDate | AmendedDate | RejectedDate | CancelledDate | FilledDate          | Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
	| Vodafone     | 0       | 01/01/2018 09:30:00 |            |             |              |               | 01/01/2018 09:30:00 | MARKET | SHORT       | USD      |            | 10               | 1000           | 1000        |
	| Vodafone     | 1       | 01/01/2018 09:45:00 |            |             |              |               | 01/01/2018 09:45:00 | MARKET | SHORT      | USD      |            | 12               | 1000           | 1000        |
	And With the intraday market data :
	| SecurityName | Epoch	             | Bid | Ask | Price | Currency | Volume      |
	| Vodafone     | 01/01/2018 09:30:00 | 101 | 101 | 110   | USD      | 10000		  |
		When I run the high profit rule
	Then I will have 0 high profit alerts

Scenario: Buy Sell orders exactly on the time window yields one alerts
	Given I have the orders for a universe from 01/01/2018 to 03/01/2018 :
	| SecurityName | OrderId | PlacedDate          | BookedDate | AmendedDate | RejectedDate | CancelledDate | FilledDate          | Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
	| Vodafone     | 0       | 01/01/2018 09:30:00 |            |             |              |               | 01/01/2018 09:30:00 | MARKET | BUY       | USD      |            | 10               | 1000           | 1000        |
	| Vodafone     | 1       | 01/01/2018 10:30:00 |            |             |              |               | 01/01/2018 10:30:00 | MARKET | SELL      | USD      |            | 12               | 1000           | 1000        |
	When I run the high profit rule
	Then I will have 2 high profit alerts

Scenario: Buy Sell orders outside of the time window yields two alerts
	Given I have the orders for a universe from 01/01/2018 to 03/01/2018 :
	| SecurityName | OrderId | PlacedDate          | BookedDate | AmendedDate | RejectedDate | CancelledDate | FilledDate          | Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
	| Vodafone     | 0       | 01/01/2018 09:30:00 |            |             |              |               | 01/01/2018 09:30:00 | MARKET | BUY       | USD      |            | 10               | 1000           | 1000        |
	| Vodafone     | 1       | 01/01/2018 11:30:00 |            |             |              |               | 01/01/2018 11:30:00 | MARKET | SELL      | USD      |            | 12               | 1000           | 1000        |
	When I run the high profit rule
	Then I will have 0 high profit alerts

Scenario: Buy Sell orders yields one alerts
	Given I have the orders for a universe from 01/01/2018 to 03/01/2018 :
	| SecurityName | OrderId | PlacedDate          | BookedDate | AmendedDate | RejectedDate | CancelledDate | FilledDate          | Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
	| Vodafone     | 0       | 01/01/2018 09:30:00 |            |             |              |               | 01/01/2018 09:30:00 | MARKET | BUY       | USD      |            | 10               | 1000           | 1000        |
	| Vodafone     | 1       | 01/01/2018 09:30:00 |            |             |              |               | 01/01/2018 09:30:00 | MARKET | SELL      | USD      |            | 12               | 1000           | 1000        |
	When I run the high profit rule
	Then I will have 2 high profit alerts

Scenario: Buy Sell orders at exact threshold yields two alerts
	Given I have the orders for a universe from 01/01/2018 to 03/01/2018 :
	| SecurityName | OrderId | PlacedDate          | BookedDate | AmendedDate | RejectedDate | CancelledDate | FilledDate          | Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
	| Vodafone     | 0       | 01/01/2018 09:30:00 |            |             |              |               | 01/01/2018 09:30:00 | MARKET | BUY       | USD      |            | 100              | 1000          | 1000        |
	| Vodafone     | 1       | 01/01/2018 09:30:00 |            |             |              |               | 01/01/2018 09:30:00 | MARKET | SELL      | USD      |            | 110              | 1000          | 1000        |
	When I run the high profit rule
	Then I will have 2 high profit alerts

Scenario: Buy Sell orders at just below threshold yields zero alerts
	Given I have the orders for a universe from 01/01/2018 to 03/01/2018 :
	| SecurityName | OrderId | PlacedDate          | BookedDate | AmendedDate | RejectedDate | CancelledDate | FilledDate          | Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
	| Vodafone     | 0       | 01/01/2018 09:30:00 |            |             |              |               | 01/01/2018 09:30:00 | MARKET | BUY       | USD      |            | 10             | 4            | 4           |
	| Vodafone     | 1       | 01/01/2018 09:30:00 |            |             |              |               | 01/01/2018 09:30:00 | MARKET | SELL      | USD      |            | 11             | 4            | 4           |
	When I run the high profit rule
	Then I will have 0 high profit alerts

Scenario: Buy order with increase in market price (bmll) yields one alert
	Given I have the orders for a universe from 01/01/2018 to 03/01/2018 :
	| SecurityName | OrderId | PlacedDate          | BookedDate | AmendedDate | RejectedDate | CancelledDate | FilledDate          | Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
	| Vodafone     | 0       | 01/01/2018 09:30:00 |            |             |              |               | 01/01/2018 09:30:00 | MARKET | BUY       | USD      |            | 100              | 100           | 100          |
	And With the intraday market data :
	| SecurityName | Epoch	             | Bid | Ask | Price | Currency | Volume      |
	| Vodafone     | 01/01/2018 09:30:00 | 101 | 101 | 110   | USD      | 10000		  |
	When I run the high profit rule
	Then I will have 2 high profit alerts

Scenario: Buy order with increase in market price to exact percentage (bmll) yields one alert
	Given I have the orders for a universe from 01/01/2018 to 03/01/2018 :
	| SecurityName | OrderId | PlacedDate          | BookedDate | AmendedDate | RejectedDate | CancelledDate | FilledDate          | Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
	| Vodafone     | 0       | 01/01/2018 09:30:00 |            |             |              |               | 01/01/2018 09:30:00 | MARKET | BUY       | USD      |            | 100              | 100           | 100          |
	And With the intraday market data :
	| SecurityName | Epoch	             | Bid | Ask | Price | Currency | Volume      |
	| Vodafone     | 01/01/2018 09:30:00 | 101 | 101 | 110   | USD      | 10000		  |
	When I run the high profit rule
	Then I will have 2 high profit alerts

Scenario: Buy order with substantial increase in market price (bmll) yields one alert
	Given I have the orders for a universe from 01/01/2018 to 03/01/2018 :
	| SecurityName | OrderId | PlacedDate          | BookedDate | AmendedDate | RejectedDate | CancelledDate | FilledDate          | Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
	| Vodafone     | 0       | 01/01/2018 09:30:00 |            |             |              |               | 01/01/2018 09:30:00 | MARKET | BUY       | USD      |            | 100              | 100           | 100          |
	And With the intraday market data :
	| SecurityName | Epoch	             | Bid | Ask | Price | Currency | Volume      |
	| Vodafone     | 01/01/2018 09:30:00 | 101 | 101 | 110   | USD      | 10000		  |
	When I run the high profit rule
	Then I will have 2 high profit alerts

Scenario: Buy order with decrease in market price (bmll) yields zero alerts
	Given I have the orders for a universe from 01/01/2018 to 03/01/2018 :
	| SecurityName | OrderId | PlacedDate          | BookedDate | AmendedDate | RejectedDate | CancelledDate | FilledDate          | Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
	| Vodafone     | 0       | 01/01/2018 09:30:00 |            |             |              |               | 01/01/2018 09:30:00 | MARKET | BUY       | USD      |            | 100              | 100           | 100          |
	And With the intraday market data :
	| SecurityName | Epoch	             | Bid | Ask | Price | Currency | Volume      |
	| Vodafone     | 01/01/2018 09:30:00 | 101 | 101 | 98    | USD      | 10000		  |
	When I run the high profit rule
	Then I will have 0 high profit alerts