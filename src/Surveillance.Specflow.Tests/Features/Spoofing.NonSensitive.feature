@spoofing
@spoofingnonsensitive
Feature: Spoofing Non Sensitive Parameters
	In order to meet MAR compliance requirements
	I need to be able to detect when traders are misleading markets
	And then capitalising on their market impact by trading differently

Background: 
	Given I have the spoofing rule parameter values
	| WindowHours | CancellationThreshold | RelativeSizeMultipleForSpoofExceedingReal |
	| 1           | 0.4                   | 1                                         |

Scenario: Empty Universe yields no alerts
		 Given I have the orders for a universe from 01/01/2018 to 01/05/2018 :
         | SecurityName | OrderId | PlacedDate | FilledDate | Type | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
         When I run the spoofing rule
		 Then I will have 0 spoofing alerts

Scenario: One order universe yields no alerts
		 Given I have the orders for a universe from 01/01/2018 to 01/05/2018 :
         | SecurityName | OrderId | PlacedDate | CancelledDate | Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
         | Barclays     | 1       | 01/01/2019 |               | Market | Buy       | GBX      |            |                  | 100           |              |
         When I run the spoofing rule
		 Then I will have 0 spoofing alerts

Scenario: Two order universe with cancellations yields one alerts
		 Given I have the orders for a universe from 01/01/2019 to 01/05/2019 :
         | SecurityName | OrderId | PlacedDate | CancelledDate | FilledDate | Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
         | Barclays     | 1       | 01/01/2019 | 01/01/2019    |            | Market | Buy       | GBX      |            |                  | 100           | 100          |
         | Barclays     | 1       | 01/01/2019 |               | 01/01/2019 |Market | Sell       | GBX      |            |                  | 100           |  100         |
         When I run the spoofing rule
		 Then I will have 1 spoofing alerts

Scenario: Two order universe with cancellations yields zero alerts
		 Given I have the orders for a universe from 01/01/2019 to 01/05/2019 :
         | SecurityName | OrderId | PlacedDate | CancelledDate | FilledDate | Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
         | Barclays     | 1       | 01/01/2019 | 01/01/2019    |            | Market | Buy       | GBX      |            |                  | 100           |  100         |
         | Barclays     | 1       | 01/01/2019 |               | 01/01/2019 |Market | Sell       | GBX      |            |                  | 100           |  110         |
         When I run the spoofing rule
		 Then I will have 0 spoofing alerts

Scenario: Ten order universe with cancellations yields one alerts
		 Given I have the orders for a universe from 01/01/2019 to 01/05/2019 :
         | SecurityName | OrderId | PlacedDate | CancelledDate | FilledDate | Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
         | Barclays     | 1       | 01/01/2019 | 01/01/2019    | 01/01/2019 | Market | Buy       | GBX      |            |                  | 100           | 100          |
         | Barclays     | 1       | 01/01/2019 | 01/01/2019    | 01/01/2019 | Market | Buy       | GBX      |            |                  | 100           | 100          |
         | Barclays     | 1       | 01/01/2019 | 01/01/2019    | 01/01/2019 | Market | Buy       | GBX      |            |                  | 100           | 100          |
         | Barclays     | 1       | 01/01/2019 | 01/01/2019    | 01/01/2019 | Market | Buy       | GBX      |            |                  | 100           | 100          |
         | Barclays     | 1       | 01/01/2019 | 01/01/2019    | 01/01/2019 | Market | Buy       | GBX      |            |                  | 100           | 100          |
         | Barclays     | 1       | 01/01/2019 |               | 01/01/2019 | Market | Buy       | GBX      |            |                  | 100           | 100          |
         | Barclays     | 1       | 01/01/2019 |               | 01/01/2019 | Market | Buy       | GBX      |            |                  | 100           | 100          |
         | Barclays     | 1       | 01/01/2019 |               | 01/01/2019 | Market | Buy       | GBX      |            |                  | 100           | 100          |
         | Barclays     | 1       | 01/01/2019 |               | 01/01/2019 | Market | Buy       | GBX      |            |                  | 100           | 100          |
         | Barclays     | 1       | 01/01/2019 |               | 01/01/2019 | Market | Buy       | GBX      |            |                  | 100           | 100          |
         | Barclays     | 1       | 01/01/2019 |               | 01/01/2019 | Market | Sell      | GBX      |            |                  | 100           |  100         |
         When I run the spoofing rule
		 Then I will have 1 spoofing alerts

