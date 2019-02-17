@markingtheclose
@markingtheclosewindow
@markingtheclosewindowsensitive
Feature: MarkingTheClose Window Sensitive Parameters
	In order to meet MAR compliance requirements
	I need to be able to detect when traders are executing trades
	towards the market closure time at an unusually
	high volume in order to extract supernormal profits

Background:
			Given I have the marking the close rule parameter values
			| WindowHours | PercentageThresholdDailyVolume | PercentageThresholdWindowVolume |
			| 1           |                                | 0.1							 |

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
		 	And With the intraday market data :
		| SecurityName | Epoch      | Bid | Ask | Price | Currency | Volume |
		| Barclays | 01/01/2019  16:00:00| 1	  | 20  | 10    | GBX      | 5000  |
		| Barclays | 01/01/2019  15:55:00| 1	  | 20  | 10    | GBX      | 5000  |
		 When I run the marking the close rule
		 Then I will have 0 marking the close alerts

Scenario: Marking the close raises 1 alerts
		Given I have the orders for a universe from 01/01/2019 to 01/01/2019 :
         | SecurityName | OrderId | PlacedDate			| CancelledDate | Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
         | Barclays     | 1       | 01/01/2019 15:35:00 |               | Market | Buy       | GBX      |            |                  | 500           | 500          |
         | Barclays     | 2       | 01/01/2019 15:35:00 |               | Market | Buy       | GBX      |            |                  | 500           | 500          |
		 	And With the intraday market data :
		| SecurityName | Epoch      | Bid | Ask | Price | Currency | Volume |
		| Barclays | 01/01/2019  16:00:00| 1	  | 20  | 10    | GBX      | 5000  |
		| Barclays | 01/01/2019  15:55:00| 1	  | 20  | 10    | GBX      | 5000  |
		 When I run the marking the close rule
		 Then I will have 1 marking the close alerts

Scenario: Marking the close raises 2 alerts for differnet days
		Given I have the orders for a universe from 01/01/2019 to 01/02/2019 :
         | SecurityName | OrderId | PlacedDate			| CancelledDate | Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
         | Barclays     | 1       | 01/01/2019 15:35:00 |               | Market | Buy       | GBX      |            |                  | 500           | 500          |
         | Barclays     | 2       | 01/01/2019 15:35:00 |               | Market | Buy       | GBX      |            |                  | 500           | 500          |
         | Barclays     | 3       | 01/02/2019 15:35:00 |               | Market | Buy       | GBX      |            |                  | 500           | 500          |
         | Barclays     | 4       | 01/02/2019 15:35:00 |               | Market | Buy       | GBX      |            |                  | 500           | 500          |
		 	And With the intraday market data :
		| SecurityName | Epoch      | Bid | Ask | Price | Currency | Volume |
		| Barclays | 01/01/2019  16:00:00| 1	  | 20  | 10    | GBX      | 5000  |
		| Barclays | 01/01/2019  15:55:00| 1	  | 20  | 10    | GBX      | 5000  |
		| Barclays | 01/02/2019  16:00:00| 1	  | 20  | 10    | GBX      | 5000  |
		| Barclays | 01/02/2019  15:55:00| 1	  | 20  | 10    | GBX      | 5000  |
		 When I run the marking the close rule
		 Then I will have 2 marking the close alerts

Scenario: Marking the close raises 0 alerts for differnet days
		Given I have the orders for a universe from 01/01/2019 to 01/02/2019 :
         | SecurityName | OrderId | PlacedDate			| CancelledDate | Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
         | Barclays     | 1       | 01/01/2019 15:35:00 |               | Market | Buy       | GBX      |            |                  | 50            | 50           |
         | Barclays     | 2       | 01/01/2019 15:35:00 |               | Market | Buy       | GBX      |            |                  | 50            | 50           |
         | Barclays     | 3       | 01/02/2019 15:35:00 |               | Market | Buy       | GBX      |            |                  | 50            | 50           |
         | Barclays     | 4       | 01/02/2019 15:35:00 |               | Market | Buy       | GBX      |            |                  | 50            | 50           |
		 	And With the intraday market data :
		| SecurityName | Epoch      | Bid | Ask | Price | Currency | Volume |
		| Barclays | 01/01/2019  16:00:00| 1	  | 20  | 10    | GBX      | 5000  |
		| Barclays | 01/01/2019  15:55:00| 1	  | 20  | 10    | GBX      | 5000  |
		| Barclays | 01/02/2019  16:00:00| 1	  | 20  | 10    | GBX      | 5000  |
		| Barclays | 01/02/2019  15:55:00| 1	  | 20  | 10    | GBX      | 5000  |
		 When I run the marking the close rule
		 Then I will have 0 marking the close alerts



