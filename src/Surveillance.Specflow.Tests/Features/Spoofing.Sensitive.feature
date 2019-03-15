@spoofing
@spoofingsensitive
Feature: Spoofing Sensitive Parameters
	In order to meet MAR compliance requirements
	I need to be able to detect when traders are misleading markets
	And then capitalising on their market impact by trading differently

Background: 
	Given I have the spoofing rule parameter values
	| WindowHours | CancellationThreshold | RelativeSizeMultipleForSpoofExceedingReal |
	| 1           | 0.1                   | 0                                         |

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
         | Barclays     | 1       | 01/01/2019 | 01/01/2019    |            | Market | Buy       | GBX      |            |                  | 100           |  100         |
         | Barclays     | 1       | 01/01/2019 |               | 01/01/2019 |Market | Sell       | GBX      |            |                  | 100           |  100         |
         When I run the spoofing rule
		 Then I will have 1 spoofing alerts

Scenario: Ten order universe with cancellations yields one alerts
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

