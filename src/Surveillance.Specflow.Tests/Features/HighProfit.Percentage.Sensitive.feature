﻿@highprofit
@highprofitpercentage
@highprofitpercentagesensitive
Feature: HighProfit Percentage Sensitive Parameters
	In order to meet MAR compliance requirements
	I need to be able to detect when traders are executing trades
	Which generate unusual levels of profits
	By measuring their security trade profits as a percentage return

#High profit rule alerts have twice as many alerts to account for market closure analysis
#This is deduplicated elsewhere in the rules process

Background:
	Given I have the high profit rule parameter values
	| WindowHours | FutureHours | HighProfitPercentage | HighProfitAbsolute | HighProfitCurrency | HighProfitUseCurrencyConversions | PerformHighProfitWindowAnalysis | PerformHighProfitDailyAnalysis |
	| 1           |				| 0.01                 |                    |                    |                                  | true                            | false                          |

Scenario: Empty Universe yields no alerts
	Given I have the orders for a universe from 01/01/2018 to 03/01/2018 :
	| SecurityName | OrderId | PlacedDate          | BookedDate | AmendedDate | RejectedDate | CancelledDate | FilledDate          | Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
	When I run the high profit rule
	Then I will have 0 high profit alerts

Scenario: Single order yields no alerts
	Given I have the orders for a universe from 01/01/2018 to 03/01/2018 :
	| SecurityName | OrderId | PlacedDate          | BookedDate | AmendedDate | RejectedDate | CancelledDate | FilledDate          | Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
	| Vodafone     | 0       | 01/01/2018 09:30:00 |            |             |              |               | 01/01/2018 09:30:00 | MARKET | BUY       | GBX      |            | 10               | 100           | 100          |
	When I run the high profit rule
	Then I will have 0 high profit alerts

Scenario: Buy Sell orders within time window yields two alerts
	Given I have the orders for a universe from 01/01/2018 to 03/01/2018 :
	| SecurityName | OrderId | PlacedDate          | BookedDate | AmendedDate | RejectedDate | CancelledDate | FilledDate          | Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
	| Vodafone     | 0       | 01/01/2018 09:30:00 |            |             |              |               | 01/01/2018 09:30:00 | MARKET | BUY       | GBX      |            | 10               | 100           | 100          |
	| Vodafone     | 1       | 01/01/2018 09:55:00 |            |             |              |               | 01/01/2018 09:55:00 | MARKET | SELL      | GBX      |            | 12               | 100           | 100          |
	When I run the high profit rule
	Then I will have 3 high profit alerts

	
Scenario: Buy Sell orders next day but within time window yields two alerts
	Given I have the high profit rule parameter values
	| WindowHours | HighProfitPercentage | HighProfitAbsolute | HighProfitCurrency | HighProfitUseCurrencyConversions |  PerformHighProfitWindowAnalysis | PerformHighProfitDailyAnalysis |
	| 25           | 0.01                 |                    |                    |                                  | true | false |
	And I have the orders for a universe from 01/01/2018 to 03/01/2018 :
	| SecurityName | OrderId | PlacedDate          | BookedDate | AmendedDate | RejectedDate | CancelledDate | FilledDate          | Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
	| Vodafone     | 0       | 01/01/2018 09:30:00 |            |             |              |               | 01/01/2018 09:30:00 | MARKET | BUY       | GBX      |            | 10               | 100           | 100          |
	| Vodafone     | 1       | 01/02/2018 09:55:00 |            |             |              |               | 01/02/2018 09:55:00 | MARKET | SELL      | GBX      |            | 12               | 100           | 100          |
	When I run the high profit rule
	Then I will have 3 high profit alerts

Scenario: Buy Sell orders next day and outside time window yields zero alerts
	Given I have the high profit rule parameter values
	| WindowHours | HighProfitPercentage | HighProfitAbsolute | HighProfitCurrency | HighProfitUseCurrencyConversions |
	| 23           | 0.01                 |                    |                    |                                  |
	And I have the orders for a universe from 01/01/2018 to 03/01/2018 :
	| SecurityName | OrderId | PlacedDate          | BookedDate | AmendedDate | RejectedDate | CancelledDate | FilledDate          | Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
	| Vodafone     | 0       | 01/01/2018 09:30:00 |            |             |              |               | 01/01/2018 09:30:00 | MARKET | BUY       | GBX      |            | 10               | 100           | 100          |
	| Vodafone     | 1       | 01/02/2018 09:55:00 |            |             |              |               | 01/02/2018 09:55:00 | MARKET | SELL      | GBX      |            | 12               | 100           | 100          |
	When I run the high profit rule
	Then I will have 0 high profit alerts
	
