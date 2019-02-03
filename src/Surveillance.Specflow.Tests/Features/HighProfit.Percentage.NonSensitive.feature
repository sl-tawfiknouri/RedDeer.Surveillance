Feature: HighProfit Percentage Non Sensitive Parameters
	In order to meet MAR compliance requirements
	I need to be able to detect when traders are executing trades
	Which generate unusual levels of profits
	By measuring their security trade profits as a percentage return

#High profit rule alerts have twice as many alerts to account for market closure analysis
#This is deduplicated elsewhere in the rules process

Background:
	Given I have the high profit rule parameter values
	| WindowHours | HighProfitPercentage | HighProfitAbsolute | HighProfitCurrency | HighProfitUseCurrencyConversions |
	| 1           | 0.1                 |                    |                    |                                  |

@highprofit
@highprofitpercentage
@highprofitpercentagenonsensitive
Scenario: Empty Universe yields no alerts
	Given I have the orders for a universe from 01/01/2018 to 03/01/2018 :
	| SecurityName | OrderId | PlacedDate          | BookedDate | AmendedDate | RejectedDate | CancelledDate | FilledDate          | Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
	When I run the high profit rule
	Then I will have 0 high profit alerts

@highprofit
@highprofitpercentage
@highprofitpercentagenonsensitive
Scenario: Single order yields no alerts
	Given I have the orders for a universe from 01/01/2018 to 03/01/2018 :
	| SecurityName | OrderId | PlacedDate          | BookedDate | AmendedDate | RejectedDate | CancelledDate | FilledDate          | Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
	| Vodafone     | 0       | 01/01/2018 09:30:00 |            |             |              |               | 01/01/2018 09:30:00 | MARKET | BUY       | GBX      |            | 10               | 100           | 100          |
	When I run the high profit rule
	Then I will have 0 high profit alerts


@highprofit
@highprofitpercentage
@highprofitpercentagenonsensitive
Scenario: Buy Sell orders within the time window yields two alerts
	Given I have the orders for a universe from 01/01/2018 to 03/01/2018 :
	| SecurityName | OrderId | PlacedDate          | BookedDate | AmendedDate | RejectedDate | CancelledDate | FilledDate          | Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
	| Vodafone     | 0       | 01/01/2018 09:30:00 |            |             |              |               | 01/01/2018 09:30:00 | MARKET | BUY       | GBX      |            | 10               | 100           | 100          |
	| Vodafone     | 1       | 01/01/2018 09:31:00 |            |             |              |               | 01/01/2018 09:31:00 | MARKET | SELL      | GBX      |            | 12               | 100           | 100          |
	When I run the high profit rule
	Then I will have 2 high profit alerts

@highprofit
@highprofitpercentage
@highprofitpercentagenonsensitive
Scenario: Buy Sell many orders within the time window yields six alerts
	Given I have the orders for a universe from 01/01/2018 to 03/01/2018 :
	| SecurityName | OrderId | PlacedDate          | BookedDate | AmendedDate | RejectedDate | CancelledDate | FilledDate          | Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
	| Vodafone     | 0       | 01/01/2018 09:30:00 |            |             |              |               | 01/01/2018 09:30:00 | MARKET | BUY       | GBX      |            | 10               | 100           | 100          |
	| Vodafone     | 1       | 01/01/2018 09:31:00 |            |             |              |               | 01/01/2018 09:31:00 | MARKET | SELL      | GBX      |            | 12               | 100           | 100          |
	| Nvidia       | 2       | 01/01/2018 09:30:00 |            |             |              |               | 01/01/2018 09:30:00 | MARKET | BUY       | GBX      |            | 10               | 100           | 100          |
	| Nvidia       | 3       | 01/01/2018 09:31:00 |            |             |              |               | 01/01/2018 09:31:00 | MARKET | SELL      | GBX      |            | 12               | 100           | 100          |
	| Barclays     | 4       | 01/01/2018 09:30:00 |            |             |              |               | 01/01/2018 09:30:00 | MARKET | BUY       | GBX      |            | 10               | 100           | 100          |
	| Barclays     | 5       | 01/01/2018 09:31:00 |            |             |              |               | 01/01/2018 09:31:00 | MARKET | SELL      | GBX      |            | 12               | 100           | 100          |
	When I run the high profit rule
	Then I will have 4 high profit alerts

