@fixedincomehighprofit
@fixedincomehighprofitpercentage
@fixedincomehighprofitpercentagenonsensitive
Feature: Fixed Income high profit Percentage Non Sensitive
	In order to meet MAR compliance requirements
	I need to be able to detect when traders are executing trades
	Which generate unusual levels of profits
	By measuring their security trade profits as a percentage return

Background:
			Given I have the fixed income high profit rule parameter values
			| WindowHours | HighProfitPercentage |  PerformHighProfitDailyAnalysis | PerformHighProfitWindowAnalysis |
			| 1           | 0.1                  | false						   | true							 |

Scenario: Empty Universe yields no alerts
	Given I have the orders for a universe from 01/01/2018 to 03/01/2018 :
	| SecurityName | OrderId | PlacedDate          | BookedDate | AmendedDate | RejectedDate | CancelledDate | FilledDate          | Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume | CleanOrDirty |
	When I run the fixed income high profit rule
	Then I will have 0 fixed income high profit alerts

Scenario: Single order yields no alerts
	Given I have the orders for a universe from 01/01/2018 to 03/01/2018 :
	| SecurityName | OrderId | PlacedDate          | BookedDate | AmendedDate | RejectedDate | CancelledDate | FilledDate          | Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume | CleanOrDirty |
	| UKGovtBondSecondaryMarket     | 0       | 01/01/2018 09:30:00 |            |             |              |               | 01/01/2018 09:30:00 | MARKET | BUY       | GBX      |            | 10               | 100           | 100          | Clean		|
	When I run the fixed income high profit rule
	Then I will have 0 fixed income high profit alerts

Scenario: Buy Sell orders within the time window yields two alerts
	Given I have the orders for a universe from 01/01/2018 to 03/01/2018 :
	| SecurityName | OrderId | PlacedDate          | BookedDate | AmendedDate | RejectedDate | CancelledDate | FilledDate          | Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume | CleanOrDirty |
	| UKGovtBondSecondaryMarket     | 0       | 01/01/2018 09:30:00 |            |             |              |               | 01/01/2018 09:30:00 | MARKET | BUY       | GBX      |            | 10               | 100           | 100          | Clean |
	| UKGovtBondSecondaryMarket     | 1       | 01/01/2018 09:31:00 |            |             |              |               | 01/01/2018 09:31:00 | MARKET | SELL      | GBX      |            | 12               | 100           | 100          | Clean |
	When I run the fixed income high profit rule
	Then I will have 3 fixed income high profit alerts

Scenario: Buy Sell orders next day but within the time window yields two alerts
	Given I have the fixed income high profit rule parameter values
	| WindowHours | HighProfitPercentage | HighProfitAbsolute | HighProfitCurrency | HighProfitUseCurrencyConversions | PerformHighProfitWindowAnalysis | PerformHighProfitDailyAnalysis |
	| 25           | 0.1                 |                    |                    |                                  | true | false |
	And I have the orders for a universe from 01/01/2018 to 03/01/2018 :
	| SecurityName | OrderId | PlacedDate          | BookedDate | AmendedDate | RejectedDate | CancelledDate | FilledDate          | Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume | CleanOrDirty |
	| UKGovtBondSecondaryMarket     | 0       | 01/01/2018 09:30:00 |            |             |              |               | 01/01/2018 09:30:00 | MARKET | BUY       | GBX      |            | 10               | 100           | 100          | Clean |
	| UKGovtBondSecondaryMarket     | 1       | 01/02/2018 09:31:00 |            |             |              |               | 01/02/2018 09:31:00 | MARKET | SELL      | GBX      |            | 12               | 100           | 100          | Clean |
	When I run the fixed income high profit rule
	Then I will have 3 fixed income high profit alerts

