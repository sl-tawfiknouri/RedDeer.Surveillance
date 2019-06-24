@placingorders
@placingorderssigma
@placingorderssigmasensitive
Feature: PlacingOrders Sigma Sensitive Parameters
		In order to meet MAR compliance requirements
		I need to be able to detect when traders are placing orders
		which were never likely to be executed


Background:
			Given I have the placing orders rule parameter values
			| WindowHours | Sigma | 
			| 24           | 1     | 

Scenario: Empty Universe yields no alerts
		 Given I have the orders for a universe from 01/01/2018 to 01/05/2018 :
         | SecurityName | OrderId | PlacedDate | FilledDate | Type | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
         When I run the placing orders rule
		 Then I will have 0 placing orders alerts


Scenario: No placing orders in range yields no alerts
		Given I have the orders for a universe from 01/01/2018 to 01/01/2018 :
         | SecurityName | OrderId | PlacedDate | CancelledDate   | Type   | Direction | Currency | LimitPrice    | AverageFillPrice | OrderedVolume | FilledVolume |
         | Barclays     | 1       | 01/01/2018 09:30:00 |               | Limit | Buy       | GBX      | 98           |                  | 100           |              |
         | Barclays     | 2       | 01/01/2018 09:30:00 |               | Limit | Buy       | GBX      | 98           |                  | 100           |              |
		And With the intraday market data :
		| SecurityName | Epoch				 | Bid	  | Ask | Price | Currency | Volume |
		| Barclays     | 01/01/2018  10:30:00| 1	  | 20  | 100    | GBX      | 5000  |
		| Barclays     | 01/01/2018  10:29:00| 1	  | 20  | 99    | GBX      | 5000  |
		| Barclays     | 01/01/2018  10:28:00| 1	  | 20  | 98    | GBX      | 5000  |
		| Barclays     | 01/01/2018  10:27:00| 1	  | 20  | 97    | GBX      | 5000  |
		| Barclays     | 01/01/2018  10:26:00| 1	  | 20  | 97    | GBX      | 5000  |
		| Barclays     | 01/01/2018  10:25:00| 1	  | 20  | 98    | GBX      | 5000  |
		| Barclays     | 01/01/2018  10:24:00| 1	  | 20  | 99    | GBX      | 5000  |
		| Barclays     | 01/01/2018  10:23:00| 1	  | 20  | 100    | GBX      | 5000  |
		| Barclays     | 01/01/2018  10:22:00| 1	  | 20  | 100    | GBX      | 5000  |
		| Barclays     | 01/01/2018  10:21:00| 1	  | 20  | 99    | GBX      | 5000  |
		| Barclays     | 01/01/2018  10:20:00| 1	  | 20  | 98    | GBX      | 5000  |
		| Barclays     | 01/01/2018  10:19:00| 1	  | 20  | 97    | GBX      | 5000  |
		 When I run the placing orders rule
		 Then I will have 0 placing orders alerts

Scenario: placing orders with one in sigma range yields one alerts
		Given I have the orders for a universe from 01/01/2018 to 01/01/2018 :
         | SecurityName | OrderId | PlacedDate | CancelledDate   | Type   | Direction | Currency | LimitPrice    | AverageFillPrice | OrderedVolume | FilledVolume |
         | Barclays     | 1       | 01/01/2018 09:30:00 |               | Limit | Sell       | GBX      | 108           |                  | 100           |              |
         | Barclays     | 2       | 01/01/2018 09:30:00 |               | Limit | Sell       | GBX      | 100           |                  | 100           |              |
		And With the intraday market data :
		| SecurityName | Epoch				 | Bid	  | Ask | Price | Currency | Volume |
		| Barclays     | 01/01/2018  10:30:00| 1	  | 20  | 100    | GBX      | 5000  |
		| Barclays     | 01/01/2018  10:29:00| 1	  | 20  | 99    | GBX      | 5000  |
		| Barclays     | 01/01/2018  10:28:00| 1	  | 20  | 98    | GBX      | 5000  |
		| Barclays     | 01/01/2018  10:27:00| 1	  | 20  | 97    | GBX      | 5000  |
		| Barclays     | 01/01/2018  10:26:00| 1	  | 20  | 97    | GBX      | 5000  |
		| Barclays     | 01/01/2018  10:25:00| 1	  | 20  | 98    | GBX      | 5000  |
		| Barclays     | 01/01/2018  10:24:00| 1	  | 20  | 99    | GBX      | 5000  |
		| Barclays     | 01/01/2018  10:23:00| 1	  | 20  | 100    | GBX      | 5000  |
		| Barclays     | 01/01/2018  10:22:00| 1	  | 20  | 100    | GBX      | 5000  |
		| Barclays     | 01/01/2018  10:21:00| 1	  | 20  | 99    | GBX      | 5000  |
		| Barclays     | 01/01/2018  10:20:00| 1	  | 20  | 98    | GBX      | 5000  |
		| Barclays     | 01/01/2018  10:19:00| 1	  | 20  | 97    | GBX      | 5000  |
		 When I run the placing orders rule
		 Then I will have 1 placing orders alerts	 