@highprofit
@highprofitpercentage
@highprofitpercentagenonsensitive
Scenario: Buy Sell orders different exchange within the time window yields two alerts
	Given I have the orders for a universe from 01/01/2018 to 03/01/2018 :
	| SecurityName | OrderId | PlacedDate          | BookedDate | AmendedDate | RejectedDate | CancelledDate | FilledDate          | Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
	| Nvidia     | 0       | 01/01/2018 09:30:00 |            |             |              |               | 01/01/2018 09:30:00 | MARKET | BUY       | GBX      |            | 10               | 100           | 100          |
	| Nvidia     | 1       | 01/01/2018 09:31:00 |            |             |              |               | 01/01/2018 09:31:00 | MARKET | SELL      | GBX      |            | 12               | 100           | 100          |
	When I run the high profit rule
	Then I will have 2 high profit alerts

@highprofit
@highprofitpercentage
@highprofitpercentagenonsensitive
Scenario: Buy just buy orders within the time window yields two alerts
	Given I have the orders for a universe from 01/01/2018 to 03/01/2018 :
	| SecurityName | OrderId | PlacedDate          | BookedDate | AmendedDate | RejectedDate | CancelledDate | FilledDate          | Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
	| Vodafone     | 0       | 01/01/2018 09:30:00 |            |             |              |               | 01/01/2018 09:30:00 | MARKET | BUY       | GBX      |            | 10               | 100           | 100          |
	| Vodafone     | 1       | 01/01/2018 09:31:00 |            |             |              |               | 01/01/2018 09:31:00 | MARKET | BUY      | GBX      |            | 12               | 100           | 100          |
	And With the intraday market data :
	| SecurityName | Epoch	             | Bid | Ask | Price | Currency | Volume      |
	| Vodafone     | 01/01/2018 09:30:00 | 101 | 101 | 110   | GBX      | 10000		  |
	When I run the high profit rule
	Then I will have 2 high profit alerts

@highprofit
@highprofitpercentage
@highprofitpercentagenonsensitive
Scenario: Cover just cover orders within the time window yields two alerts
	Given I have the orders for a universe from 01/01/2018 to 03/01/2018 :
	| SecurityName | OrderId | PlacedDate          | BookedDate | AmendedDate | RejectedDate | CancelledDate | FilledDate          | Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
	| Vodafone     | 0       | 01/01/2018 09:30:00 |            |             |              |               | 01/01/2018 09:30:00 | MARKET | COVER       | GBX      |            | 10               | 100           | 100          |
	| Vodafone     | 1       | 01/01/2018 09:31:00 |            |             |              |               | 01/01/2018 09:31:00 | MARKET | COVER       | GBX      |            | 12               | 100           | 100          |
	And With the intraday market data :
	| SecurityName | Epoch	             | Bid | Ask | Price | Currency | Volume      |
	| Vodafone     | 01/01/2018 09:30:00 | 101 | 101 | 110   | GBX      | 10000		  |
	When I run the high profit rule
	Then I will have 2 high profit alerts

@highprofit
@highprofitpercentage
@highprofitpercentagenonsensitive
Scenario: Sell just sell orders within the time window yields zero alerts
	Given I have the orders for a universe from 01/01/2018 to 03/01/2018 :
	| SecurityName | OrderId | PlacedDate          | BookedDate | AmendedDate | RejectedDate | CancelledDate | FilledDate          | Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
	| Vodafone     | 0       | 01/01/2018 09:30:00 |            |             |              |               | 01/01/2018 09:30:00 | MARKET | SELL       | GBX      |            | 10               | 100           | 100          |
	| Vodafone     | 1       | 01/01/2018 09:31:00 |            |             |              |               | 01/01/2018 09:31:00 | MARKET | SELL      | GBX      |            | 12               | 100           | 100          |
	And With the intraday market data :
	| SecurityName | Epoch	             | Bid | Ask | Price | Currency | Volume      |
	| Vodafone     | 01/01/2018 09:30:00 | 101 | 101 | 110   | GBX      | 10000		  |
	When I run the high profit rule
	Then I will have 0 high profit alerts

