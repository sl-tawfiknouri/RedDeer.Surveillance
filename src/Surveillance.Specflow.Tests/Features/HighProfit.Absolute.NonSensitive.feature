Feature: HighProfit Absolute Non Sensitive Parameters
	In order to meet MAR compliance requirements
	I need to be able to detect when traders are executing trades
	Which generate unusual levels of profits
	By measuring their security trade profits as an absolute return

#High profit rule alerts have twice as many alerts to account for market closure analysis
#This is deduplicated elsewhere in the rules process

Background:
	Given I have the high profit rule parameter values
	| WindowHours | HighProfitPercentage | HighProfitAbsolute | HighProfitCurrency | HighProfitUseCurrencyConversions |
	| 1           |                      | 100000             | GBX                | false                            |

@highprofit
@highprofitabsolute
@highprofitabsolutenonsensitive
Scenario: Empty Universe yields no alerts
	Given I have the orders for a universe from 01/01/2018 to 03/01/2018 :
	| SecurityName | OrderId | PlacedDate          | BookedDate | AmendedDate | RejectedDate | CancelledDate | FilledDate          | Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
	When I run the high profit rule
	Then I will have 0 high profit alerts

@highprofit
@highprofitabsolute
@highprofitabsolutenonsensitive
Scenario: Single order yields no alerts
	Given I have the orders for a universe from 01/01/2018 to 03/01/2018 :
	| SecurityName | OrderId | PlacedDate          | BookedDate | AmendedDate | RejectedDate | CancelledDate | FilledDate          | Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
	| Vodafone     | 0       | 01/01/2018 09:30:00 |            |             |              |               | 01/01/2018 09:30:00 | MARKET | BUY       | GBX      |            | 10               | 100           | 100          |
	When I run the high profit rule
	Then I will have 0 high profit alerts

@highprofit
@highprofitabsolute
@highprofitabsolutenonsensitive
Scenario: Buy Sell orders within window yields two alerts
	Given I have the orders for a universe from 01/01/2018 to 03/01/2018 :
	| SecurityName | OrderId | PlacedDate          | BookedDate | AmendedDate | RejectedDate | CancelledDate | FilledDate          | Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
	| Vodafone     | 0       | 01/01/2018 09:30:00 |            |             |              |               | 01/01/2018 09:30:00 | MARKET | BUY       | GBX      |            | 100              | 100000	       | 100000       |
	| Vodafone     | 1       | 01/01/2018 09:45:00 |            |             |              |               | 01/01/2018 09:45:00 | MARKET | SELL      | GBX      |            | 110              | 100000	       | 100000       |
	When I run the high profit rule
	Then I will have 2 high profit alerts

@highprofit
@highprofitabsolute
@highprofitabsolutenonsensitive
Scenario: Buy Sell orders different exchange within window yields two alerts
	Given I have the orders for a universe from 01/01/2018 to 03/01/2018 :
	| SecurityName | OrderId | PlacedDate          | BookedDate | AmendedDate | RejectedDate | CancelledDate | FilledDate          | Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
	| Nvidia     | 0       | 01/01/2018 09:30:00 |            |             |              |               | 01/01/2018 09:30:00 | MARKET | BUY       | USD      |            | 100              | 100000	       | 100000       |
	| Nvidia     | 1       | 01/01/2018 09:45:00 |            |             |              |               | 01/01/2018 09:45:00 | MARKET | SELL      | USD      |            | 110              | 100000	       | 100000       |
	When I run the high profit rule
	Then I will have 2 high profit alerts	   