Scenario: Buy Sell orders next day and outside the time window yields zero alerts
	Given I have the fixed income high profit rule parameter values
	| WindowHours | HighProfitPercentage | HighProfitAbsolute | HighProfitCurrency | HighProfitUseCurrencyConversions |
	| 23           | 0.1                 |                    |                    |                                  |
	And I have the orders for a universe from 01/01/2018 to 03/01/2018 :
	| SecurityName | OrderId | PlacedDate          | BookedDate | AmendedDate | RejectedDate | CancelledDate | FilledDate          | Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume | CleanOrDirty |
	| UKGovtBondSecondaryMarket     | 0       | 01/01/2018 09:30:00 |            |             |              |               | 01/01/2018 09:30:00 | MARKET | BUY       | GBX      |            | 10               | 100           | 100          | Clean |
	| UKGovtBondSecondaryMarket     | 1       | 01/02/2018 09:31:00 |            |             |              |               | 01/02/2018 09:31:00 | MARKET | SELL      | GBX      |            | 12               | 100           | 100          | Clean |
	When I run the fixed income high profit rule
	Then I will have 0 fixed income high profit alerts



Scenario: Buy Sell orders before market open within the time window yields two alerts
	Given I have the orders for a universe from 01/01/2018 to 03/01/2018 :
	| SecurityName | OrderId | PlacedDate          | BookedDate | AmendedDate | RejectedDate | CancelledDate | FilledDate          | Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume | CleanOrDirty |
	| UKGovtBondSecondaryMarket     | 0       | 01/01/2018 05:30:00 |            |             |              |               | 01/01/2018 05:30:00 | MARKET | BUY       | GBX      |            | 10               | 100           | 100          | Clean |
	| UKGovtBondSecondaryMarket     | 1       | 01/01/2018 05:31:00 |            |             |              |               | 01/01/2018 05:31:00 | MARKET | SELL      | GBX      |            | 12               | 100           | 100          | Clean |
	When I run the fixed income high profit rule
	Then I will have 3 fixed income high profit alerts

Scenario: Buy Sell orders after market close within the time window yields two alerts
	Given I have the orders for a universe from 01/01/2018 to 03/01/2018 :
	| SecurityName | OrderId | PlacedDate          | BookedDate | AmendedDate | RejectedDate | CancelledDate | FilledDate          | Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume | CleanOrDirty |
	| UKGovtBondSecondaryMarket     | 0       | 01/01/2018 19:30:00 |            |             |              |               | 01/01/2018 19:30:00 | MARKET | BUY       | GBX      |            | 10               | 100           | 100          | Clean |
	| UKGovtBondSecondaryMarket     | 1       | 01/01/2018 19:31:00 |            |             |              |               | 01/01/2018 19:31:00 | MARKET | SELL      | GBX      |            | 12               | 100           | 100          | Clean |
	When I run the fixed income high profit rule
	Then I will have 3 fixed income high profit alerts


Scenario: Buy Sell orders with losses within the time window yields zero alerts
	Given I have the orders for a universe from 01/01/2018 to 03/01/2018 :
	| SecurityName | OrderId | PlacedDate          | BookedDate | AmendedDate | RejectedDate | CancelledDate | FilledDate          | Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume | CleanOrDirty |
	| UKGovtBondSecondaryMarket     | 0       | 01/01/2018 09:30:00 |            |             |              |               | 01/01/2018 09:30:00 | MARKET | BUY       | GBX      |            | 10               | 100           | 100          | Clean |
	| UKGovtBondSecondaryMarket     | 1       | 01/01/2018 09:31:00 |            |             |              |               | 01/01/2018 09:31:00 | MARKET | SELL      | GBX      |            | 9                | 100           | 100          | Clean |
	When I run the fixed income high profit rule
	Then I will have 0 fixed income high profit alerts

Scenario: Buy Sell partially filled orders within the time window yields two alerts
	Given I have the orders for a universe from 01/01/2018 to 03/01/2018 :
	| SecurityName | OrderId | PlacedDate          | BookedDate | AmendedDate | RejectedDate | CancelledDate | FilledDate          | Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume | CleanOrDirty |
	| UKGovtBondSecondaryMarket     | 0       | 01/01/2018 09:30:00 |            |             |              |               | 01/01/2018 09:30:00 | MARKET | BUY       | GBX      |            | 10               | 1000          | 100          | Clean |
	| UKGovtBondSecondaryMarket     | 1       | 01/01/2018 09:31:00 |            |             |              |               | 01/01/2018 09:31:00 | MARKET | SELL      | GBX      |            | 12               | 1000          | 100          | Clean |
	When I run the fixed income high profit rule
	Then I will have 3 fixed income high profit alerts