Scenario: Eleven order universe with cancellations yields zero alerts
		 Given I have the orders for a universe from 01/01/2019 to 01/05/2019 :
         | SecurityName | OrderId | PlacedDate | CancelledDate | FilledDate | Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
         | Barclays     | 1       | 01/01/2019 | 01/01/2019    | 01/01/2019 | Market | Buy       | GBX      |            |                  | 100           | 100          |
         | Barclays     | 1       | 01/01/2019 |               | 01/01/2019 | Market | Buy       | GBX      |            |                  | 100           | 100          |
         | Barclays     | 1       | 01/01/2019 |               | 01/01/2019 | Market | Buy       | GBX      |            |                  | 100           | 100          |
         | Barclays     | 1       | 01/01/2019 |               | 01/01/2019 | Market | Buy       | GBX      |            |                  | 100           | 100          |
         | Barclays     | 1       | 01/01/2019 |               | 01/01/2019 | Market | Buy       | GBX      |            |                  | 100           | 100          |
         | Barclays     | 1       | 01/01/2019 |               | 01/01/2019 | Market | Buy       | GBX      |            |                  | 100           | 100          |
         | Barclays     | 1       | 01/01/2019 |               | 01/01/2019 | Market | Buy       | GBX      |            |                  | 100           | 100          |
         | Barclays     | 1       | 01/01/2019 |               | 01/01/2019 | Market | Buy       | GBX      |            |                  | 100           | 100          |
         | Barclays     | 1       | 01/01/2019 |               | 01/01/2019 | Market | Buy       | GBX      |            |                  | 100           | 100          |
         | Barclays     | 1       | 01/01/2019 |               | 01/01/2019 | Market | Buy       | GBX      |            |                  | 100           | 100          |
         | Barclays     | 1       | 01/01/2019 |               | 01/01/2019 | Market | Buy       | GBX      |            |                  | 100           | 100          |
         | Barclays     | 1       | 01/01/2019 |               | 01/01/2019 | Market | Sell      | GBX      |            |                  | 100           |  100         |
         When I run the spoofing rule
		 Then I will have 0 spoofing alerts

Scenario: Out of Time window yields No alerts
		 Given I have the orders for a universe from 01/01/2019 to 01/05/2019 :
         | SecurityName | OrderId | PlacedDate | CancelledDate | FilledDate | Type   | Direction | Currency | OrderedVolume | FilledVolume |
         | Barclays     | 1       | 01/01/2019 |               | 01/01/2019 | Market | Buy       | GBX      | 100           | 100          |
         | Barclays     | 1       | 02/01/2019 | 02/01/2019    |            | Market | Sell      | GBX      | 100           | 100          |
         When I run the spoofing rule
		 Then I will have 0 spoofing alerts

Scenario: Inside Time window yields One alerts
		 Given I have the orders for a universe from 01/01/2019 to 01/05/2019 :
         | SecurityName | OrderId | PlacedDate | CancelledDate | FilledDate | Type   | Direction | Currency | OrderedVolume | FilledVolume |
         | Barclays     | 1       | 01/01/2019 | 01/01/2019    |            | Market | Buy       | GBX      | 100           | 200          |
         | Barclays     | 1       | 01/01/2019 |               | 01/01/2019 | Market | Sell      | GBX      | 100           | 100          |
         When I run the spoofing rule
		 Then I will have 1 spoofing alerts

