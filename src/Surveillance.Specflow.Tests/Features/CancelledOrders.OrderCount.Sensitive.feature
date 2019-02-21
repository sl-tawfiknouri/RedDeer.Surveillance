@cancelledorders
@cancelledorderscount
@cancelledordercountsensitive
Feature: CancelledOrders Order Count Sensitive Parameters
		In order to meet MAR compliance requirements
		I need to be able to detect when traders are placing orders
		which are then cancelled in an unusual pattern

Background:
			Given I have the cancelled orders rule parameter values
			| WindowHours | CancelledOrderPercentagePositionThreshold | CancelledOrderCountPercentageThreshold | MinimumNumberOfTradesToApplyRuleTo | MaximumNumberOfTradesToApplyRuleTo |
			| 1			  |											  |									  0.1 |									 2 |									 |
		
Scenario: Empty Universe yields no alerts
		 Given I have the orders for a universe from 01/01/2018 to 01/05/2018 :
         | SecurityName | OrderId | PlacedDate | FilledDate | Type | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
         When I run the cancelled orders rule
		 Then I will have 0 cancelled orders alerts

Scenario: No cancelled orders just placed in range yields no alerts
		Given I have the orders for a universe from 01/01/2019 to 01/01/2019 :
         | SecurityName | OrderId | PlacedDate | CancelledDate | Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
         | Barclays     | 1       | 01/01/2019 |               | Market | Buy       | GBX      |            |                  | 100           |              |
         | Barclays     | 2       | 01/01/2019 |               | Market | Buy       | GBX      |            |                  | 100           |              |
		 When I run the cancelled orders rule
		 Then I will have 0 cancelled orders alerts

Scenario: One cancelled orders just out of range out of two yields one alert
		Given I have the orders for a universe from 01/01/2019 to 01/01/2019 :
         | SecurityName | OrderId | PlacedDate			| CancelledDate | Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
         | Barclays     | 1       | 01/01/2019 09:30:00 |               | Market | Buy       | GBX      |            |                  | 100           |              |
         | Barclays     | 2       | 01/01/2019 10:31:00	| 01/01/2019    | Market | Buy       | GBX      |            |                  | 100           |              |
		 When I run the cancelled orders rule
		 Then I will have 0 cancelled orders alerts

Scenario: One cancelled orders just in range out of two yields one alert
		Given I have the orders for a universe from 01/01/2019 to 01/01/2019 :
         | SecurityName | OrderId | PlacedDate			| CancelledDate | Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
         | Barclays     | 1       | 01/01/2019 09:30:00 |               | Market | Buy       | GBX      |            |                  | 100           |              |
         | Barclays     | 2       | 01/01/2019 10:29:00	| 01/01/2019    | Market | Buy       | GBX      |            |                  | 100           |              |
		 When I run the cancelled orders rule
		 Then I will have 1 cancelled orders alerts

Scenario: One cancelled orders exactly on the range out of two yields one alert
		Given I have the orders for a universe from 01/01/2019 to 01/01/2019 :
         | SecurityName | OrderId | PlacedDate			| CancelledDate | Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
         | Barclays     | 1       | 01/01/2019 09:30:00 |               | Market | Buy       | GBX      |            |                  | 100           |              |
         | Barclays     | 2       | 01/01/2019 10:30:00	| 01/01/2019    | Market | Buy       | GBX      |            |                  | 100           |              |
		 When I run the cancelled orders rule
		 Then I will have 1 cancelled orders alerts

Scenario: One cancelled order in a buy sell pattern yields one alert
		Given I have the orders for a universe from 01/01/2019 to 01/01/2019 :
         | SecurityName | OrderId | PlacedDate			| CancelledDate | Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
         | Barclays     | 1       | 01/01/2019 09:30:00 |               | Market | Buy       | GBX      |            |                  | 100           |              |
         | Barclays     | 2       | 01/01/2019 10:29:00	| 01/01/2019    | Market | Sell       | GBX      |            |                  | 100           |              |
		 When I run the cancelled orders rule
		 Then I will have 1 cancelled orders alerts

Scenario: One cancelled order in a short cover pattern yields one alert
		Given I have the orders for a universe from 01/01/2019 to 01/01/2019 :
         | SecurityName | OrderId | PlacedDate			| CancelledDate | Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
         | Barclays     | 1       | 01/01/2019 09:30:00 |               | Market | Short     | GBX      |            |                  | 100           |              |
         | Barclays     | 2       | 01/01/2019 10:29:00	| 01/01/2019    | Market | Cover     | GBX      |            |                  | 100           |              |
		 When I run the cancelled orders rule
		 Then I will have 1 cancelled orders alerts

Scenario: One cancelled order only yields no alert
		Given I have the orders for a universe from 01/01/2019 to 01/01/2019 :
         | SecurityName | OrderId | PlacedDate			| CancelledDate | Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
         | Barclays     | 1       | 01/01/2019 10:30:00	| 01/01/2019    | Market | Buy       | GBX      |            |                  | 100           |              |
		 When I run the cancelled orders rule
		 Then I will have 0 cancelled orders alerts

