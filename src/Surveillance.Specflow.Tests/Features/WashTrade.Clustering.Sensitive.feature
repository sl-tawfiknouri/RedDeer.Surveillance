@washtrade
@washtradeclustering
@washtradesensitive
Feature: WashTrade Clustering Sensitive Parameters
	In order to meet MAR compliance requirements
	I need to be able to detect when traders are executing trades
	with no meaningful change of ownership
	By clustering their trades for average value change being below
	threshold parameters

Background:
	Given I have the wash trade rule parameter values
	| WindowHours | ClusteringPositionMinimumNumberOfTrades | ClusteringPercentageValueDifferenceThreshold | UseClustering |
	| 1           | 2                                       | 0.03                                         | true          |


Scenario: Empty Universe yields no alerts
	Given I have the orders for a universe from 01/01/2018 to 03/01/2018 :
	| SecurityName | OrderId | PlacedDate          | BookedDate | AmendedDate | RejectedDate | CancelledDate | FilledDate          | Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
	When I run the wash trade rule
	Then I will have 0 wash trade alerts


Scenario: One Trade For Vodafone yields no alerts
	Given I have the orders for a universe from 01/01/2018 to 03/01/2018 :
	| SecurityName | OrderId | PlacedDate          | BookedDate | AmendedDate | RejectedDate | CancelledDate | FilledDate          | Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
	| Vodafone     | 0       | 01/01/2018 09:30:00 |            |             |              |               | 01/01/2018 09:30:00 | MARKET | BUY       | GBX      |            | 100              | 1000          | 1000         |     
	When I run the wash trade rule
	Then I will have 0 wash trade alerts


Scenario: One Trade For Barclays yields no alerts
	Given I have the orders for a universe from 01/01/2018 to 03/01/2018 :
	| SecurityName | OrderId | PlacedDate          | BookedDate | AmendedDate | RejectedDate | CancelledDate | FilledDate          | Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
	| Barclays     | 0       | 01/01/2018 09:30:00 |            |             |              |               | 01/01/2018 09:30:00 | MARKET | BUY       | GBX      |            | 100              | 1000          | 1000         |     
	When I run the wash trade rule
	Then I will have 0 wash trade alerts


Scenario: Two Trades In Wash Trade For Different Securities yields zero alerts
	Given I have the orders for a universe from 01/01/2018 to 03/01/2018 :
	| SecurityName | OrderId | PlacedDate          | BookedDate | AmendedDate | RejectedDate | CancelledDate | FilledDate          | Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
	| Vodafone     | 0       | 01/01/2018 09:30:00 |            |             |              |               | 01/01/2018 09:30:00 | MARKET | BUY       | GBX      |            | 100              | 1000          | 1000         |     
	| Barclays     | 1       | 01/01/2018 09:30:00 |            |             |              |               | 01/01/2018 09:30:00 | MARKET | SELL      | GBX      |            | 100              | 1000          | 1000         |     
	When I run the wash trade rule
	Then I will have 0 wash trade alerts


Scenario: Three Trades at same price point In Wash Trade yields no alerts
	Given I have the orders for a universe from 01/01/2018 to 03/01/2018 :
	| SecurityName | OrderId | PlacedDate          | BookedDate | AmendedDate | RejectedDate | CancelledDate | FilledDate          | Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
	| Vodafone     | 0       | 01/01/2018 09:30:00 |            |             |              |               | 01/01/2018 09:30:00 | MARKET | BUY       | GBX      |            | 100              | 1000          | 1000         |     
	| Vodafone     | 1       | 01/01/2018 09:30:00 |            |             |              |               | 01/01/2018 09:30:00 | MARKET | BUY       | GBX      |            | 100              | 1000          | 1000         |     
	| Vodafone     | 2       | 01/01/2018 09:30:00 |            |             |              |               | 01/01/2018 09:30:00 | MARKET | SELL      | GBX      |            | 100              | 1000          | 1000         |     
	When I run the wash trade rule
	Then I will have 0 wash trade alerts



@MinimumNumberOfTrades
Scenario: Two trades when min number of trades threshold set to four yields zero alerts
	Given I have the orders for a universe from 01/01/2018 to 03/01/2018 :
	| SecurityName | OrderId | PlacedDate          | BookedDate | AmendedDate | RejectedDate | CancelledDate | FilledDate          | Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
	| Barclays     | 0       | 01/01/2018 09:33:00 |            |             |              |               | 01/01/2018 09:33:00 | MARKET | BUY       | GBX      |            | 100              | 1000          | 1000         |     
	| Barclays     | 1       | 01/01/2018 10:33:00 |            |             |              |               | 01/01/2018 10:33:00 | MARKET | SELL       | GBX      |            | 100              | 1000          | 1000         |     
	And I have the wash trade rule parameter values
	| WindowHours | ClusteringPositionMinimumNumberOfTrades | ClusteringPercentageValueDifferenceThreshold | UseClustering |
	| 1           | 4                                       | 0.03                                         | true          |
	When I run the wash trade rule
	Then I will have 0 wash trade alerts


