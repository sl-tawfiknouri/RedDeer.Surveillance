@highprofit
@highprofitabsolutecurrency
@highprofitabsolutenonsensitive
Feature: HighProfit Absolute Non Sensitive Currency Parameters
	In order to meet MAR compliance requirements
	I need to be able to detect when traders are executing trades
	Which generate unusual levels of profits
	By measuring their security trade profits as an absolute currency adjusted return

#High profit rule alerts have twice as many alerts to account for market closure analysis
#This is deduplicated elsewhere in the rules process

Background:
	Given I have the high profit rule parameter values
	| WindowHours | HighProfitPercentage | HighProfitAbsolute | HighProfitCurrency | HighProfitUseCurrencyConversions |
	| 1           |                      | 100000             | GBX                | true                             |

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

Scenario: Buy Sell orders within the window yields one alerts
	Given I have the orders for a universe from 01/01/2018 to 03/01/2018 :
	| SecurityName | OrderId | PlacedDate          | BookedDate | AmendedDate | RejectedDate | CancelledDate | FilledDate          | Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
	| Vodafone     | 0       | 01/01/2018 09:30:00 |            |             |              |               | 01/01/2018 09:30:00 | MARKET | BUY       | USD      |            | 100              | 100000	       | 100000       |
	| Vodafone     | 1       | 01/01/2018 09:55:00 |            |             |              |               | 01/01/2018 09:55:00 | MARKET | SELL      | USD      |            | 110              | 100000	       | 100000       |
	When I run the high profit rule
	Then I will have 1 high profit alerts

Scenario: Buy Sell orders with losses within the window yields zero alerts
	Given I have the orders for a universe from 01/01/2018 to 03/01/2018 :
	| SecurityName | OrderId | PlacedDate          | BookedDate | AmendedDate | RejectedDate | CancelledDate | FilledDate          | Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
	| Vodafone     | 0       | 01/01/2018 09:30:00 |            |             |              |               | 01/01/2018 09:30:00 | MARKET | BUY       | USD      |            | 100              | 100000	       | 100000       |
	| Vodafone     | 1       | 01/01/2018 09:55:00 |            |             |              |               | 01/01/2018 09:55:00 | MARKET | SELL      | USD      |            | 50               | 100000	       | 100000       |
	When I run the high profit rule
	Then I will have 0 high profit alerts

Scenario: Buy Sell partially filled orders within the window yields one alerts
	Given I have the orders for a universe from 01/01/2018 to 03/01/2018 :
	| SecurityName | OrderId | PlacedDate          | BookedDate | AmendedDate | RejectedDate | CancelledDate | FilledDate          | Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
	| Vodafone     | 0       | 01/01/2018 09:30:00 |            |             |              |               | 01/01/2018 09:30:00 | MARKET | BUY       | USD      |            | 100              | 1000000       | 100000       |
	| Vodafone     | 1       | 01/01/2018 09:55:00 |            |             |              |               | 01/01/2018 09:55:00 | MARKET | SELL      | USD      |            | 110              | 1000000       | 100000       |
	When I run the high profit rule
	Then I will have 1 high profit alerts

Scenario: Buy Sell unfilled orders within the window yields zero alerts
	Given I have the orders for a universe from 01/01/2018 to 03/01/2018 :
	| SecurityName | OrderId | PlacedDate          | BookedDate | AmendedDate | RejectedDate | CancelledDate | FilledDate          | Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
	| Vodafone     | 0       | 01/01/2018 09:30:00 |            |             |              |               |					   | MARKET | BUY       | USD      |            | 100              | 100000	       |		      |
	| Vodafone     | 1       | 01/01/2018 09:55:00 |            |             |              |               |					   | MARKET | SELL      | USD      |            | 110              | 100000	       |		      |
	When I run the high profit rule
	Then I will have 0 high profit alerts