Scenario: Buy Sell orders before market open within time window yields two alerts
	Given I have the orders for a universe from 01/01/2018 to 03/01/2018 :
	| SecurityName | OrderId | PlacedDate          | BookedDate | AmendedDate | RejectedDate | CancelledDate | FilledDate          | Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
	| Vodafone     | 0       | 01/01/2018 05:30:00 |            |             |              |               | 01/01/2018 05:30:00 | MARKET | BUY       | GBX      |            | 10               | 100           | 100          |
	| Vodafone     | 1       | 01/01/2018 05:55:00 |            |             |              |               | 01/01/2018 05:55:00 | MARKET | SELL      | GBX      |            | 12               | 100           | 100          |
	When I run the high profit rule
	Then I will have 3 high profit alerts

Scenario: Buy Sell orders after market close within time window yields two alerts
	Given I have the orders for a universe from 01/01/2018 to 03/01/2018 :
	| SecurityName | OrderId | PlacedDate          | BookedDate | AmendedDate | RejectedDate | CancelledDate | FilledDate          | Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
	| Vodafone     | 0       | 01/01/2018 19:30:00 |            |             |              |               | 01/01/2018 19:30:00 | MARKET | BUY       | GBX      |            | 10               | 100           | 100          |
	| Vodafone     | 1       | 01/01/2018 19:55:00 |            |             |              |               | 01/01/2018 19:55:00 | MARKET | SELL      | GBX      |            | 12               | 100           | 100          |
	When I run the high profit rule
	Then I will have 3 high profit alerts

Scenario: Buy Sell orders with losses within time window yields zero alerts
	Given I have the orders for a universe from 01/01/2018 to 03/01/2018 :
	| SecurityName | OrderId | PlacedDate          | BookedDate | AmendedDate | RejectedDate | CancelledDate | FilledDate          | Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
	| Vodafone     | 0       | 01/01/2018 09:30:00 |            |             |              |               | 01/01/2018 09:30:00 | MARKET | BUY       | GBX      |            | 10               | 100           | 100          |
	| Vodafone     | 1       | 01/01/2018 09:55:00 |            |             |              |               | 01/01/2018 09:55:00 | MARKET | SELL      | GBX      |            | 9.5              | 100         | 100          |
	When I run the high profit rule
	Then I will have 0 high profit alerts

Scenario: Buy Sell partially filled orders within time window yields two alerts
	Given I have the orders for a universe from 01/01/2018 to 03/01/2018 :
	| SecurityName | OrderId | PlacedDate          | BookedDate | AmendedDate | RejectedDate | CancelledDate | FilledDate          | Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
	| Vodafone     | 0       | 01/01/2018 09:30:00 |            |             |              |               | 01/01/2018 09:30:00 | MARKET | BUY       | GBX      |            | 10               | 1000          | 100          |
	| Vodafone     | 1       | 01/01/2018 09:55:00 |            |             |              |               | 01/01/2018 09:55:00 | MARKET | SELL      | GBX      |            | 12               | 1000          | 100          |
	When I run the high profit rule
	Then I will have 3 high profit alerts

Scenario: Buy Sell unfilled orders within time window yields zero alerts
	Given I have the orders for a universe from 01/01/2018 to 03/01/2018 :
	| SecurityName | OrderId | PlacedDate          | BookedDate | AmendedDate | RejectedDate | CancelledDate | FilledDate          | Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
	| Vodafone     | 0       | 01/01/2018 09:30:00 |            |             |              |               |				       | MARKET | BUY       | GBX      |            | 10               | 100           |	          |
	| Vodafone     | 1       | 01/01/2018 09:55:00 |            |             |              |               |				       | MARKET | SELL      | GBX      |            | 12               | 100           |	          |
	When I run the high profit rule
	Then I will have 0 high profit alerts