Scenario: Outside Cancellation Threshold yields No alerts
		 Given I have the orders for a universe from 01/01/2019 to 01/05/2019 :
         | SecurityName | OrderId | PlacedDate | CancelledDate | FilledDate | Type   | Direction | Currency | OrderedVolume | FilledVolume |
         | Barclays     | 1       | 01/01/2019 |               | 01/01/2019 | Market | Buy       | GBX      | 100           | 10           |
         | Barclays     | 1       | 01/01/2019 | 01/01/2019    |            | Market | Sell      | GBX      | 100           | 100          |
         When I run the spoofing rule
		 Then I will have 0 spoofing alerts

Scenario: Inside Cancellation Threshold yields one alerts
		 Given I have the orders for a universe from 01/01/2019 to 01/05/2019 :
         | SecurityName | OrderId | PlacedDate | CancelledDate | FilledDate | Type   | Direction | Currency | OrderedVolume | FilledVolume |
         | Barclays     | 1       | 01/01/2019 | 01/01/2019    |            | Market | Buy       | GBX      | 100           | 100          |
         | Barclays     | 1       | 01/01/2019 |               | 01/01/2019 |Market | Sell       | GBX      | 100           |  100         |
         When I run the spoofing rule
		 Then I will have 1 spoofing alerts

Scenario: Outside RelativeSizeMultipleForSpoofExceedingReal yields No alerts
		 Given I have the orders for a universe from 01/01/2019 to 01/05/2019 :
         | SecurityName | OrderId | PlacedDate | CancelledDate | FilledDate | Type   | Direction | Currency | OrderedVolume | FilledVolume |
         | Barclays     | 1       | 01/01/2019 | 01/01/2019    |            | Market | Buy       | GBX      | 100           | 1            |
         | Barclays     | 1       | 01/01/2019 |               | 01/01/2019 | Market | Sell      | GBX      | 100           | 100          |
         When I run the spoofing rule
		 Then I will have 0 spoofing alerts

Scenario: Inside RelativeSizeMultipleForSpoofExceedingReal yields One alerts
		 Given I have the orders for a universe from 01/01/2019 to 01/05/2019 :
         | SecurityName | OrderId | PlacedDate | CancelledDate | FilledDate | Type   | Direction | Currency | OrderedVolume | FilledVolume |
         | Barclays     | 1       | 01/01/2019 | 01/01/2019    |            | Market | Buy       | GBX      | 100           | 200          |
         | Barclays     | 1       | 01/01/2019 |               | 01/01/2019 | Market | Sell      | GBX      | 100           | 100          |
         When I run the spoofing rule
		 Then I will have 1 spoofing alerts

Scenario: Trades with short and cover yields One alerts 
		 Given I have the orders for a universe from 01/01/2019 to 01/05/2019 :
         | SecurityName | OrderId | PlacedDate | CancelledDate | FilledDate | Type   | Direction | Currency | OrderedVolume | FilledVolume |
         | Barclays     | 1       | 01/01/2019 | 01/01/2019    |            | Market | Short     | GBX      | 100           | 200          |
         | Barclays     | 1       | 01/01/2019 |               | 01/01/2019 | Market | Cover     | GBX      | 100           | 100          |
         When I run the spoofing rule
		 Then I will have 1 spoofing alerts


Scenario: Single Cancelled Trade yields No alerts 
		 Given I have the orders for a universe from 01/01/2019 to 01/05/2019 :
         | SecurityName | OrderId | PlacedDate | CancelledDate | FilledDate | Type   | Direction | Currency | OrderedVolume | FilledVolume |
         | Barclays     | 1       | 01/01/2019 | 01/01/2019    |            | Market | Short     | GBX      | 100           | 200          |
         When I run the spoofing rule
		 Then I will have 0 spoofing alerts