Scenario: Buy Sell unfilled orders within the time window yields zero alerts
	Given I have the orders for a universe from 01/01/2018 to 03/01/2018 :
	| SecurityName | OrderId | PlacedDate          | BookedDate | AmendedDate | RejectedDate | CancelledDate | FilledDate          | Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume | CleanOrDirty |
	| UKGovtBondSecondaryMarket     | 0       | 01/01/2018 09:30:00 |            |             |              |               |					   | MARKET | BUY       | GBX      |            | 10               | 100           |	          | Clean |
	| UKGovtBondSecondaryMarket     | 1       | 01/01/2018 09:31:00 |            |             |              |               |					   | MARKET | SELL      | GBX      |            | 12               | 100           |	          | Clean |
	When I run the fixed income high profit rule
	Then I will have 0 fixed income high profit alerts

Scenario: Buy Sell many orders within the time window yields six alerts
	Given I have the orders for a universe from 01/01/2018 to 03/01/2018 :
	| SecurityName | OrderId | PlacedDate          | BookedDate | AmendedDate | RejectedDate | CancelledDate | FilledDate          | Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume | CleanOrDirty |
	| UKGovtBondSecondaryMarket     | 0       | 01/01/2018 09:30:00 |            |             |              |               | 01/01/2018 09:30:00 | MARKET | BUY       | GBX      |            | 10               | 100           | 100          | Clean |
	| UKGovtBondSecondaryMarket     | 1       | 01/01/2018 09:31:00 |            |             |              |               | 01/01/2018 09:31:00 | MARKET | SELL      | GBX      |            | 12               | 100           | 100          | Clean |
	| UKGovtBondSecondaryMarket       | 2       | 01/01/2018 09:30:00 |            |             |              |               | 01/01/2018 09:30:00 | MARKET | BUY       | GBX      |            | 10               | 100           | 100          | Clean |
	| UKGovtBondSecondaryMarket       | 3       | 01/01/2018 09:31:00 |            |             |              |               | 01/01/2018 09:31:00 | MARKET | SELL      | GBX      |            | 12               | 100           | 100          | Clean |
	| UKGovtBondSecondaryMarket     | 4       | 01/01/2018 09:30:00 |            |             |              |               | 01/01/2018 09:30:00 | MARKET | BUY       | GBX      |            | 10               | 100           | 100          | Clean |
	| UKGovtBondSecondaryMarket     | 5       | 01/01/2018 09:31:00 |            |             |              |               | 01/01/2018 09:31:00 | MARKET | SELL      | GBX      |            | 12               | 100           | 100          | Clean |
	When I run the fixed income high profit rule
	Then I will have 3 fixed income high profit alerts

Scenario: Buy Sell orders different exchange within the time window yields two alerts
	Given I have the orders for a universe from 01/01/2018 to 03/01/2018 :
	| SecurityName | OrderId | PlacedDate          | BookedDate | AmendedDate | RejectedDate | CancelledDate | FilledDate          | Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume | CleanOrDirty |
	| UKGovtBondSecondaryMarket     | 0       | 01/01/2018 09:30:00 |            |             |              |               | 01/01/2018 09:30:00 | MARKET | BUY       | GBX      |            | 10               | 100           | 100          | Clean |
	| UKGovtBondSecondaryMarket     | 1       | 01/01/2018 09:31:00 |            |             |              |               | 01/01/2018 09:31:00 | MARKET | SELL      | GBX      |            | 12               | 100           | 100          | Clean |
	When I run the fixed income high profit rule
	Then I will have 3 fixed income high profit alerts