Scenario: Buy Sell many orders within time window yields four alerts
	Given I have the orders for a universe from 01/01/2018 to 03/01/2018 :
	| SecurityName | OrderId | PlacedDate          | BookedDate | AmendedDate | RejectedDate | CancelledDate | FilledDate          | Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
	| Vodafone     | 0       | 01/01/2018 09:30:00 |            |             |              |               | 01/01/2018 09:30:00 | MARKET | BUY       | GBX      |            | 10               | 100           | 100          |
	| Vodafone     | 1       | 01/01/2018 09:55:00 |            |             |              |               | 01/01/2018 09:55:00 | MARKET | SELL      | GBX      |            | 12               | 100           | 100          |
	| Nvidia       | 2       | 01/01/2018 09:30:00 |            |             |              |               | 01/01/2018 09:30:00 | MARKET | BUY       | GBX      |            | 10               | 100           | 100          |
	| Nvidia       | 3       | 01/01/2018 09:55:00 |            |             |              |               | 01/01/2018 09:55:00 | MARKET | SELL      | GBX      |            | 12               | 100           | 100          |
	| Barclays     | 4       | 01/01/2018 09:30:00 |            |             |              |               | 01/01/2018 09:30:00 | MARKET | BUY       | GBX      |            | 10               | 100           | 100          |
	| Barclays     | 5       | 01/01/2018 09:55:00 |            |             |              |               | 01/01/2018 09:55:00 | MARKET | SELL      | GBX      |            | 12               | 100           | 100          |
	When I run the high profit rule
	Then I will have 6 high profit alerts

Scenario: Buy Sell orders different exchange within time window yields two alerts
	Given I have the orders for a universe from 01/01/2018 to 03/01/2018 :
	| SecurityName | OrderId | PlacedDate          | BookedDate | AmendedDate | RejectedDate | CancelledDate | FilledDate          | Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
	| Nvidia     | 0       | 01/01/2018 09:30:00 |            |             |              |               | 01/01/2018 09:30:00 | MARKET | BUY       | GBX      |            | 10               | 100           | 100          |
	| Nvidia     | 1       | 01/01/2018 09:55:00 |            |             |              |               | 01/01/2018 09:55:00 | MARKET | SELL      | GBX      |            | 12               | 100           | 100          |
	When I run the high profit rule
	Then I will have 3 high profit alerts

Scenario: Buy just buy orders within time window yields two alerts
	Given I have the orders for a universe from 01/01/2018 to 03/01/2018 :
	| SecurityName | OrderId | PlacedDate          | BookedDate | AmendedDate | RejectedDate | CancelledDate | FilledDate          | Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
	| Vodafone     | 0       | 01/01/2018 09:30:00 |            |             |              |               | 01/01/2018 09:30:00 | MARKET | BUY       | GBX      |            | 10               | 100           | 100          |
	| Vodafone     | 1       | 01/01/2018 09:55:00 |            |             |              |               | 01/01/2018 09:55:00 | MARKET | BUY      | GBX      |            | 12               | 100           | 100          |
	And With the intraday market data :
	| SecurityName | Epoch	             | Bid | Ask | Price | Currency | Volume      |
	| Vodafone     | 01/01/2018 09:30:00 | 101 | 101 | 101   | GBX      | 10000		  |
	When I run the high profit rule
	Then I will have 4 high profit alerts

Scenario: Cover just cover orders within time window yields two alerts
	Given I have the orders for a universe from 01/01/2018 to 03/01/2018 :
	| SecurityName | OrderId | PlacedDate          | BookedDate | AmendedDate | RejectedDate | CancelledDate | FilledDate          | Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
	| Vodafone     | 0       | 01/01/2018 09:30:00 |            |             |              |               | 01/01/2018 09:30:00 | MARKET | COVER       | GBX      |            | 10               | 100           | 100          |
	| Vodafone     | 1       | 01/01/2018 09:55:00 |            |             |              |               | 01/01/2018 09:55:00 | MARKET | COVER      | GBX      |            | 12               | 100           | 100          |
	And With the intraday market data :
	| SecurityName | Epoch	             | Bid | Ask | Price | Currency | Volume      |
	| Vodafone     | 01/01/2018 09:30:00 | 101 | 101 | 101   | GBX      | 10000		  |
	When I run the high profit rule
	Then I will have 4 high profit alerts