Scenario: Buy Sell orders over lots of securities within the window yields two alerts
	Given I have the orders for a universe from 01/01/2018 to 03/01/2018 :
	| SecurityName | OrderId | PlacedDate          | BookedDate | AmendedDate | RejectedDate | CancelledDate | FilledDate          | Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
	| Vodafone     | 0       | 01/01/2018 09:30:00 |            |             |              |               | 01/01/2018 09:30:00 | MARKET | BUY       | USD      |            | 100              | 100000	       | 100000       |
	| Vodafone     | 1       | 01/01/2018 09:55:00 |            |             |              |               | 01/01/2018 09:55:00 | MARKET | SELL      | USD      |            | 110              | 100000	       | 100000       |
	| Micron       | 2       | 01/01/2018 09:30:00 |            |             |              |               | 01/01/2018 09:30:00 | MARKET | BUY       | USD      |            | 100              | 100000	       | 100000       |
	| Micron       | 3       | 01/01/2018 09:55:00 |            |             |              |               | 01/01/2018 09:55:00 | MARKET | SELL      | USD      |            | 110              | 100000	       | 100000       |
	| Barclays     | 4       | 01/01/2018 09:30:00 |            |             |              |               | 01/01/2018 09:30:00 | MARKET | BUY       | USD      |            | 100              | 100000	       | 100000       |
	| Barclays     | 5       | 01/01/2018 09:55:00 |            |             |              |               | 01/01/2018 09:55:00 | MARKET | SELL      | USD      |            | 110              | 100000	       | 100000       |
	When I run the high profit rule
	Then I will have 2 high profit alerts

Scenario: Buy Sell orders different exchange within the window yields one alerts
	Given I have the orders for a universe from 01/01/2018 to 03/01/2018 :
	| SecurityName | OrderId | PlacedDate          | BookedDate | AmendedDate | RejectedDate | CancelledDate | FilledDate          | Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
	| Nvidia     | 0       | 01/01/2018 09:30:00 |            |             |              |               | 01/01/2018 09:30:00 | MARKET | BUY       | USD      |            | 100              | 100000	       | 100000       |
	| Nvidia     | 1       | 01/01/2018 09:55:00 |            |             |              |               | 01/01/2018 09:55:00 | MARKET | SELL      | USD      |            | 110              | 100000	       | 100000       |
	When I run the high profit rule
	Then I will have 1 high profit alerts

Scenario: Buy just buy orders within the window yields two alerts
	Given I have the orders for a universe from 01/01/2018 to 03/01/2018 :
	| SecurityName | OrderId | PlacedDate          | BookedDate | AmendedDate | RejectedDate | CancelledDate | FilledDate          | Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
	| Vodafone     | 0       | 01/01/2018 09:30:00 |            |             |              |               | 01/01/2018 09:30:00 | MARKET | BUY       | USD      |            | 100              | 100000	       | 100000       |
	| Vodafone     | 1       | 01/01/2018 09:55:00 |            |             |              |               | 01/01/2018 09:55:00 | MARKET | BUY      | USD      |            | 110              | 100000	       | 100000       |
	And With the intraday market data :
	| SecurityName | Epoch	             | Bid | Ask | Price | Currency | Volume      |
	| Vodafone     | 01/01/2018 09:30:00 | 101 | 101 | 110   | USD      | 10000		  |
	When I run the high profit rule
	Then I will have 2 high profit alerts

Scenario: Cover just cover orders within the window yields two alerts
	Given I have the orders for a universe from 01/01/2018 to 03/01/2018 :
	| SecurityName | OrderId | PlacedDate          | BookedDate | AmendedDate | RejectedDate | CancelledDate | FilledDate          | Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
	| Vodafone     | 0       | 01/01/2018 09:30:00 |            |             |              |               | 01/01/2018 09:30:00 | MARKET | COVER       | USD      |            | 100              | 100000	       | 100000       |
	| Vodafone     | 1       | 01/01/2018 09:55:00 |            |             |              |               | 01/01/2018 09:55:00 | MARKET | COVER      | USD      |            | 110              | 100000	       | 100000       |
	And With the intraday market data :
	| SecurityName | Epoch	             | Bid | Ask | Price | Currency | Volume      |
	| Vodafone     | 01/01/2018 09:30:00 | 101 | 101 | 110   | USD      | 10000		  |
	When I run the high profit rule
	Then I will have 2 high profit alerts