@highprofit
@highprofitpercentage
@highprofitpercentagenonsensitive
Scenario: Short just short orders within the time window yields zero alerts
	Given I have the orders for a universe from 01/01/2018 to 03/01/2018 :
	| SecurityName | OrderId | PlacedDate          | BookedDate | AmendedDate | RejectedDate | CancelledDate | FilledDate          | Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
	| Vodafone     | 0       | 01/01/2018 09:30:00 |            |             |              |               | 01/01/2018 09:30:00 | MARKET | SHORT       | GBX      |            | 10               | 100           | 100          |
	| Vodafone     | 1       | 01/01/2018 09:31:00 |            |             |              |               | 01/01/2018 09:31:00 | MARKET | SHORT      | GBX      |            | 12               | 100           | 100          |
	And With the intraday market data :
	| SecurityName | Epoch	             | Bid | Ask | Price | Currency | Volume      |
	| Vodafone     | 01/01/2018 09:30:00 | 101 | 101 | 110   | GBX      | 10000		  |
	When I run the high profit rule
	Then I will have 0 high profit alerts


@highprofit
@highprofitpercentage
@highprofitpercentagenonsensitive
Scenario: Buy Sell orders exactly on the time window yields two alerts
	Given I have the orders for a universe from 01/01/2018 to 03/01/2018 :
	| SecurityName | OrderId | PlacedDate          | BookedDate | AmendedDate | RejectedDate | CancelledDate | FilledDate          | Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
	| Vodafone     | 0       | 01/01/2018 09:30:00 |            |             |              |               | 01/01/2018 09:30:00 | MARKET | BUY       | GBX      |            | 10               | 100           | 100          |
	| Vodafone     | 1       | 01/01/2018 10:30:00 |            |             |              |               | 01/01/2018 10:30:00 | MARKET | SELL      | GBX      |            | 12               | 100           | 100          |
	When I run the high profit rule
	Then I will have 2 high profit alerts

@highprofit
@highprofitpercentage
@highprofitpercentagenonsensitive
Scenario: Buy Sell orders outside of the time window yields zero alerts
	Given I have the orders for a universe from 01/01/2018 to 03/01/2018 :
	| SecurityName | OrderId | PlacedDate          | BookedDate | AmendedDate | RejectedDate | CancelledDate | FilledDate          | Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
	| Vodafone     | 0       | 01/01/2018 09:30:00 |            |             |              |               | 01/01/2018 09:30:00 | MARKET | BUY       | GBX      |            | 10               | 100           | 100          |
	| Vodafone     | 1       | 01/01/2018 11:30:00 |            |             |              |               | 01/01/2018 11:30:00 | MARKET | SELL      | GBX      |            | 12               | 100           | 100          |
	When I run the high profit rule
	Then I will have 0 high profit alerts

@highprofit
@highprofitpercentage
@highprofitpercentagenonsensitive
Scenario: Buy Sell orders yields two alerts
	Given I have the orders for a universe from 01/01/2018 to 03/01/2018 :
	| SecurityName | OrderId | PlacedDate          | BookedDate | AmendedDate | RejectedDate | CancelledDate | FilledDate          | Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
	| Vodafone     | 0       | 01/01/2018 09:30:00 |            |             |              |               | 01/01/2018 09:30:00 | MARKET | BUY       | GBX      |            | 10               | 100           | 100          |
	| Vodafone     | 1       | 01/01/2018 09:30:00 |            |             |              |               | 01/01/2018 09:30:00 | MARKET | SELL      | GBX      |            | 12               | 100           | 100          |
	When I run the high profit rule
	Then I will have 2 high profit alerts

@highprofit
@highprofitpercentage
@highprofitpercentagenonsensitive
Scenario: Buy Sell orders at exact percentage yields two alerts
	Given I have the orders for a universe from 01/01/2018 to 03/01/2018 :
	| SecurityName | OrderId | PlacedDate          | BookedDate | AmendedDate | RejectedDate | CancelledDate | FilledDate          | Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
	| Vodafone     | 0       | 01/01/2018 09:30:00 |            |             |              |               | 01/01/2018 09:30:00 | MARKET | BUY       | GBX      |            | 100              | 100           | 100          |
	| Vodafone     | 1       | 01/01/2018 09:30:00 |            |             |              |               | 01/01/2018 09:30:00 | MARKET | SELL      | GBX      |            | 110               | 100           | 100          |
	When I run the high profit rule
	Then I will have 2 high profit alerts