Scenario: Sell just sell orders within time window yields zero alerts
	Given I have the orders for a universe from 01/01/2018 to 03/01/2018 :
	| SecurityName | OrderId | PlacedDate          | BookedDate | AmendedDate | RejectedDate | CancelledDate | FilledDate          | Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
	| Vodafone     | 0       | 01/01/2018 09:30:00 |            |             |              |               | 01/01/2018 09:30:00 | MARKET | SELL       | GBX      |            | 10               | 100           | 100          |
	| Vodafone     | 1       | 01/01/2018 09:55:00 |            |             |              |               | 01/01/2018 09:55:00 | MARKET | SELL      | GBX      |            | 12               | 100           | 100          |
	And With the intraday market data :
	| SecurityName | Epoch	             | Bid | Ask | Price | Currency | Volume      |
	| Vodafone     | 01/01/2018 09:30:00 | 101 | 101 | 101   | GBX      | 10000		  |
	When I run the high profit rule
	Then I will have 0 high profit alerts

Scenario: Short just short orders within time window yields zero alerts
	Given I have the orders for a universe from 01/01/2018 to 03/01/2018 :
	| SecurityName | OrderId | PlacedDate          | BookedDate | AmendedDate | RejectedDate | CancelledDate | FilledDate          | Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
	| Vodafone     | 0       | 01/01/2018 09:30:00 |            |             |              |               | 01/01/2018 09:30:00 | MARKET | SHORT       | GBX      |            | 10               | 100           | 100          |
	| Vodafone     | 1       | 01/01/2018 09:55:00 |            |             |              |               | 01/01/2018 09:55:00 | MARKET | SHORT      | GBX      |            | 12               | 100           | 100          |
	And With the intraday market data :
	| SecurityName | Epoch	             | Bid | Ask | Price | Currency | Volume      |
	| Vodafone     | 01/01/2018 09:30:00 | 101 | 101 | 101   | GBX      | 10000		  |
	When I run the high profit rule
	Then I will have 0 high profit alerts

Scenario: Buy Sell orders exactly on time window yields two alerts
	Given I have the orders for a universe from 01/01/2018 to 03/01/2018 :
	| SecurityName | OrderId | PlacedDate          | BookedDate | AmendedDate | RejectedDate | CancelledDate | FilledDate          | Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
	| Vodafone     | 0       | 01/01/2018 09:30:00 |            |             |              |               | 01/01/2018 09:30:00 | MARKET | BUY       | GBX      |            | 10               | 100           | 100          |
	| Vodafone     | 1       | 01/01/2018 10:30:00 |            |             |              |               | 01/01/2018 10:30:00 | MARKET | SELL      | GBX      |            | 12               | 100           | 100          |
	When I run the high profit rule
	Then I will have 3 high profit alerts

Scenario: Buy Sell orders outside of time window yields zero alerts
	Given I have the orders for a universe from 01/01/2018 to 03/01/2018 :
	| SecurityName | OrderId | PlacedDate          | BookedDate | AmendedDate | RejectedDate | CancelledDate | FilledDate          | Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
	| Vodafone     | 0       | 01/01/2018 09:30:00 |            |             |              |               | 01/01/2018 09:30:00 | MARKET | BUY       | GBX      |            | 10               | 100           | 100          |
	| Vodafone     | 1       | 01/01/2018 11:30:00 |            |             |              |               | 01/01/2018 11:30:00 | MARKET | SELL      | GBX      |            | 12               | 100           | 100          |
	When I run the high profit rule
	Then I will have 0 high profit alerts

Scenario: Buy Sell orders yields two alerts
	Given I have the orders for a universe from 01/01/2018 to 03/01/2018 :
	| SecurityName | OrderId | PlacedDate          | BookedDate | AmendedDate | RejectedDate | CancelledDate | FilledDate          | Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
	| Vodafone     | 0       | 01/01/2018 09:30:00 |            |             |              |               | 01/01/2018 09:30:00 | MARKET | BUY       | GBX      |            | 10               | 100           | 100          |
	| Vodafone     | 1       | 01/01/2018 09:30:00 |            |             |              |               | 01/01/2018 09:30:00 | MARKET | SELL      | GBX      |            | 12               | 100           | 100          |
	When I run the high profit rule
	Then I will have 3 high profit alerts

Scenario: Buy Sell orders at exact percentage yields two alerts
	Given I have the orders for a universe from 01/01/2018 to 03/01/2018 :
	| SecurityName | OrderId | PlacedDate          | BookedDate | AmendedDate | RejectedDate | CancelledDate | FilledDate          | Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
	| Vodafone     | 0       | 01/01/2018 09:30:00 |            |             |              |               | 01/01/2018 09:30:00 | MARKET | BUY       | GBX      |            | 100              | 100           | 100          |
	| Vodafone     | 1       | 01/01/2018 09:30:00 |            |             |              |               | 01/01/2018 09:30:00 | MARKET | SELL      | GBX      |            | 101               | 100           | 100          |
	When I run the high profit rule
	Then I will have 3 high profit alerts