@highprofit
@highprofitabsolute
@highprofitabsolutenonsensitive
Scenario: Buy Sell orders at exact window yields two alerts
	Given I have the orders for a universe from 01/01/2018 to 03/01/2018 :
	| SecurityName | OrderId | PlacedDate          | BookedDate | AmendedDate | RejectedDate | CancelledDate | FilledDate          | Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
	| Vodafone     | 0       | 01/01/2018 09:30:00 |            |             |              |               | 01/01/2018 09:30:00 | MARKET | BUY       | GBX      |            | 100              | 100000	       | 100000       |
	| Vodafone     | 1       | 01/01/2018 10:30:00 |            |             |              |               | 01/01/2018 10:30:00 | MARKET | SELL      | GBX      |            | 110              | 100000	       | 100000       |
	When I run the high profit rule
	Then I will have 2 high profit alerts

@highprofit
@highprofitabsolute
@highprofitabsolutenonsensitive
Scenario: Buy Sell orders but outside of window yields zero alerts
	Given I have the orders for a universe from 01/01/2018 to 03/01/2018 :
	| SecurityName | OrderId | PlacedDate          | BookedDate | AmendedDate | RejectedDate | CancelledDate | FilledDate          | Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
	| Vodafone     | 0       | 01/01/2018 09:30:00 |            |             |              |               | 01/01/2018 09:30:00 | MARKET | BUY       | GBX      |            | 100              | 100000	       | 100000       |
	| Vodafone     | 1       | 01/01/2018 10:35:00 |            |             |              |               | 01/01/2018 10:35:00 | MARKET | SELL      | GBX      |            | 110              | 100000	       | 100000       |
	When I run the high profit rule
	Then I will have 0 high profit alerts


@highprofit
@highprofitabsolute
@highprofitabsolutenonsensitive
Scenario: Buy just buy orders yields two alerts
	Given I have the orders for a universe from 01/01/2018 to 03/01/2018 :
	| SecurityName | OrderId | PlacedDate          | BookedDate | AmendedDate | RejectedDate | CancelledDate | FilledDate          | Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
	| Vodafone     | 0       | 01/01/2018 09:30:00 |            |             |              |               | 01/01/2018 09:30:00 | MARKET | BUY       | GBX      |            | 100              | 100000	       | 100000       |
	| Vodafone     | 1       | 01/01/2018 09:30:00 |            |             |              |               | 01/01/2018 09:30:00 | MARKET | BUY       | GBX      |            | 110              | 100000	       | 100000       |
	And With the intraday market data :
	| SecurityName | Epoch	             | Bid | Ask | Price | Currency | Volume      |
	| Vodafone     | 01/01/2018 09:30:00 | 101 | 101 | 110   | GBX      | 10000		  |
	When I run the high profit rule
	Then I will have 2 high profit alerts

@highprofit
@highprofitabsolute
@highprofitabsolutenonsensitive
Scenario: Cover just cover orders yields two alerts
	Given I have the orders for a universe from 01/01/2018 to 03/01/2018 :
	| SecurityName | OrderId | PlacedDate          | BookedDate | AmendedDate | RejectedDate | CancelledDate | FilledDate          | Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
	| Vodafone     | 0       | 01/01/2018 09:30:00 |            |             |              |               | 01/01/2018 09:30:00 | MARKET | COVER       | GBX      |            | 100              | 100000	       | 100000       |
	| Vodafone     | 1       | 01/01/2018 09:30:00 |            |             |              |               | 01/01/2018 09:30:00 | MARKET | COVER       | GBX      |            | 110              | 100000	       | 100000       |
	And With the intraday market data :
	| SecurityName | Epoch	             | Bid | Ask | Price | Currency | Volume      |
	| Vodafone     | 01/01/2018 09:30:00 | 101 | 101 | 110   | GBX      | 10000		  |
	When I run the high profit rule
	Then I will have 2 high profit alerts