Scenario: Buy just buy orders within the time window yields two alerts
	Given I have the orders for a universe from 01/01/2018 to 03/01/2018 :
	| SecurityName | OrderId | PlacedDate          | BookedDate | AmendedDate | RejectedDate | CancelledDate | FilledDate          | Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume | CleanOrDirty|
	| UKGovtBondSecondaryMarket     | 0       | 01/01/2018 09:30:00 |            |             |              |               | 01/01/2018 09:30:00 | MARKET | BUY       | GBX      |            | 10               | 100           | 100          | Clean |
	| UKGovtBondSecondaryMarket     | 1       | 01/01/2018 09:31:00 |            |             |              |               | 01/01/2018 09:31:00 | MARKET | BUY      | GBX      |            | 12               | 100           | 100          | Clean |
	And With the fixed income intraday market data :
	| SecurityName | Epoch	             | Bid | Ask | Price | Currency | Volume      |
	| UKGovtBondSecondaryMarket     | 01/01/2018 09:29:00 | 101 | 101 | 110   | GBX      | 10000		  |
	When I run the fixed income high profit rule
	Then I will have 4 fixed income high profit alerts

Scenario: Cover just cover orders within the time window yields two alerts
	Given I have the orders for a universe from 01/01/2018 to 03/01/2018 :
	| SecurityName | OrderId | PlacedDate          | BookedDate | AmendedDate | RejectedDate | CancelledDate | FilledDate          | Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |  CleanOrDirty |
	| UKGovtBondSecondaryMarket     | 0       | 01/01/2018 09:30:00 |            |             |              |               | 01/01/2018 09:30:00 | MARKET | COVER       | GBX      |            | 10               | 100           | 100          | Clean |
	| UKGovtBondSecondaryMarket     | 1       | 01/01/2018 09:31:00 |            |             |              |               | 01/01/2018 09:31:00 | MARKET | COVER       | GBX      |            | 12               | 100           | 100          | Clean |
	And With the fixed income intraday market data :
	| SecurityName | Epoch	             | Bid | Ask | Price | Currency | Volume      |
	| UKGovtBondSecondaryMarket     | 01/01/2018 09:30:00 | 101 | 101 | 110   | GBX      | 10000		  |
	When I run the fixed income high profit rule
	Then I will have 4 fixed income high profit alerts

Scenario: Sell just sell orders within the time window yields zero alerts
	Given I have the orders for a universe from 01/01/2018 to 03/01/2018 :
	| SecurityName | OrderId | PlacedDate          | BookedDate | AmendedDate | RejectedDate | CancelledDate | FilledDate          | Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume | CleanOrDirty |
	| UKGovtBondSecondaryMarket     | 0       | 01/01/2018 09:30:00 |            |             |              |               | 01/01/2018 09:30:00 | MARKET | SELL       | GBX      |            | 10               | 100           | 100          | Clean |
	| UKGovtBondSecondaryMarket     | 1       | 01/01/2018 09:31:00 |            |             |              |               | 01/01/2018 09:31:00 | MARKET | SELL      | GBX      |            | 12               | 100           | 100          | Clean |
	And With the fixed income intraday market data :
	| SecurityName | Epoch	             | Bid | Ask | Price | Currency | Volume      |
	| UKGovtBondSecondaryMarket     | 01/01/2018 09:30:00 | 101 | 101 | 110   | GBX      | 10000		  |
	When I run the fixed income high profit rule
	Then I will have 0 fixed income high profit alerts

Scenario: Short just short orders within the time window yields zero alerts
	Given I have the orders for a universe from 01/01/2018 to 03/01/2018 :
	| SecurityName | OrderId | PlacedDate          | BookedDate | AmendedDate | RejectedDate | CancelledDate | FilledDate          | Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |  CleanOrDirty |
	| UKGovtBondSecondaryMarket     | 0       | 01/01/2018 09:30:00 |            |             |              |               | 01/01/2018 09:30:00 | MARKET | SHORT       | GBX      |            | 10               | 100           | 100          | Clean |
	| UKGovtBondSecondaryMarket     | 1       | 01/01/2018 09:31:00 |            |             |              |               | 01/01/2018 09:31:00 | MARKET | SHORT      | GBX      |            | 12               | 100           | 100          | Clean |
	And With the fixed income intraday market data :
	| SecurityName | Epoch	             | Bid | Ask | Price | Currency | Volume      |
	| UKGovtBondSecondaryMarket     | 01/01/2018 09:30:00 | 101 | 101 | 110   | GBX      | 10000		  |
	When I run the fixed income high profit rule
	Then I will have 0 fixed income high profit alerts

