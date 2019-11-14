@layering
@layeringsensitive
Feature: Layering Sensitive Parameters
	In order to meet MAR compliance requirements
	I need to be able to detect when traders are misleading markets by layering orders
	And then capitalising on their market impact by trading differently

Background: 
	Given I have the layering rule parameter values
	| WindowHours | CheckForCorrespondingPriceMovement | PercentageOfMarketDailyVolume | PercentageOfMarketWindowVolume |
	| 1           |					                   | 							   | 								|

Scenario: Empty Universe yields no alerts
		 Given I have the orders for a universe from 01/01/2018 to 01/05/2018 :
         | SecurityName | OrderId | PlacedDate | FilledDate | Type | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
         When I run the layering rule
		 Then I will have 1 layering alerts

Scenario: One order universe yields no alerts
		 Given I have the orders for a universe from 01/01/2018 to 01/05/2018 :
         | SecurityName | OrderId | PlacedDate | CancelledDate | Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
         | Barclays     | 1       | 01/01/2019 |               | Market | Buy       | GBX      |            |                  | 1000000       |              |
         When I run the layering rule
		 Then I will have 1 layering alerts

Scenario: Two order universe yields one alerts
		 Given I have the orders for a universe from 01/01/2019 to 01/05/2019 :
         | SecurityName | OrderId | PlacedDate | CancelledDate | FilledDate | Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
         | Barclays     | 1       | 01/01/2019 |			   | 01/01/2019 | Market | Buy       | GBX      |     140    | 140                 | 1000000       |  1000000     |
         | Barclays     | 2       | 01/01/2019 |               | 01/01/2019 | Market | Sell      | GBX      |     140    | 140                 | 1000000       |  1000000     |
         When I run the layering rule
		 Then I will have 2 layering alerts

Scenario: Ten order universe with bidirectional trading yields one alerts
		 Given I have the orders for a universe from 01/01/2019 to 01/05/2019 :
         | SecurityName | OrderId | PlacedDate | CancelledDate | FilledDate | Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
         | Barclays     | 1       | 01/01/2019 |			   | 01/01/2019 | Market | Buy       | GBX      |            | 100              | 1000000       | 1000000      |
         | Barclays     | 2       | 01/01/2019 |               | 01/01/2019 | Market | Buy       | GBX      |            | 100              | 1000000       | 1000000      |
         | Barclays     | 3       | 01/01/2019 |               | 01/01/2019 | Market | Buy       | GBX      |            | 100              | 1000000       | 1000000      |
         | Barclays     | 4       | 01/01/2019 |               | 01/01/2019 | Market | Buy       | GBX      |            | 100              | 1000000       | 1000000      |
         | Barclays     | 5       | 01/01/2019 |               | 01/01/2019 | Market | Buy       | GBX      |            | 100              | 1000000       | 1000000      |
         | Barclays     | 6       | 01/01/2019 |               | 01/01/2019 | Market | Sell      | GBX      |            | 100              | 1000000       | 1000000      |
         | Barclays     | 7       | 01/01/2019 |               | 01/01/2019 | Market | Sell      | GBX      |            | 100              | 1000000       | 1000000      |
         | Barclays     | 8       | 01/01/2019 |               | 01/01/2019 | Market | Sell      | GBX      |            | 100              | 1000000       | 1000000      |
         | Barclays     | 9       | 01/01/2019 |               | 01/01/2019 | Market | Sell      | GBX      |            | 100              | 1000000       | 1000000      |
         | Barclays     | 10      | 01/01/2019 |               | 01/01/2019 | Market | Sell      | GBX      |            | 100              | 1000000       | 1000000      |
         When I run the layering rule
		 Then I will have 6 layering alerts