@highprofit
@highprofitabsolute
@highprofitabsolutenonsensitive
Scenario: Sell just sell orders yields zero alerts
	Given I have the orders for a universe from 01/01/2018 to 03/01/2018 :
	| SecurityName | OrderId | PlacedDate          | BookedDate | AmendedDate | RejectedDate | CancelledDate | FilledDate          | Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
	| Vodafone     | 0       | 01/01/2018 09:30:00 |            |             |              |               | 01/01/2018 09:30:00 | MARKET | SELL       | GBX      |            | 100              | 100000	       | 100000       |
	| Vodafone     | 1       | 01/01/2018 09:30:00 |            |             |              |               | 01/01/2018 09:30:00 | MARKET | SELL       | GBX      |            | 110              | 100000	       | 100000       |
	And With the intraday market data :
	| SecurityName | Epoch	             | Bid | Ask | Price | Currency | Volume      |
	| Vodafone     | 01/01/2018 09:30:00 | 101 | 101 | 110   | GBX      | 10000		  |
	When I run the high profit rule
	Then I will have 0 high profit alerts

@highprofit
@highprofitabsolute
@highprofitabsolutenonsensitive
Scenario: Short just short orders yields zero alerts
	Given I have the orders for a universe from 01/01/2018 to 03/01/2018 :
	| SecurityName | OrderId | PlacedDate          | BookedDate | AmendedDate | RejectedDate | CancelledDate | FilledDate          | Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
	| Vodafone     | 0       | 01/01/2018 09:30:00 |            |             |              |               | 01/01/2018 09:30:00 | MARKET | SHORT       | GBX      |            | 100              | 100000	       | 100000       |
	| Vodafone     | 1       | 01/01/2018 09:30:00 |            |             |              |               | 01/01/2018 09:30:00 | MARKET | SHORT       | GBX      |            | 110              | 100000	       | 100000       |
	And With the intraday market data :
	| SecurityName | Epoch	             | Bid | Ask | Price | Currency | Volume      |
	| Vodafone     | 01/01/2018 09:30:00 | 101 | 101 | 110   | GBX      | 10000		  |
	When I run the high profit rule
	Then I will have 0 high profit alerts

@highprofit
@highprofitabsolute
@highprofitabsolutenonsensitive
Scenario: Buy Sell orders yields two alerts
	Given I have the orders for a universe from 01/01/2018 to 03/01/2018 :
	| SecurityName | OrderId | PlacedDate          | BookedDate | AmendedDate | RejectedDate | CancelledDate | FilledDate          | Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
	| Vodafone     | 0       | 01/01/2018 09:30:00 |            |             |              |               | 01/01/2018 09:30:00 | MARKET | BUY       | GBX      |            | 100              | 100000	       | 100000       |
	| Vodafone     | 1       | 01/01/2018 09:30:00 |            |             |              |               | 01/01/2018 09:30:00 | MARKET | SELL      | GBX      |            | 110              | 100000	       | 100000       |
	When I run the high profit rule
	Then I will have 2 high profit alerts

@highprofit
@highprofitabsolute
@highprofitabsolutenonsensitive
Scenario: Buy Sell orders at exact threshold yields two alerts
	Given I have the orders for a universe from 01/01/2018 to 03/01/2018 :
	| SecurityName | OrderId | PlacedDate          | BookedDate | AmendedDate | RejectedDate | CancelledDate | FilledDate          | Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
	| Vodafone     | 0       | 01/01/2018 09:30:00 |            |             |              |               | 01/01/2018 09:30:00 | MARKET | BUY       | GBX      |            | 100              | 100000     | 100000	    |
	| Vodafone     | 1       | 01/01/2018 09:30:00 |            |             |              |               | 01/01/2018 09:30:00 | MARKET | SELL      | GBX      |            | 101              | 100000     | 100000	    |
	When I run the high profit rule
	Then I will have 2 high profit alerts

@highprofit
@highprofitabsolute
@highprofitabsolutenonsensitive
Scenario: Buy Sell orders at just below threshold yields zero alerts
	Given I have the orders for a universe from 01/01/2018 to 03/01/2018 :
	| SecurityName | OrderId | PlacedDate          | BookedDate | AmendedDate | RejectedDate | CancelledDate | FilledDate          | Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
	| Vodafone     | 0       | 01/01/2018 09:30:00 |            |             |              |               | 01/01/2018 09:30:00 | MARKET | BUY       | GBX      |            | 1000             | 99999		   | 99999       |
	| Vodafone     | 1       | 01/01/2018 09:30:00 |            |             |              |               | 01/01/2018 09:30:00 | MARKET | SELL      | GBX      |            | 1001             | 99999		   | 99999       |
	When I run the high profit rule
	Then I will have 0 high profit alerts