Scenario: placing orders with one in sigma below range yields one alerts
		Given I have the orders for a universe from 01/01/2018 to 01/01/2018 :
         | SecurityName | OrderId | PlacedDate | CancelledDate   | Type   | Direction | Currency | LimitPrice    | AverageFillPrice | OrderedVolume | FilledVolume |
         | Barclays     | 1       | 01/01/2018 09:30:00 |               | Limit | Sell       | GBX      | 98           |                  | 100           |              |
         | Barclays     | 2       | 01/01/2018 09:30:00 |               | Limit | Buy       | GBX      | 90           |                  | 100           |              |
		And With the intraday market data :
		| SecurityName | Epoch				 | Bid	  | Ask | Price | Currency | Volume |
		| Barclays     | 01/01/2018  10:30:00| 1	  | 20  | 100    | GBX      | 5000  |
		| Barclays     | 01/01/2018  10:29:00| 1	  | 20  | 99    | GBX      | 5000  |
		| Barclays     | 01/01/2018  10:28:00| 1	  | 20  | 98    | GBX      | 5000  |
		| Barclays     | 01/01/2018  10:27:00| 1	  | 20  | 97    | GBX      | 5000  |
		| Barclays     | 01/01/2018  10:26:00| 1	  | 20  | 97    | GBX      | 5000  |
		| Barclays     | 01/01/2018  10:25:00| 1	  | 20  | 98    | GBX      | 5000  |
		| Barclays     | 01/01/2018  10:24:00| 1	  | 20  | 99    | GBX      | 5000  |
		| Barclays     | 01/01/2018  10:23:00| 1	  | 20  | 100    | GBX      | 5000  |
		| Barclays     | 01/01/2018  10:22:00| 1	  | 20  | 100    | GBX      | 5000  |
		| Barclays     | 01/01/2018  10:21:00| 1	  | 20  | 99    | GBX      | 5000  |
		| Barclays     | 01/01/2018  10:20:00| 1	  | 20  | 98    | GBX      | 5000  |
		| Barclays     | 01/01/2018  10:19:00| 1	  | 20  | 97    | GBX      | 5000  |
		 When I run the placing orders rule
		 Then I will have 1 placing orders alerts	

Scenario: placing orders with two in sigma range yields one alerts
		Given I have the orders for a universe from 01/01/2018 to 01/01/2018 :
         | SecurityName | OrderId | PlacedDate | CancelledDate   | Type   | Direction | Currency | LimitPrice    | AverageFillPrice | OrderedVolume | FilledVolume |
         | Barclays     | 1       | 01/01/2018 09:30:00 |               | Limit | Sell       | GBX      | 108           |                  | 100           |              |
         | Barclays     | 2       | 01/01/2018 09:30:00 |               | Limit | Buy       | GBX      | 90           |                  | 100           |              |
		And With the intraday market data :
		| SecurityName | Epoch				 | Bid	  | Ask | Price | Currency | Volume |
		| Barclays     | 01/01/2018  10:30:00| 1	  | 20  | 100    | GBX      | 5000  |
		| Barclays     | 01/01/2018  10:29:00| 1	  | 20  | 99    | GBX      | 5000  |
		| Barclays     | 01/01/2018  10:28:00| 1	  | 20  | 98    | GBX      | 5000  |
		| Barclays     | 01/01/2018  10:27:00| 1	  | 20  | 97    | GBX      | 5000  |
		| Barclays     | 01/01/2018  10:26:00| 1	  | 20  | 97    | GBX      | 5000  |
		| Barclays     | 01/01/2018  10:25:00| 1	  | 20  | 98    | GBX      | 5000  |
		| Barclays     | 01/01/2018  10:24:00| 1	  | 20  | 99    | GBX      | 5000  |
		| Barclays     | 01/01/2018  10:23:00| 1	  | 20  | 100    | GBX      | 5000  |
		| Barclays     | 01/01/2018  10:22:00| 1	  | 20  | 100    | GBX      | 5000  |
		| Barclays     | 01/01/2018  10:21:00| 1	  | 20  | 99    | GBX      | 5000  |
		| Barclays     | 01/01/2018  10:20:00| 1	  | 20  | 98    | GBX      | 5000  |
		| Barclays     | 01/01/2018  10:19:00| 1	  | 20  | 97    | GBX      | 5000  |
		 When I run the placing orders rule
		 Then I will have 1 placing orders alerts	 

