@markingtheclose
@markingtheclosedaily
@markingtheclosedailynonsensitive
Feature: MarkingTheClose Daily Non Sensitive Parameters
	In order to meet MAR compliance requirements
	I need to be able to detect when traders are executing trades
	towards the market closure time at an unusually
	high volume in order to extract supernormal profits

Background:
			Given I have the marking the close rule parameter values
			| WindowHours | PercentageThresholdDailyVolume | PercentageThresholdWindowVolume |
			| 1			  |	0.5							   |						         |
		
Scenario: Empty Universe yields no alerts
		 Given I have the orders for a universe from 01/01/2018 to 01/05/2018 :
         | SecurityName | OrderId | PlacedDate | FilledDate | Type | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
         When I run the marking the close rule
		 Then I will have 0 marking the close alerts

Scenario: Marking the close just out of window raises 0 alerts
		Given I have the orders for a universe from 01/01/2019 to 01/01/2019 :
         | SecurityName | OrderId | PlacedDate			| CancelledDate | Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
         | Barclays     | 1       | 01/01/2019 14:59:00 |               | Market | Buy       | GBX      |            |                  | 500           | 500          |
         | Barclays     | 2       | 01/01/2019 14:59:00 |               | Market | Buy       | GBX      |            |                  | 500           | 500          |
		And With the interday market data :
		| SecurityName | Epoch      | OpenPrice | ClosePrice | HighIntradayPrice | LowIntradayPrice | ListedSecurities | MarketCap | DailyVolume | Currency |
		| Barclays     | 01/01/2019 | 10        | 11         | 11.5              | 10               | 10               | 1000000  | 10000       | GBX      |
		| Barclays     | 01/02/2019 | 10        | 11         | 11.5              | 10               | 10               | 1000000  | 10000       | GBX      |
		 When I run the marking the close rule
		 Then I will have 0 marking the close alerts

Scenario: Marking the close raises 1 alerts
		Given I have the orders for a universe from 01/01/2019 to 01/01/2019 :
         | SecurityName | OrderId | PlacedDate			| CancelledDate | Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
         | Barclays     | 1       | 01/01/2019 15:35:00 |               | Market | Buy       | GBX      |            |                  | 2500          | 2500         |
         | Barclays     | 2       | 01/01/2019 15:35:00 |               | Market | Buy       | GBX      |            |                  | 2500          | 2500         |
		And With the interday market data :
		| SecurityName | Epoch      | OpenPrice | ClosePrice | HighIntradayPrice | LowIntradayPrice | ListedSecurities | MarketCap | DailyVolume | Currency |
		| Barclays     | 01/01/2019 | 10        | 11         | 11.5              | 10               | 10               | 1000000  | 10000       | GBX      |
		| Barclays     | 01/02/2019 | 10        | 11         | 11.5              | 10               | 10               | 1000000  | 10000       | GBX      |
		 When I run the marking the close rule
		 Then I will have 1 marking the close alerts

Scenario: Marking the close raises 1 alerts for nasdaq
		Given I have the orders for a universe from 01/01/2019 to 01/01/2019 :
         | SecurityName | OrderId | PlacedDate			| CancelledDate | Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
         | Micron     | 1       | 01/01/2019 22:35:00 |               | Market | Buy       | USD      |            |                  | 2500          | 2500         |
         | Micron     | 2       | 01/01/2019 22:35:00 |               | Market | Buy       | USD      |            |                  | 2500          | 2500         |
		And With the interday market data :
		| SecurityName | Epoch      | OpenPrice | ClosePrice | HighIntradayPrice | LowIntradayPrice | ListedSecurities | MarketCap | DailyVolume | Currency |
		| Micron     | 01/01/2019 | 10        | 11         | 11.5              | 10               | 10               | 1000000  | 10000       | USD      |
		| Micron     | 01/02/2019 | 10        | 11         | 11.5              | 10               | 10               | 1000000  | 10000       | USD      |
		 When I run the marking the close rule
		 Then I will have 1 marking the close alerts