@highprofit
@highprofitabsolute
@highprofitabsolutenonsensitive
Scenario: Buy order with increase in market price (bmll) yields one alert
	Given I have the orders for a universe from 01/01/2018 to 03/01/2018 :
	| SecurityName | OrderId | PlacedDate          | BookedDate | AmendedDate | RejectedDate | CancelledDate | FilledDate          | Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
	| Vodafone     | 0       | 01/01/2018 09:30:00 |            |             |              |               | 01/01/2018 09:30:00 | MARKET | BUY       | GBX      |            | 100              | 100000000     | 100000000    |
	And With the intraday market data :
	| SecurityName | Epoch	             | Bid | Ask | Price | Currency | Volume      |
	| Vodafone     | 01/01/2018 09:30:00 | 101 | 101 | 110   | GBX      | 10000		  |
	When I run the high profit rule
	Then I will have 1 high profit alerts


@highprofit
@highprofitabsolute
@highprofitabsolutenonsensitive
Scenario: Buy order with increase in market price to exact threshold (bmll) yields one alert
	Given I have the orders for a universe from 01/01/2018 to 03/01/2018 :
	| SecurityName | OrderId | PlacedDate          | BookedDate | AmendedDate | RejectedDate | CancelledDate | FilledDate          | Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
	| Vodafone     | 0       | 01/01/2018 09:30:00 |            |             |              |               | 01/01/2018 09:30:00 | MARKET | BUY       | GBX      |            | 100              | 100000        | 100000       |
	And With the intraday market data :
	| SecurityName | Epoch	             | Bid | Ask | Price | Currency | Volume      |
	| Vodafone     | 01/01/2018 09:30:00 | 101 | 101 | 101   | GBX      | 10000		  |
	When I run the high profit rule
	Then I will have 1 high profit alerts

@highprofit
@highprofitabsolute
@highprofitabsolutenonsensitive
Scenario: Buy order with substantial increase in market price (bmll) yields one alert
	Given I have the orders for a universe from 01/01/2018 to 03/01/2018 :
	| SecurityName | OrderId | PlacedDate          | BookedDate | AmendedDate | RejectedDate | CancelledDate | FilledDate          | Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
	| Vodafone     | 0       | 01/01/2018 09:30:00 |            |             |              |               | 01/01/2018 09:30:00 | MARKET | BUY       | GBX      |            | 100              | 100000        | 100000       |
	And With the intraday market data :
	| SecurityName | Epoch	             | Bid | Ask | Price | Currency | Volume      |
	| Vodafone     | 01/01/2018 09:30:00 | 101 | 101 | 110   | GBX      | 10000		  |
	When I run the high profit rule
	Then I will have 1 high profit alerts

@highprofit
@highprofitabsolute
@highprofitabsolutenonsensitive
Scenario: Buy order with decrease in market price (bmll) yields zero alerts
	Given I have the orders for a universe from 01/01/2018 to 03/01/2018 :
	| SecurityName | OrderId | PlacedDate          | BookedDate | AmendedDate | RejectedDate | CancelledDate | FilledDate          | Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
	| Vodafone     | 0       | 01/01/2018 09:30:00 |            |             |              |               | 01/01/2018 09:30:00 | MARKET | BUY       | GBX      |            | 100              | 100           | 100          |
	And With the intraday market data :
	| SecurityName | Epoch	             | Bid | Ask | Price | Currency | Volume      |
	| Vodafone     | 01/01/2018 09:30:00 | 101 | 101 | 98    | GBX      | 10000		  |
	When I run the high profit rule
	Then I will have 0 high profit alerts