Scenario: Buy Sell orders exactly on the time window yields two alerts
	Given I have the orders for a universe from 01/01/2018 to 03/01/2018 :
	| SecurityName | OrderId | PlacedDate          | BookedDate | AmendedDate | RejectedDate | CancelledDate | FilledDate          | Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume | CleanOrDirty |
	| UKGovtBondSecondaryMarket     | 0       | 01/01/2018 09:30:00 |            |             |              |               | 01/01/2018 09:30:00 | MARKET | BUY       | GBX      |            | 10               | 100           | 100          | Clean |
	| UKGovtBondSecondaryMarket     | 1       | 01/01/2018 10:30:00 |            |             |              |               | 01/01/2018 10:30:00 | MARKET | SELL      | GBX      |            | 12               | 100           | 100          | Clean |
	When I run the fixed income high profit rule
	Then I will have 3 fixed income high profit alerts

Scenario: Buy Sell orders outside of the time window yields zero alerts
	Given I have the orders for a universe from 01/01/2018 to 03/01/2018 :
	| SecurityName | OrderId | PlacedDate          | BookedDate | AmendedDate | RejectedDate | CancelledDate | FilledDate          | Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume | CleanOrDirty |
	| UKGovtBondSecondaryMarket     | 0       | 01/01/2018 09:30:00 |            |             |              |               | 01/01/2018 09:30:00 | MARKET | BUY       | GBX      |            | 10               | 100           | 100          | Clean |
	| UKGovtBondSecondaryMarket     | 1       | 01/01/2018 11:30:00 |            |             |              |               | 01/01/2018 11:30:00 | MARKET | SELL      | GBX      |            | 12               | 100           | 100          | Clean |
	When I run the fixed income high profit rule
	Then I will have 0 fixed income high profit alerts

Scenario: Buy Sell orders yields two alerts
	Given I have the orders for a universe from 01/01/2018 to 03/01/2018 :
	| SecurityName | OrderId | PlacedDate          | BookedDate | AmendedDate | RejectedDate | CancelledDate | FilledDate          | Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |  CleanOrDirty |
	| UKGovtBondSecondaryMarket     | 0       | 01/01/2018 09:30:00 |            |             |              |               | 01/01/2018 09:30:00 | MARKET | BUY       | GBX      |            | 10               | 100           | 100          | Clean |
	| UKGovtBondSecondaryMarket     | 1       | 01/01/2018 09:30:00 |            |             |              |               | 01/01/2018 09:30:00 | MARKET | SELL      | GBX      |            | 12               | 100           | 100          | Clean |
	When I run the fixed income high profit rule
	Then I will have 3 fixed income high profit alerts

Scenario: Buy Sell orders at exact percentage yields two alerts
	Given I have the orders for a universe from 01/01/2018 to 03/01/2018 :
	| SecurityName | OrderId | PlacedDate          | BookedDate | AmendedDate | RejectedDate | CancelledDate | FilledDate          | Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |  CleanOrDirty |
	| UKGovtBondSecondaryMarket     | 0       | 01/01/2018 09:30:00 |            |             |              |               | 01/01/2018 09:30:00 | MARKET | BUY       | GBX      |            | 100              | 100           | 100          | Clean |
	| UKGovtBondSecondaryMarket     | 1       | 01/01/2018 09:30:00 |            |             |              |               | 01/01/2018 09:30:00 | MARKET | SELL      | GBX      |            | 110               | 100           | 100          | Clean |
	When I run the fixed income high profit rule
	Then I will have 3 fixed income high profit alerts

Scenario: Buy Sell orders at just below percentage yields zero alerts
	Given I have the orders for a universe from 01/01/2018 to 03/01/2018 :
	| SecurityName | OrderId | PlacedDate          | BookedDate | AmendedDate | RejectedDate | CancelledDate | FilledDate          | Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |  CleanOrDirty |
	| UKGovtBondSecondaryMarket     | 0       | 01/01/2018 09:30:00 |            |             |              |               | 01/01/2018 09:30:00 | MARKET | BUY       | GBX      |            | 1000             | 100           | 100          | Clean |
	| UKGovtBondSecondaryMarket     | 1       | 01/01/2018 09:30:00 |            |             |              |               | 01/01/2018 09:30:00 | MARKET | SELL      | GBX      |            | 1001             | 100           | 100          | Clean |
	When I run the fixed income high profit rule
	Then I will have 0 fixed income high profit alerts