Scenario: Marking the close raises 0 alerts for market pressure cancelling out
		Given I have the orders for a universe from 01/01/2019 to 01/01/2019 :
         | SecurityName | OrderId | PlacedDate			| CancelledDate | Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
         | Micron     | 1       | 01/01/2019 22:35:00 |               | Market | Buy       | USD      |            |                  | 2500          | 2500         |
         | Micron     | 2       | 01/01/2019 22:35:00 |               | Market | Sell       | USD      |            |                  | 2500          | 2500         |
		And With the interday market data :
		| SecurityName | Epoch      | OpenPrice | ClosePrice | HighIntradayPrice | LowIntradayPrice | ListedSecurities | MarketCap | DailyVolume | Currency |
		| Micron     | 01/01/2019 | 10        | 11         | 11.5              | 10               | 10               | 1000000  | 10000       | USD      |
		| Micron     | 01/02/2019 | 10        | 11         | 11.5              | 10               | 10               | 1000000  | 10000       | USD      |
		 When I run the marking the close rule
		 Then I will have 0 marking the close alerts


Scenario: Marking the close raises 2 alerts for differnet days
		Given I have the orders for a universe from 01/01/2019 to 01/02/2019 :
         | SecurityName | OrderId | PlacedDate			| CancelledDate | Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
         | Barclays     | 1       | 01/01/2019 15:35:00 |               | Market | Buy       | GBX      |            |                  | 2500          | 2500         |
         | Barclays     | 2       | 01/01/2019 15:35:00 |               | Market | Buy       | GBX      |            |                  | 2500          | 2500         |
         | Barclays     | 3       | 01/02/2019 15:35:00 |               | Market | Buy       | GBX      |            |                  | 2500          | 2500         |
         | Barclays     | 4       | 01/02/2019 15:35:00 |               | Market | Buy       | GBX      |            |                  | 2500          | 2500         |
		And With the interday market data :
		| SecurityName | Epoch      | OpenPrice | ClosePrice | HighIntradayPrice | LowIntradayPrice | ListedSecurities | MarketCap | DailyVolume | Currency |
		| Barclays     | 01/01/2019 | 10        | 11         | 11.5              | 10               | 10               | 1000000  | 10000       | GBX      |
		| Barclays     | 01/02/2019 | 10        | 11         | 11.5              | 10               | 10               | 1000000  | 10000       | GBX      |
		 When I run the marking the close rule
		 Then I will have 2 marking the close alerts

Scenario: Marking the close raises 0 alerts for differnet days
		Given I have the orders for a universe from 01/01/2019 to 01/02/2019 :
         | SecurityName | OrderId | PlacedDate			| CancelledDate | Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
         | Barclays     | 1       | 01/01/2019 15:35:00 |               | Market | Buy       | GBX      |            |                  | 50            | 50           |
         | Barclays     | 2       | 01/01/2019 15:35:00 |               | Market | Buy       | GBX      |            |                  | 50            | 50           |
         | Barclays     | 3       | 01/02/2019 15:35:00 |               | Market | Buy       | GBX      |            |                  | 50            | 50           |
         | Barclays     | 4       | 01/02/2019 15:35:00 |               | Market | Buy       | GBX      |            |                  | 50            | 50           |
		And With the interday market data :
		| SecurityName | Epoch      | OpenPrice | ClosePrice | HighIntradayPrice | LowIntradayPrice | ListedSecurities | MarketCap | DailyVolume | Currency |
		| Barclays     | 01/01/2019 | 10        | 11         | 11.5              | 10               | 10               | 1000000  | 10000       | GBX      |
		| Barclays     | 01/02/2019 | 10        | 11         | 11.5              | 10               | 10               | 1000000  | 10000       | GBX      |
		 When I run the marking the close rule
		 Then I will have 0 marking the close alerts