Scenario: Sell just sell orders within the window yields zero alerts
	Given I have the orders for a universe from 01/01/2018 to 03/01/2018 :
	| SecurityName | OrderId | PlacedDate          | BookedDate | AmendedDate | RejectedDate | CancelledDate | FilledDate          | Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
	| Vodafone     | 0       | 01/01/2018 09:30:00 |            |             |              |               | 01/01/2018 09:30:00 | MARKET | SELL       | USD      |            | 100              | 100000	       | 100000       |
	| Vodafone     | 1       | 01/01/2018 09:55:00 |            |             |              |               | 01/01/2018 09:55:00 | MARKET | SELL      | USD      |            | 110              | 100000	       | 100000       |
		And With the intraday market data :
	| SecurityName | Epoch	             | Bid | Ask | Price | Currency | Volume      |
	| Vodafone     | 01/01/2018 09:30:00 | 101 | 101 | 110   | USD      | 10000		  |
	When I run the high profit rule
	Then I will have 0 high profit alerts

Scenario: Short just short orders within the window yields zero alerts
	Given I have the orders for a universe from 01/01/2018 to 03/01/2018 :
	| SecurityName | OrderId | PlacedDate          | BookedDate | AmendedDate | RejectedDate | CancelledDate | FilledDate          | Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
	| Vodafone     | 0       | 01/01/2018 09:30:00 |            |             |              |               | 01/01/2018 09:30:00 | MARKET | SHORT       | USD      |            | 100              | 100000	       | 100000       |
	| Vodafone     | 1       | 01/01/2018 09:55:00 |            |             |              |               | 01/01/2018 09:55:00 | MARKET | SHORT      | USD      |            | 110              | 100000	       | 100000       |
	And With the intraday market data :
	| SecurityName | Epoch	             | Bid | Ask | Price | Currency | Volume      |
	| Vodafone     | 01/01/2018 09:30:00 | 101 | 101 | 110   | USD      | 10000		  |
	When I run the high profit rule
	Then I will have 0 high profit alerts

Scenario: Buy Sell orders at the window yields one alerts
	Given I have the orders for a universe from 01/01/2018 to 03/01/2018 :
	| SecurityName | OrderId | PlacedDate          | BookedDate | AmendedDate | RejectedDate | CancelledDate | FilledDate          | Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
	| Vodafone     | 0       | 01/01/2018 09:30:00 |            |             |              |               | 01/01/2018 09:30:00 | MARKET | BUY       | USD      |            | 100              | 100000	       | 100000       |
	| Vodafone     | 1       | 01/01/2018 10:30:00 |            |             |              |               | 01/01/2018 10:30:00 | MARKET | SELL      | USD      |            | 110              | 100000	       | 100000       |
	When I run the high profit rule
	Then I will have 1 high profit alerts

Scenario: Buy Sell orders outside of the window yields zero alerts
	Given I have the orders for a universe from 01/01/2018 to 03/01/2018 :
	| SecurityName | OrderId | PlacedDate          | BookedDate | AmendedDate | RejectedDate | CancelledDate | FilledDate          | Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
	| Vodafone     | 0       | 01/01/2018 09:30:00 |            |             |              |               | 01/01/2018 09:30:00 | MARKET | BUY       | USD      |            | 100              | 100000	       | 100000       |
	| Vodafone     | 1       | 01/01/2018 10:35:00 |            |             |              |               | 01/01/2018 10:35:00 | MARKET | SELL      | USD      |            | 110              | 100000	       | 100000       |
	When I run the high profit rule
	Then I will have 0 high profit alerts

Scenario: Buy Sell orders yields two alerts
	Given I have the orders for a universe from 01/01/2018 to 03/01/2018 :
	| SecurityName | OrderId | PlacedDate          | BookedDate | AmendedDate | RejectedDate | CancelledDate | FilledDate          | Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
	| Vodafone     | 0       | 01/01/2018 09:30:00 |            |             |              |               | 01/01/2018 09:30:00 | MARKET | BUY       | USD      |            | 100              | 100000	       | 100000       |
	| Vodafone     | 1       | 01/01/2018 09:30:00 |            |             |              |               | 01/01/2018 09:30:00 | MARKET | SELL      | USD      |            | 110              | 100000	       | 100000       |
	When I run the high profit rule
	Then I will have 1 high profit alerts

Scenario: Buy Sell orders at exact threshold yields two alerts
	Given I have the orders for a universe from 01/01/2018 to 03/01/2018 :
	| SecurityName | OrderId | PlacedDate          | BookedDate | AmendedDate | RejectedDate | CancelledDate | FilledDate          | Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
	| Vodafone     | 0       | 01/01/2018 09:30:00 |            |             |              |               | 01/01/2018 09:30:00 | MARKET | BUY       | USD      |            | 100              | 100000     | 100000	    |
	| Vodafone     | 1       | 01/01/2018 09:30:00 |            |             |              |               | 01/01/2018 09:30:00 | MARKET | SELL      | USD      |            | 101              | 100000     | 100000	    |
	When I run the high profit rule
	Then I will have 1 high profit alerts