@highprofit
@highprofitpercentage
@highprofitpercentagenonsensitive
Scenario: Buy Sell orders at just below percentage yields zero alerts
	Given I have the orders for a universe from 01/01/2018 to 03/01/2018 :
	| SecurityName | OrderId | PlacedDate          | BookedDate | AmendedDate | RejectedDate | CancelledDate | FilledDate          | Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
	| Vodafone     | 0       | 01/01/2018 09:30:00 |            |             |              |               | 01/01/2018 09:30:00 | MARKET | BUY       | GBX      |            | 1000             | 100           | 100          |
	| Vodafone     | 1       | 01/01/2018 09:30:00 |            |             |              |               | 01/01/2018 09:30:00 | MARKET | SELL      | GBX      |            | 1001             | 100           | 100          |
	When I run the high profit rule
	Then I will have 0 high profit alerts


@highprofit
@highprofitpercentage
@highprofitpercentagenonsensitive
Scenario: Buy order with increase in market price (bmll) yields one alert
	Given I have the orders for a universe from 01/01/2018 to 03/01/2018 :
	| SecurityName | OrderId | PlacedDate          | BookedDate | AmendedDate | RejectedDate | CancelledDate | FilledDate          | Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
	| Vodafone     | 0       | 01/01/2018 09:30:00 |            |             |              |               | 01/01/2018 09:30:00 | MARKET | BUY       | GBX      |            | 100              | 100           | 100          |
	And With the intraday market data :
	| SecurityName | Epoch	             | Bid | Ask | Price | Currency | Volume      |
	| Vodafone     | 01/01/2018 09:30:00 | 101 | 101 | 110   | GBX      | 10000		  |
	When I run the high profit rule
	Then I will have 1 high profit alerts


@highprofit
@highprofitpercentage
@highprofitpercentagenonsensitive
Scenario: Buy order with increase in market price to exact percentage (bmll) yields one alert
	Given I have the orders for a universe from 01/01/2018 to 03/01/2018 :
	| SecurityName | OrderId | PlacedDate          | BookedDate | AmendedDate | RejectedDate | CancelledDate | FilledDate          | Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
	| Vodafone     | 0       | 01/01/2018 09:30:00 |            |             |              |               | 01/01/2018 09:30:00 | MARKET | BUY       | GBX      |            | 100              | 100           | 100          |
	And With the intraday market data :
	| SecurityName | Epoch	             | Bid | Ask | Price | Currency | Volume      |
	| Vodafone     | 01/01/2018 09:30:00 | 101 | 101 | 110   | GBX      | 10000		  |
	When I run the high profit rule
	Then I will have 1 high profit alerts

@highprofit
@highprofitpercentage
@highprofitpercentagenonsensitive
Scenario: Buy order with substantial increase in market price (bmll) yields one alert
	Given I have the orders for a universe from 01/01/2018 to 03/01/2018 :
	| SecurityName | OrderId | PlacedDate          | BookedDate | AmendedDate | RejectedDate | CancelledDate | FilledDate          | Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
	| Vodafone     | 0       | 01/01/2018 09:30:00 |            |             |              |               | 01/01/2018 09:30:00 | MARKET | BUY       | GBX      |            | 100              | 100           | 100          |
	And With the intraday market data :
	| SecurityName | Epoch	             | Bid | Ask | Price | Currency | Volume      |
	| Vodafone     | 01/01/2018 09:30:00 | 101 | 101 | 110   | GBX      | 10000		  |
	When I run the high profit rule
	Then I will have 1 high profit alerts

@highprofit
@highprofitpercentage
@highprofitpercentagenonsensitive
Scenario: Buy order with decrease in market price (bmll) yields zero alerts
	Given I have the orders for a universe from 01/01/2018 to 03/01/2018 :
	| SecurityName | OrderId | PlacedDate          | BookedDate | AmendedDate | RejectedDate | CancelledDate | FilledDate          | Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
	| Vodafone     | 0       | 01/01/2018 09:30:00 |            |             |              |               | 01/01/2018 09:30:00 | MARKET | BUY       | GBX      |            | 100              | 100           | 100          |
	And With the intraday market data :
	| SecurityName | Epoch	             | Bid | Ask | Price | Currency | Volume      |
	| Vodafone     | 01/01/2018 09:30:00 | 101 | 101 | 98    | GBX      | 10000		  |
	When I run the high profit rule
	Then I will have 0 high profit alerts