Scenario: Inside Time Window yields 1 alert
		Given I have the orders for a universe from 01/01/2019 to 01/01/2019 :
         | SecurityName | OrderId | PlacedDate			| CancelledDate | Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
         | Micron     | 1       | 01/01/2019 22:35:00 |               | Market | Buy       | USD      |            |                  | 2500          | 2500         |
         | Micron     | 2       | 01/01/2019 22:35:00 |               | Market | Buy       | USD      |            |                  | 2500          | 2500         |
		And With the interday market data :
		| SecurityName | Epoch      | OpenPrice | ClosePrice | HighIntradayPrice | LowIntradayPrice | ListedSecurities | MarketCap | DailyVolume | Currency |
		| Micron     | 01/01/2019 | 10        | 11         | 11.5              | 10               | 10               | 1000000  | 10000       | USD      |
		| Micron     | 01/02/2019 | 10        | 11         | 11.5              | 10               | 10               | 1000000  | 10000       | USD      |
		 When I run the marking the close rule
		 Then I will have 1 marking the close alerts


Scenario: Outside Time Window yields 0 alert
		Given I have the orders for a universe from 01/01/2019 to 01/01/2019 :
         | SecurityName | OrderId | PlacedDate			| CancelledDate | Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
         | Micron     | 1       | 01/01/2019 23:35:00 |               | Market | Buy       | USD      |            |                  | 2500          | 2500         |
         | Micron     | 2       | 01/01/2019 23:35:00 |               | Market | Buy       | USD      |            |                  | 2500          | 2500         |
		And With the interday market data :
		| SecurityName | Epoch      | OpenPrice | ClosePrice | HighIntradayPrice | LowIntradayPrice | ListedSecurities | MarketCap | DailyVolume | Currency |
		| Micron     | 01/01/2019 | 10        | 11         | 11.5              | 10               | 10               | 1000000  | 10000       | USD      |
		| Micron     | 01/02/2019 | 10        | 11         | 11.5              | 10               | 10               | 1000000  | 10000       | USD      |
		 When I run the marking the close rule
		 Then I will have 0 marking the close alerts

Scenario: Inside PercentageThresholdDailyVolume yields 1 alert
		Given I have the orders for a universe from 01/01/2019 to 01/01/2019 :
         | SecurityName | OrderId | PlacedDate			| CancelledDate | Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
         | Micron     | 1       | 01/01/2019 22:35:00 |               | Market | Buy       | USD      |            |                  | 2500          | 3500         |
         | Micron     | 2       | 01/01/2019 22:35:00 |               | Market | Buy       | USD      |            |                  | 2500          | 3500         |
		And With the interday market data :
		| SecurityName | Epoch      | OpenPrice | ClosePrice | HighIntradayPrice | LowIntradayPrice | ListedSecurities | MarketCap | DailyVolume | Currency |
		| Micron     | 01/01/2019 | 10        | 11         | 11.5              | 10               | 10               | 1000000  | 10000       | USD      |
		| Micron     | 01/02/2019 | 10        | 11         | 11.5              | 10               | 10               | 1000000  | 10000       | USD      |
		 When I run the marking the close rule
		 Then I will have 1 marking the close alerts

Scenario: Outside PercentageThresholdDailyVolume yields 0 alert
		Given I have the orders for a universe from 01/01/2019 to 01/01/2019 :
         | SecurityName | OrderId | PlacedDate			| CancelledDate | Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
         | Micron     | 1       | 01/01/2019 22:35:00 |               | Market | Buy       | USD      |            |                  | 2500          | 500         |
         | Micron     | 2       | 01/01/2019 22:35:00 |               | Market | Buy       | USD      |            |                  | 2500          | 500         |
		And With the interday market data :
		| SecurityName | Epoch      | OpenPrice | ClosePrice | HighIntradayPrice | LowIntradayPrice | ListedSecurities | MarketCap | DailyVolume | Currency |
		| Micron     | 01/01/2019 | 10        | 11         | 11.5              | 10               | 10               | 1000000  | 10000       | USD      |
		| Micron     | 01/02/2019 | 10        | 11         | 11.5              | 10               | 10               | 1000000  | 10000       | USD      |
		 When I run the marking the close rule
		 Then I will have 0 marking the close alerts