Scenario: Buy order with increase in market price (bmll) yields one alert
	Given I have the orders for a universe from 01/01/2018 to 03/01/2018 :
	| SecurityName | OrderId | PlacedDate          | BookedDate | AmendedDate | RejectedDate | CancelledDate | FilledDate          | Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume | CleanOrDirty |
	| UKGovtBondSecondaryMarket     | 0       | 01/01/2018 09:30:00 |            |             |              |               | 01/01/2018 09:30:00 | MARKET | BUY       | GBX      |            | 100              | 100           | 100          | Clean |
	And With the fixed income intraday market data :
	| SecurityName | Epoch	             | Bid | Ask | Price | Currency | Volume      |
	| UKGovtBondSecondaryMarket     | 01/01/2018 09:30:00 | 101 | 101 | 110   | GBX      | 10000		  |
	When I run the fixed income high profit rule
	Then I will have 2 fixed income high profit alerts

Scenario: Buy order with increase in market price to exact percentage (bmll) yields one alert
	Given I have the orders for a universe from 01/01/2018 to 03/01/2018 :
	| SecurityName | OrderId | PlacedDate          | BookedDate | AmendedDate | RejectedDate | CancelledDate | FilledDate          | Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |  CleanOrDirty |
	| UKGovtBondSecondaryMarket     | 0       | 01/01/2018 09:30:00 |            |             |              |               | 01/01/2018 09:30:00 | MARKET | BUY       | GBX      |            | 100              | 100           | 100          | Clean |
	And With the fixed income intraday market data :
	| SecurityName | Epoch	             | Bid | Ask | Price | Currency | Volume      |
	| UKGovtBondSecondaryMarket     | 01/01/2018 09:30:00 | 101 | 101 | 110   | GBX      | 10000		  |
	When I run the fixed income high profit rule
	Then I will have 2 fixed income high profit alerts

Scenario: Buy order with substantial increase in market price (bmll) yields one alert
	Given I have the orders for a universe from 01/01/2018 to 03/01/2018 :
	| SecurityName | OrderId | PlacedDate          | BookedDate | AmendedDate | RejectedDate | CancelledDate | FilledDate          | Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |  CleanOrDirty |
	| UKGovtBondSecondaryMarket     | 0       | 01/01/2018 09:30:00 |            |             |              |               | 01/01/2018 09:30:00 | MARKET | BUY       | GBX      |            | 100              | 100           | 100          | Clean |
	And With the fixed income intraday market data :
	| SecurityName | Epoch	             | Bid | Ask | Price | Currency | Volume      |
	| UKGovtBondSecondaryMarket     | 01/01/2018 09:30:00 | 101 | 101 | 110   | GBX      | 10000		  |
	When I run the fixed income high profit rule
	Then I will have 2 fixed income high profit alerts

Scenario: Buy order with decrease in market price (bmll) yields zero alerts
	Given I have the orders for a universe from 01/01/2018 to 03/01/2018 :
	| SecurityName | OrderId | PlacedDate          | BookedDate | AmendedDate | RejectedDate | CancelledDate | FilledDate          | Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |  CleanOrDirty |
	| UKGovtBondSecondaryMarket     | 0       | 01/01/2018 09:30:00 |            |             |              |               | 01/01/2018 09:30:00 | MARKET | BUY       | GBX      |            | 100              | 100           | 100          | Clean |
	And With the fixed income intraday market data :
	| SecurityName | Epoch	             | Bid | Ask | Price | Currency | Volume      |
	| UKGovtBondSecondaryMarket     | 01/01/2018 09:30:00 | 101 | 101 | 98    | GBX      | 10000		  |
	When I run the fixed income high profit rule
	Then I will have 0 fixed income high profit alerts