@MinimumNumberOfTrades
Scenario: Four Trade For Barclays when min number of trades threshold set to four yields one alerts
	Given I have the orders for a universe from 01/01/2018 to 03/01/2018 :
	| SecurityName | OrderId | PlacedDate          | BookedDate | AmendedDate | RejectedDate | CancelledDate | FilledDate          | Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
	| Barclays     | 0       | 01/01/2018 09:33:00 |            |             |              |               | 01/01/2018 09:33:00 | MARKET | BUY       | GBX      |            | 100              | 1000          | 1000         |     
	| Barclays     | 1       | 01/01/2018 09:33:00 |            |             |              |               | 01/01/2018 09:33:00 | MARKET | BUY       | GBX      |            | 100              | 1000          | 1000         |     
	| Barclays     | 2       | 01/01/2018 09:33:00 |            |             |              |               | 01/01/2018 09:33:00 | MARKET | SELL       | GBX      |            | 100              | 1000          | 1000         |     
	| Barclays     | 3       | 01/01/2018 09:33:00 |            |             |              |               | 01/01/2018 09:33:00 | MARKET | SELL       | GBX      |            | 100              | 1000          | 1000         |     
	And I have the wash trade rule parameter values
	| WindowHours | ClusteringPositionMinimumNumberOfTrades | ClusteringPercentageValueDifferenceThreshold | UseClustering |
	| 1           | 4                                       | 0.03                                         | true          |
	When I run the wash trade rule
	Then I will have 1 wash trade alerts


@MinimumNumberOfTrades
Scenario: Ten Trade For Barclays when min number of trades threshold set to four yields one alerts
	Given I have the orders for a universe from 01/01/2018 to 03/01/2018 :
	| SecurityName | OrderId | PlacedDate          | BookedDate | AmendedDate | RejectedDate | CancelledDate | FilledDate          | Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
	| Barclays     | 0       | 01/01/2018 09:33:00 |            |             |              |               | 01/01/2018 09:33:00 | MARKET | BUY       | GBX      |            | 100              | 1000          | 1000         |     
	| Barclays     | 1       | 01/01/2018 09:33:00 |            |             |              |               | 01/01/2018 09:33:00 | MARKET | BUY       | GBX      |            | 100              | 1000          | 1000         |     
	| Barclays     | 2       | 01/01/2018 09:33:00 |            |             |              |               | 01/01/2018 09:33:00 | MARKET | BUY       | GBX      |            | 100              | 1000          | 1000         |     
	| Barclays     | 3       | 01/01/2018 09:33:00 |            |             |              |               | 01/01/2018 09:33:00 | MARKET | BUY       | GBX      |            | 100              | 1000          | 1000         |     
	| Barclays     | 4       | 01/01/2018 09:33:00 |            |             |              |               | 01/01/2018 09:33:00 | MARKET | BUY       | GBX      |            | 100              | 1000          | 1000         |     
	| Barclays     | 5       | 01/01/2018 09:33:00 |            |             |              |               | 01/01/2018 09:33:00 | MARKET | SELL       | GBX      |            | 100              | 1000          | 1000         |     
	| Barclays     | 6       | 01/01/2018 09:33:00 |            |             |              |               | 01/01/2018 09:33:00 | MARKET | SELL       | GBX      |            | 100              | 1000          | 1000         |     
	| Barclays     | 7       | 01/01/2018 09:33:00 |            |             |              |               | 01/01/2018 09:33:00 | MARKET | SELL       | GBX      |            | 100              | 1000          | 1000         |     
	| Barclays     | 8       | 01/01/2018 09:33:00 |            |             |              |               | 01/01/2018 09:33:00 | MARKET | SELL       | GBX      |            | 100              | 1000          | 1000         |     
	| Barclays     | 9       | 01/01/2018 09:33:00 |            |             |              |               | 01/01/2018 09:33:00 | MARKET | SELL       | GBX      |            | 100              | 1000          | 1000         |     
	And I have the wash trade rule parameter values
	| WindowHours | ClusteringPositionMinimumNumberOfTrades | ClusteringPercentageValueDifferenceThreshold | UseClustering |
	| 1           | 4                                       | 0.03                                         | true          |
	When I run the wash trade rule
	Then I will have 1 wash trade alerts