Scenario: Buy Sell orders at just below threshold yields zero alerts
	Given I have the orders for a universe from 01/01/2018 to 03/01/2018 :
	| SecurityName | OrderId | PlacedDate          | BookedDate | AmendedDate | RejectedDate | CancelledDate | FilledDate          | Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
	| Vodafone     | 0       | 01/01/2018 09:30:00 |            |             |              |               | 01/01/2018 09:30:00 | MARKET | BUY       | USD      |            | 1000             | 99999		   | 99999       |
	| Vodafone     | 1       | 01/01/2018 09:30:00 |            |             |              |               | 01/01/2018 09:30:00 | MARKET | SELL      | USD      |            | 1001             | 99999		   | 99999       |
	When I run the high profit rule
	Then I will have 1 high profit alerts

Scenario: Buy order with increase in market price (bmll) yields one alert
	Given I have the orders for a universe from 01/01/2018 to 03/01/2018 :
	| SecurityName | OrderId | PlacedDate          | BookedDate | AmendedDate | RejectedDate | CancelledDate | FilledDate          | Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
	| Vodafone     | 0       | 01/01/2018 09:30:00 |            |             |              |               | 01/01/2018 09:30:00 | MARKET | BUY       | USD      |            | 100              | 100000000     | 100000000    |
	And With the intraday market data :
	| SecurityName | Epoch	             | Bid | Ask | Price | Currency | Volume      |
	| Vodafone     | 01/01/2018 09:30:00 | 101 | 101 | 110   | USD      | 10000		  |
	When I run the high profit rule
	Then I will have 1 high profit alerts

Scenario: Buy order with increase in market price to exact threshold (bmll) yields one alert
	Given I have the orders for a universe from 01/01/2018 to 03/01/2018 :
	| SecurityName | OrderId | PlacedDate          | BookedDate | AmendedDate | RejectedDate | CancelledDate | FilledDate          | Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
	| Vodafone     | 0       | 01/01/2018 09:30:00 |            |             |              |               | 01/01/2018 09:30:00 | MARKET | BUY       | USD      |            | 100              | 100000        | 100000       |
	And With the intraday market data :
	| SecurityName | Epoch	             | Bid | Ask | Price | Currency | Volume      |
	| Vodafone     | 01/01/2018 09:30:00 | 101 | 101 | 101   | USD      | 10000		  |
	When I run the high profit rule
	Then I will have 1 high profit alerts

Scenario: Buy order with substantial increase in market price (bmll) yields one alert
	Given I have the orders for a universe from 01/01/2018 to 03/01/2018 :
	| SecurityName | OrderId | PlacedDate          | BookedDate | AmendedDate | RejectedDate | CancelledDate | FilledDate          | Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
	| Vodafone     | 0       | 01/01/2018 09:30:00 |            |             |              |               | 01/01/2018 09:30:00 | MARKET | BUY       | USD      |            | 100              | 100000        | 100000       |
	And With the intraday market data :
	| SecurityName | Epoch	             | Bid | Ask | Price | Currency | Volume      |
	| Vodafone     | 01/01/2018 09:30:00 | 101 | 101 | 110   | USD      | 10000		  |
	When I run the high profit rule
	Then I will have 1 high profit alerts

Scenario: Buy order with decrease in market price (bmll) yields zero alerts
	Given I have the orders for a universe from 01/01/2018 to 03/01/2018 :
	| SecurityName | OrderId | PlacedDate          | BookedDate | AmendedDate | RejectedDate | CancelledDate | FilledDate          | Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
	| Vodafone     | 0       | 01/01/2018 09:30:00 |            |             |              |               | 01/01/2018 09:30:00 | MARKET | BUY       | USD      |            | 100              | 100           | 100          |
	And With the intraday market data :
	| SecurityName | Epoch	             | Bid | Ask | Price | Currency | Volume      |
	| Vodafone     | 01/01/2018 09:30:00 | 101 | 101 | 98    | USD      | 10000		  |
	When I run the high profit rule
	Then I will have 0 high profit alerts