Scenario: Buy Sell orders at just below percentage yields zero alerts
	Given I have the orders for a universe from 01/01/2018 to 03/01/2018 :
	| SecurityName | OrderId | PlacedDate          | BookedDate | AmendedDate | RejectedDate | CancelledDate | FilledDate          | Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
	| Vodafone     | 0       | 01/01/2018 09:30:00 |            |             |              |               | 01/01/2018 09:30:00 | MARKET | BUY       | GBX      |            | 1000             | 100           | 100          |
	| Vodafone     | 1       | 01/01/2018 09:30:00 |            |             |              |               | 01/01/2018 09:30:00 | MARKET | SELL      | GBX      |            | 1001             | 100           | 100          |
	When I run the high profit rule
	Then I will have 0 high profit alerts

Scenario: Buy order with increase in market price (bmll) yields one alert
	Given I have the orders for a universe from 01/01/2018 to 03/01/2018 :
	| SecurityName | OrderId | PlacedDate          | BookedDate | AmendedDate | RejectedDate | CancelledDate | FilledDate          | Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
	| Vodafone     | 0       | 01/01/2018 09:30:00 |            |             |              |               | 01/01/2018 09:30:00 | MARKET | BUY       | GBX      |            | 100              | 100           | 100          |
	And With the intraday market data :
	| SecurityName | Epoch	             | Bid | Ask | Price | Currency | Volume      |
	| Vodafone     | 01/01/2018 09:30:00 | 101 | 101 | 101   | GBX      | 10000		  |
	When I run the high profit rule
	Then I will have 2 high profit alerts

Scenario: Buy order with increase in market price to exact percentage (bmll) yields one alert
	Given I have the orders for a universe from 01/01/2018 to 03/01/2018 :
	| SecurityName | OrderId | PlacedDate          | BookedDate | AmendedDate | RejectedDate | CancelledDate | FilledDate          | Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
	| Vodafone     | 0       | 01/01/2018 09:30:00 |            |             |              |               | 01/01/2018 09:30:00 | MARKET | BUY       | GBX      |            | 100              | 100           | 100          |
	And With the intraday market data :
	| SecurityName | Epoch	             | Bid | Ask | Price | Currency | Volume      |
	| Vodafone     | 01/01/2018 09:30:00 | 101 | 101 | 101   | GBX      | 10000		  |
	When I run the high profit rule
	Then I will have 2 high profit alerts

Scenario: Buy order with substantial increase in market price (bmll) yields one alert
	Given I have the orders for a universe from 01/01/2018 to 03/01/2018 :
	| SecurityName | OrderId | PlacedDate          | BookedDate | AmendedDate | RejectedDate | CancelledDate | FilledDate          | Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
	| Vodafone     | 0       | 01/01/2018 09:30:00 |            |             |              |               | 01/01/2018 09:30:00 | MARKET | BUY       | GBX      |            | 100              | 100           | 100          |
	And With the intraday market data :
	| SecurityName | Epoch	             | Bid | Ask | Price | Currency | Volume      |
	| Vodafone     | 01/01/2018 09:30:00 | 101 | 101 | 110   | GBX      | 10000		  |
	When I run the high profit rule
	Then I will have 2 high profit alerts

Scenario: Buy order with decrease in market price (bmll) yields zero alerts
	Given I have the orders for a universe from 01/01/2018 to 03/01/2018 :
	| SecurityName | OrderId | PlacedDate          | BookedDate | AmendedDate | RejectedDate | CancelledDate | FilledDate          | Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
	| Vodafone     | 0       | 01/01/2018 09:30:00 |            |             |              |               | 01/01/2018 09:30:00 | MARKET | BUY       | GBX      |            | 100              | 100           | 100          |
	And With the intraday market data :
	| SecurityName | Epoch	             | Bid | Ask | Price | Currency | Volume      |
	| Vodafone     | 01/01/2018 09:30:00 | 101 | 101 | 98    | GBX      | 10000		  |
	When I run the high profit rule
	Then I will have 0 high profit alerts