@percentagevaluedifference
Scenario: Two trades when inside percentage value difference threshold yields one alerts
	Given I have the orders for a universe from 01/01/2018 to 03/01/2018 :
	| SecurityName | OrderId | PlacedDate          | BookedDate | AmendedDate | RejectedDate | CancelledDate | FilledDate          | Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
	| Barclays     | 0       | 01/01/2018 09:33:00 |            |             |              |               | 01/01/2018 09:33:00 | MARKET | BUY       | GBX      |            | 100              | 1000          | 1000         |     
	| Barclays     | 1       | 01/01/2018 09:33:00 |            |             |              |               | 01/01/2018 09:33:00 | MARKET | SELL       | GBX      |            | 101              | 1000          | 1000         |     
	When I run the wash trade rule
	Then I will have 1 wash trade alerts


@percentagevaluedifference
Scenario: Two trades when exactly on percentage value difference threshold yields one alerts
	Given I have the orders for a universe from 01/01/2018 to 03/01/2018 :
	| SecurityName | OrderId | PlacedDate          | BookedDate | AmendedDate | RejectedDate | CancelledDate | FilledDate          | Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
	| Barclays     | 0       | 01/01/2018 09:33:00 |            |             |              |               | 01/01/2018 09:33:00 | MARKET | BUY       | GBX      |            | 100              | 1000          | 1000         |     
	| Barclays     | 1       | 01/01/2018 09:33:00 |            |             |              |               | 01/01/2018 09:33:00 | MARKET | SELL       | GBX      |            | 103              | 1000          | 1000         |     
	When I run the wash trade rule
	Then I will have 1 wash trade alerts


@percentagevaluedifference
Scenario: Two trades when outside percentage value difference threshold yields zero alerts
	Given I have the orders for a universe from 01/01/2018 to 03/01/2018 :
	| SecurityName | OrderId | PlacedDate          | BookedDate | AmendedDate | RejectedDate | CancelledDate | FilledDate          | Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
	| Barclays     | 0       | 01/01/2018 09:33:00 |            |             |              |               | 01/01/2018 09:33:00 | MARKET | BUY       | GBX      |            | 100              | 1000          | 1000         |     
	| Barclays     | 1       | 01/01/2018 09:33:00 |            |             |              |               | 01/01/2018 09:33:00 | MARKET | SELL       | GBX      |            | 104              | 1000          | 1000         |     
	When I run the wash trade rule
	Then I will have 0 wash trade alerts




@justbuy
Scenario: Five Trade For Barclays yields zero alerts when just buys
	Given I have the orders for a universe from 01/01/2018 to 03/01/2018 :
	| SecurityName | OrderId | PlacedDate          | BookedDate | AmendedDate | RejectedDate | CancelledDate | FilledDate          | Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
	| Barclays     | 0       | 01/01/2018 09:33:00 |            |             |              |               | 01/01/2018 09:33:00 | MARKET | BUY       | GBX      |            | 1000000              | 1000000         | 1000000       |     
	| Barclays     | 1       | 01/01/2018 09:33:00 |            |             |              |               | 01/01/2018 09:33:00 | MARKET | BUY      | GBX      |            | 1000000              | 1000000          | 1000000        |     
   	| Barclays     | 2       | 01/01/2018 09:33:00 |            |             |              |               | 01/01/2018 09:33:00 | MARKET | BUY      | GBX      |            | 1000000              | 1000000          | 1000000        |   
	| Barclays     | 3       | 01/01/2018 09:33:00 |            |             |              |               | 01/01/2018 09:33:00 | MARKET | BUY      | GBX      |            | 1000000              | 1000000          | 1000000        |   
	| Barclays     | 4       | 01/01/2018 09:33:00 |            |             |              |               | 01/01/2018 09:33:00 | MARKET | BUY      | GBX      |            | 1000000              | 1000000          | 1000000        |
	When I run the wash trade rule
	Then I will have 0 wash trade alerts