Scenario: One cancelled orders out of eleven yields ten alerts
		Given I have the orders for a universe from 01/01/2019 to 01/01/2019 :
         | SecurityName | OrderId | PlacedDate | CancelledDate | Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
         | Barclays     | 1       | 01/01/2019 |               | Market | Buy       | GBX      |            |                  | 100           |              |
         | Barclays     | 2       | 01/01/2019 | 01/01/2019    | Market | Buy       | GBX      |            |                  | 100           |              |
         | Barclays     | 3       | 01/01/2019 |               | Market | Buy       | GBX      |            |                  | 100           |              |
         | Barclays     | 4       | 01/01/2019 |               | Market | Buy       | GBX      |            |                  | 100           |              |
         | Barclays     | 5       | 01/01/2019 |               | Market | Buy       | GBX      |            |                  | 100           |              |
         | Barclays     | 6       | 01/01/2019 |               | Market | Buy       | GBX      |            |                  | 100           |              |
         | Barclays     | 7       | 01/01/2019 |               | Market | Buy       | GBX      |            |                  | 100           |              |
         | Barclays     | 8       | 01/01/2019 |               | Market | Buy       | GBX      |            |                  | 100           |              |
         | Barclays     | 9       | 01/01/2019 |               | Market | Buy       | GBX      |            |                  | 100           |              |
         | Barclays     | 10      | 01/01/2019 |               | Market | Buy       | GBX      |            |                  | 100           |              |
         | Barclays     | 11      | 01/01/2019 |               | Market | Buy       | GBX      |            |                  | 100           |              |
		 When I run the cancelled orders rule
		 Then I will have 10 cancelled orders alerts

Scenario: One cancelled orders out of ten yields one alert
		Given I have the orders for a universe from 01/01/2019 to 01/01/2019 :
         | SecurityName | OrderId | PlacedDate | CancelledDate | Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
         | Barclays     | 1       | 01/01/2019 |               | Market | Buy       | GBX      |            |                  | 100           |              |
         | Barclays     | 2       | 01/01/2019 | 01/01/2019    | Market | Buy       | GBX      |            |                  | 100           |              |
         | Barclays     | 3       | 01/01/2019 |               | Market | Buy       | GBX      |            |                  | 100           |              |
         | Barclays     | 4       | 01/01/2019 |               | Market | Buy       | GBX      |            |                  | 100           |              |
         | Barclays     | 5       | 01/01/2019 |               | Market | Buy       | GBX      |            |                  | 100           |              |
         | Barclays     | 6       | 01/01/2019 |               | Market | Buy       | GBX      |            |                  | 100           |              |
         | Barclays     | 7       | 01/01/2019 |               | Market | Buy       | GBX      |            |                  | 100           |              |
         | Barclays     | 8       | 01/01/2019 |               | Market | Buy       | GBX      |            |                  | 100           |              |
         | Barclays     | 9       | 01/01/2019 |               | Market | Buy       | GBX      |            |                  | 100           |              |
         | Barclays     | 10      | 01/01/2019 |               | Market | Buy       | GBX      |            |                  | 100           |              |
		 When I run the cancelled orders rule
		 Then I will have 9 cancelled orders alerts

Scenario: Ten cancelled orders out of eleven yields one alert
		Given I have the orders for a universe from 01/01/2019 to 01/01/2019 :
         | SecurityName | OrderId | PlacedDate | CancelledDate | Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
         | Barclays     | 1       | 01/01/2019 |               | Market | Sell       | GBX      |            |                  | 100           |              |
         | Barclays     | 2       | 01/01/2019 | 01/01/2019    | Market | Sell       | GBX      |            |                  | 100           |              |
         | Barclays     | 3       | 01/01/2019 | 01/01/2019    | Market | Sell       | GBX      |            |                  | 100           |              |
         | Barclays     | 4       | 01/01/2019 | 01/01/2019    | Market | Sell       | GBX      |            |                  | 100           |              |
         | Barclays     | 5       | 01/01/2019 | 01/01/2019    | Market | Sell       | GBX      |            |                  | 100           |              |
         | Barclays     | 6       | 01/01/2019 | 01/01/2019    | Market | Sell       | GBX      |            |                  | 100           |              |
         | Barclays     | 7       | 01/01/2019 | 01/01/2019    | Market | Sell       | GBX      |            |                  | 100           |              |
         | Barclays     | 8       | 01/01/2019 | 01/01/2019    | Market | Sell       | GBX      |            |                  | 100           |              |
         | Barclays     | 9       | 01/01/2019 | 01/01/2019    | Market | Sell       | GBX      |            |                  | 100           |              |
         | Barclays     | 10      | 01/01/2019 | 01/01/2019    | Market | Sell       | GBX      |            |                  | 100           |              |
         | Barclays     | 11      | 01/01/2019 | 01/01/2019    | Market | Sell       | GBX      |            |                  | 100           |              |
		 When I run the cancelled orders rule
		 Then I will have 10 cancelled orders alerts