@justsell
Scenario: Five Trade For Barclays yields zero alerts when just sells
	Given I have the orders for a universe from 01/01/2018 to 03/01/2018 :
	| SecurityName | OrderId | PlacedDate          | BookedDate | AmendedDate | RejectedDate | CancelledDate | FilledDate          | Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
	| Barclays     | 0       | 01/01/2018 09:33:00 |            |             |              |               | 01/01/2018 09:33:00 | MARKET | SELL       | GBX      |            | 1000000              | 1000000         | 1000000       |     
	| Barclays     | 1       | 01/01/2018 09:33:00 |            |             |              |               | 01/01/2018 09:33:00 | MARKET | SELL      | GBX      |            | 1000000              | 1000000          | 1000000        |     
   	| Barclays     | 2       | 01/01/2018 09:33:00 |            |             |              |               | 01/01/2018 09:33:00 | MARKET | SELL      | GBX      |            | 1000000              | 1000000          | 1000000        |   
	| Barclays     | 3       | 01/01/2018 09:33:00 |            |             |              |               | 01/01/2018 09:33:00 | MARKET | SELL      | GBX      |            | 1000000              | 1000000          | 1000000        |   
	| Barclays     | 4       | 01/01/2018 09:33:00 |            |             |              |               | 01/01/2018 09:33:00 | MARKET | SELL      | GBX      |            | 1000000              | 1000000          | 1000000        |
	When I run the wash trade rule
	Then I will have 0 wash trade alerts


@washtradelosses
Scenario: Four Trade For Nvidia yields zero alerts with losses due to clustering as two price points
	Given I have the orders for a universe from 01/01/2018 to 03/01/2018 :
	| SecurityName | OrderId | PlacedDate          | BookedDate | AmendedDate | RejectedDate | CancelledDate | FilledDate			| Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
	| Nvidia     | 0		| 01/01/2018 09:30:00 |            |             |              |               | 01/01/2018 09:30:00	| MARKET | BUY       | GBX      |            | 100              | 1000          | 1000         |     
	| Nvidia     | 1		| 01/01/2018 09:30:00 |            |             |              |               | 01/01/2018 09:30:00	| MARKET | BUY       | GBX      |            | 100              | 1000          | 1000         |     
	| Nvidia     | 2		| 01/01/2018 09:30:00 |            |             |              |               | 01/01/2018 09:30:00	| MARKET | SELL       | GBX      |            | 99              | 1000          | 1000         |     
	| Nvidia     | 3		| 01/01/2018 09:30:00 |            |             |              |               | 01/01/2018 09:30:00	| MARKET | SELL       | GBX      |            | 99              | 1000          | 1000         |     
	When I run the wash trade rule
	Then I will have 0 wash trade alerts


@washtradepartialfill
Scenario: Two Trade For Nvidia with partial fills yields one alerts
	Given I have the orders for a universe from 01/01/2018 to 03/01/2018 :
	| SecurityName | OrderId | PlacedDate          | BookedDate | AmendedDate | RejectedDate | CancelledDate | FilledDate			| Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
	| Nvidia     | 0		| 01/01/2018 09:30:00 |            |             |              |               | 01/01/2018 09:30:00	| MARKET | BUY       | GBX      |            | 100              | 1000          | 150         |     
	| Nvidia     | 1		| 01/01/2018 10:00:00 |            |             |              |               | 01/01/2018 10:00:00	| MARKET | SELL       | GBX      |            | 100              | 1000          | 150         |     
	When I run the wash trade rule
	Then I will have 1 wash trade alerts



@washtradeearlyorder
Scenario: Two Trade For Nvidia with pre market order times yields one alerts
	Given I have the orders for a universe from 01/01/2018 to 02/01/2018 :
	| SecurityName | OrderId | PlacedDate          | BookedDate | AmendedDate | RejectedDate | CancelledDate | FilledDate			| Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
	| Nvidia     | 0		| 01/01/2018 06:30:00 |            |             |              |               | 01/01/2018 06:30:00	| MARKET | BUY       | GBX      |            | 100              | 1000          | 150         |     
	| Nvidia     | 1		| 01/01/2018 06:30:00 |            |             |              |               | 01/01/2018 06:30:00	| MARKET | SELL       | GBX      |            | 100              | 1000          | 150         |     
	When I run the wash trade rule
	Then I will have 1 wash trade alerts


@washtradelateorder
Scenario: Two Trade For Nvidia with post market order times yields one alerts
	Given I have the orders for a universe from 01/01/2018 to 02/01/2018 :
	| SecurityName | OrderId | PlacedDate          | BookedDate | AmendedDate | RejectedDate | CancelledDate | FilledDate			| Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
	| Nvidia     | 0		| 01/01/2018 20:30:00 |            |             |              |               | 01/01/2018 20:30:00	| MARKET | BUY       | GBX      |            | 100              | 1000          | 150         |     
	| Nvidia     | 1		| 01/01/2018 20:30:00 |            |             |              |               | 01/01/2018 20:30:00	| MARKET | SELL       | GBX      |            | 100              | 1000          | 150         |     
	When I run the wash trade rule
	Then I will have 1 wash trade alerts


@washtradenextdaysell
Scenario: Two Trade For Nvidia with next day sell within window yields one alert
	Given I have the orders for a universe from 01/01/2018 to 03/01/2018 :
	| SecurityName | OrderId | PlacedDate          | BookedDate | AmendedDate | RejectedDate | CancelledDate | FilledDate			| Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
	| Nvidia     | 0		| 01/01/2018 12:00:00 |            |             |              |               | 01/01/2018 12:00:00	| MARKET | BUY       | GBX      |            | 100              | 1000          | 150         |     
	| Nvidia     | 1		| 01/02/2018 12:00:00 |            |             |              |               | 01/02/2018 12:00:00	| MARKET | SELL       | GBX      |            | 100              | 1000          | 150         |     
	And I have the wash trade rule parameter values
	| WindowHours | ClusteringPositionMinimumNumberOfTrades | ClusteringPercentageValueDifferenceThreshold | UseClustering |
	| 24           | 2                                       | 0.10                                         | true          |
	When I run the wash trade rule
	Then I will have 1 wash trade alerts


@washtradenextdaysell
Scenario: Two Trade For Nvidia with next day sell outside window yields zero alerts
	Given I have the orders for a universe from 01/01/2018 to 02/01/2018 :
	| SecurityName | OrderId | PlacedDate          | BookedDate | AmendedDate | RejectedDate | CancelledDate | FilledDate			| Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
	| Nvidia     | 0		| 01/01/2018 12:00:00 |            |             |              |               | 01/01/2018 12:00:00	| MARKET | BUY       | GBX      |            | 100              | 1000          | 150         |     
	| Nvidia     | 1		| 01/02/2018 12:00:00 |            |             |              |               | 01/02/2018 12:00:00	| MARKET | SELL       | GBX      |            | 100              | 1000          | 150         |     
	And I have the wash trade rule parameter values
	| WindowHours | ClusteringPositionMinimumNumberOfTrades | ClusteringPercentageValueDifferenceThreshold | UseClustering |
	| 22           | 2                                       | 0.10                                         | true          |
	When I run the wash trade rule
	Then I will have 0 wash trade alerts



@washtrademultiplealerts
Scenario: Two Trade For Nvidia and two for vodafone yields two alerts
	Given I have the orders for a universe from 01/01/2018 to 03/01/2018 :
	| SecurityName | OrderId | PlacedDate          | BookedDate | AmendedDate | RejectedDate | CancelledDate | FilledDate			| Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
	| Nvidia     | 0		| 01/01/2018 09:30:00 |            |             |              |               | 01/01/2018 09:30:00	| MARKET | BUY       | GBX      |            | 100              | 1000          | 1000         |     
	| Nvidia     | 1		| 01/01/2018 10:00:00 |            |             |              |               | 01/01/2018 10:00:00	| MARKET | SELL       | GBX      |            | 100              | 1000          | 1000         |     
	| Vodafone   | 0		| 01/01/2018 09:30:00 |            |             |              |               | 01/01/2018 09:30:00	| MARKET | BUY       | GBX      |            | 100              | 1000          | 1000         |     
	| Vodafone   | 1		| 01/01/2018 10:00:00 |            |             |              |               | 01/01/2018 10:00:00	| MARKET | SELL       | GBX      |            | 100              | 1000          | 1000         |     
	When I run the wash trade rule
	Then I will have 2 wash trade alerts

	

@washtradecurrencies
Scenario: Two Trade For Nvidia in USD converts to GBX for absolute currency breach
	Given I have the orders for a universe from 01/01/2018 to 02/01/2018 :
	| SecurityName | OrderId | PlacedDate          | BookedDate | AmendedDate | RejectedDate | CancelledDate | FilledDate			| Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
	| Nvidia     | 0		| 01/01/2018 06:30:00 |            |             |              |               | 01/01/2018 06:30:00	| MARKET | BUY       | USD      |            | 10000000              | 1000          | 150         |     
	| Nvidia     | 1		| 01/01/2018 06:30:00 |            |             |              |               | 01/01/2018 06:30:00	| MARKET | SELL       | USD      |            | 10000000              | 1000          | 149         |     
	When I run the wash trade rule
	Then I will have 1 wash trade alerts