@fixedincomewashtrade
@fixedincomewashtradeaveragenetting
@fixedincomewashtradeaveragenettingnonsensitive
Feature: Fixed Income Wash Trade Average Netting Non Sensitive
	In order to meet MAR compliance requirements
	I need to be able to detect when traders are executing trades
	Which net out to near zero net gain whilst meeting
	A given threshold of volume

Background:
	Given I have the wash trade rule parameter values
	| WindowHours | ClusteringPositionMinimumNumberOfTrades | ClusteringPercentageValueDifferenceThreshold | UseClustering |
	| 1           | 2                                       | 0.10                                         | true          |


Scenario: Empty Universe yields no alerts
	Given I have the orders for a universe from 01/01/2018 to 03/01/2018 :
	| SecurityName | OrderId | PlacedDate          | BookedDate | AmendedDate | RejectedDate | CancelledDate | FilledDate          | Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
	When I run the wash trade rule
	Then I will have 0 wash trade alerts


	Scenario:  One Trade For UKGovtBond yields no alerts
	Given I have the orders for a universe from 01/01/2018 to 03/01/2018 :
	| SecurityName | OrderId | PlacedDate          | FilledDate          | Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
	| UKGovtBond   | 0       | 01/01/2018 09:30:00 | 01/01/2018 09:30:00 | MARKET | BUY       | GBX      |            | 100              | 1000          | 1000         |
	When I run the wash trade rule
	Then I will have 0 wash trade alerts


Scenario: Two Trades In Wash Trade For Different Securities yields one alert
	Given I have the orders for a universe from 01/01/2018 to 03/01/2018 :
	| SecurityName | OrderId | PlacedDate          | BookedDate | AmendedDate | RejectedDate | CancelledDate | FilledDate          | Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
	| UKGovtBond   | 0       | 01/01/2018 09:30:00 |            |             |              |               | 01/01/2018 09:30:00 | MARKET | BUY       | GBX      |            | 100              | 1000          | 1000         |
	| UKGovtBond   | 1       | 01/01/2018 09:30:00 |            |             |              |               | 01/01/2018 09:30:00 | MARKET | SELL      | GBX      |            | 100              | 1000          | 1000         |
	When I run the wash trade rule
	Then I will have 1 wash trade alerts

	
Scenario: Three Trades at same price point In Wash Trade yields no alerts
	Given I have the orders for a universe from 01/01/2018 to 03/01/2018 :
	| SecurityName | OrderId | PlacedDate          | BookedDate | AmendedDate | RejectedDate | CancelledDate | FilledDate          | Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
	| UKGovtBond   | 0       | 01/01/2018 09:30:00 |            |             |              |               | 01/01/2018 09:30:00 | MARKET | BUY       | GBX      |            | 100              | 1000          | 1000         |
	| UKGovtBond   | 1       | 01/01/2018 09:30:00 |            |             |              |               | 01/01/2018 09:30:00 | MARKET | BUY       | GBX      |            | 100              | 1000          | 1000         |
	| UKGovtBond   | 2       | 01/01/2018 09:30:00 |            |             |              |               | 01/01/2018 09:30:00 | MARKET | SELL      | GBX      |            | 100              | 1000          | 1000         |    
	When I run the wash trade rule
	Then I will have 0 wash trade alerts

	
@justbuy
Scenario: Five Trade For UKGovtBond yields zero alerts when just buys
	Given I have the orders for a universe from 01/01/2018 to 03/01/2018 :
	| SecurityName | OrderId | PlacedDate          | BookedDate | AmendedDate | RejectedDate | CancelledDate | FilledDate          | Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
	| UKGovtBond   | 0       | 01/01/2018 09:33:00 |            |             |              |               | 01/01/2018 09:33:00 | MARKET | BUY       | GBX      |            | 1000000          | 1000000       | 1000000      |
	| UKGovtBond   | 1       | 01/01/2018 09:33:00 |            |             |              |               | 01/01/2018 09:33:00 | MARKET | BUY       | GBX      |            | 1000000          | 1000000       | 1000000      |
	| UKGovtBond   | 2       | 01/01/2018 09:33:00 |            |             |              |               | 01/01/2018 09:33:00 | MARKET | BUY       | GBX      |            | 1000000          | 1000000       | 1000000      |
	| UKGovtBond   | 3       | 01/01/2018 09:33:00 |            |             |              |               | 01/01/2018 09:33:00 | MARKET | BUY       | GBX      |            | 1000000          | 1000000       | 1000000      |
	| UKGovtBond   | 4       | 01/01/2018 09:33:00 |            |             |              |               | 01/01/2018 09:33:00 | MARKET | BUY       | GBX      |            | 1000000          | 1000000       | 1000000      |
	When I run the wash trade rule
	Then I will have 0 wash trade alerts

@timewindow
Scenario: Two Trade For UKGovtBond yields one alerts when within 1 hour
	Given I have the orders for a universe from 01/01/2018 to 03/01/2018 :
	| SecurityName | OrderId | PlacedDate          | BookedDate | AmendedDate | RejectedDate | CancelledDate | FilledDate          | Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
	| UKGovtBond   | 0       | 01/01/2018 09:30:00 |            |             |              |               | 01/01/2018 09:30:00 | MARKET | BUY       | GBX      |            | 100              | 1000          | 1000         |
	| UKGovtBond   | 1       | 01/01/2018 10:00:00 |            |             |              |               | 01/01/2018 10:00:00 | MARKET | SELL      | GBX      |            | 100              | 1000          | 1000         |   
	When I run the wash trade rule
	Then I will have 1 wash trade alerts

@percentagevaluedifference
Scenario: Two trades when inside percentage value difference threshold yields one alerts
	Given I have the orders for a universe from 01/01/2018 to 03/01/2018 :
	| SecurityName | OrderId | PlacedDate          | BookedDate | AmendedDate | RejectedDate | CancelledDate | FilledDate          | Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
	| UKGovtBond   | 0       | 01/01/2018 09:33:00 |            |             |              |               | 01/01/2018 09:33:00 | MARKET | BUY       | GBX      |            | 100              | 1000          | 1000         |
	| UKGovtBond   | 1       | 01/01/2018 09:33:00 |            |             |              |               | 01/01/2018 09:33:00 | MARKET | SELL      | GBX      |            | 109              | 1000          | 1000         | 
	When I run the wash trade rule
	Then I will have 1 wash trade alerts


@justsell
Scenario: Five Trade For UKGovtBond yields zero alerts when just sells
	Given I have the orders for a universe from 01/01/2018 to 03/01/2018 :
	| SecurityName | OrderId | PlacedDate          | BookedDate | AmendedDate | RejectedDate | CancelledDate | FilledDate          | Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
	| UKGovtBond   | 0       | 01/01/2018 09:33:00 |            |             |              |               | 01/01/2018 09:33:00 | MARKET | SELL      | GBX      |            | 1000000          | 1000000       | 1000000      |
	| UKGovtBond   | 1       | 01/01/2018 09:33:00 |            |             |              |               | 01/01/2018 09:33:00 | MARKET | SELL      | GBX      |            | 1000000          | 1000000       | 1000000      |
	| UKGovtBond   | 2       | 01/01/2018 09:33:00 |            |             |              |               | 01/01/2018 09:33:00 | MARKET | SELL      | GBX      |            | 1000000          | 1000000       | 1000000      |
	| UKGovtBond   | 3       | 01/01/2018 09:33:00 |            |             |              |               | 01/01/2018 09:33:00 | MARKET | SELL      | GBX      |            | 1000000          | 1000000       | 1000000      |
	| UKGovtBond   | 4       | 01/01/2018 09:33:00 |            |             |              |               | 01/01/2018 09:33:00 | MARKET | SELL      | GBX      |            | 1000000          | 1000000       | 1000000      |
	When I run the wash trade rule
	Then I will have 0 wash trade alerts



@washtradelosses
Scenario: Four Trade For UKGovtBond yields zero alerts with losses due to clustering at price point
	Given I have the orders for a universe from 01/01/2018 to 03/01/2018 :
	| SecurityName | OrderId | PlacedDate          | BookedDate | AmendedDate | RejectedDate | CancelledDate | FilledDate			| Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
	| UKGovtBond     | 0		| 01/01/2018 09:30:00 |            |             |              |               | 01/01/2018 09:30:00	| MARKET | BUY       | GBX      |            | 1000              | 1000          | 1000         |     
	| UKGovtBond     | 1		| 01/01/2018 09:30:00 |            |             |              |               | 01/01/2018 09:30:00	| MARKET | BUY       | GBX      |            | 1000              | 1000          | 1000         |     
	| UKGovtBond     | 2		| 01/01/2018 09:30:00 |            |             |              |               | 01/01/2018 09:30:00	| MARKET | SELL       | GBX      |            | 999              | 1000          | 1000         |     
	| UKGovtBond     | 3		| 01/01/2018 09:30:00 |            |             |              |               | 01/01/2018 09:30:00	| MARKET | SELL       | GBX      |            | 999              | 1000          | 1000         |     
	When I run the wash trade rule
	Then I will have 0 wash trade alerts
	

@washtradepartialfill
Scenario: Two Trade For UKGovtBond with partial fills yields one alerts
	Given I have the orders for a universe from 01/01/2018 to 03/01/2018 :
	| SecurityName | OrderId | PlacedDate          | BookedDate | AmendedDate | RejectedDate | CancelledDate | FilledDate			| Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
	| UKGovtBond     | 0		| 01/01/2018 09:30:00 |            |             |              |               | 01/01/2018 09:30:00	| MARKET | BUY       | GBX      |            | 100              | 1000          | 150         |     
	| UKGovtBond     | 1		| 01/01/2018 10:00:00 |            |             |              |               | 01/01/2018 10:00:00	| MARKET | SELL       | GBX      |            | 100              | 1000          | 150         |     
	When I run the wash trade rule
	Then I will have 1 wash trade alerts



@washtradeearlyorder
Scenario: Two Trade For UKGovtBond with pre market order times yields one alerts
	Given I have the orders for a universe from 01/01/2018 to 02/01/2018 :
	| SecurityName | OrderId | PlacedDate          | BookedDate | AmendedDate | RejectedDate | CancelledDate | FilledDate			| Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
	| UKGovtBond     | 0		| 01/01/2018 06:30:00 |            |             |              |               | 01/01/2018 06:30:00	| MARKET | BUY       | GBX      |            | 100              | 1000          | 150         |     
	| UKGovtBond     | 1		| 01/01/2018 06:30:00 |            |             |              |               | 01/01/2018 06:30:00	| MARKET | SELL       | GBX      |            | 100              | 1000          | 150         |     
	When I run the wash trade rule
	Then I will have 1 wash trade alerts

@washtradelateorder
Scenario: Two Trade For UKGovtBond with post market order times yields one alerts
	Given I have the orders for a universe from 01/01/2018 to 02/01/2018 :
	| SecurityName | OrderId | PlacedDate          | BookedDate | AmendedDate | RejectedDate | CancelledDate | FilledDate			| Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
	| UKGovtBond     | 0		| 01/01/2018 20:30:00 |            |             |              |               | 01/01/2018 20:30:00	| MARKET | BUY       | GBX      |            | 100              | 1000          | 150         |     
	| UKGovtBond     | 1		| 01/01/2018 20:30:00 |            |             |              |               | 01/01/2018 20:30:00	| MARKET | SELL       | GBX      |            | 100              | 1000          | 150         |     
	When I run the wash trade rule
	Then I will have 1 wash trade alerts

	
@washtradenextdaysell
Scenario: Two Trade For UKGovtBond with next day sell within window yields one alert
	Given I have the orders for a universe from 01/01/2018 to 03/01/2018 :
	| SecurityName | OrderId | PlacedDate          | BookedDate | AmendedDate | RejectedDate | CancelledDate | FilledDate			| Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
	| UKGovtBond     | 0		| 01/01/2018 12:00:00 |            |             |              |               | 01/01/2018 12:00:00	| MARKET | BUY       | GBX      |            | 100              | 1000          | 150         |     
	| UKGovtBond     | 1		| 01/02/2018 12:00:00 |            |             |              |               | 01/02/2018 12:00:00	| MARKET | SELL       | GBX      |            | 100              | 1000          | 150         |     
	And I have the wash trade rule parameter values
	| WindowHours | ClusteringPositionMinimumNumberOfTrades | ClusteringPercentageValueDifferenceThreshold | UseClustering |
	| 24           | 2                                       | 0.10                                         | true          |
	When I run the wash trade rule
	Then I will have 1 wash trade alerts


@washtradenextdaysell
Scenario: Two Trade For UKGovtBond with next day sell outside window yields zero alerts
	Given I have the orders for a universe from 01/01/2018 to 02/01/2018 :
	| SecurityName | OrderId | PlacedDate          | BookedDate | AmendedDate | RejectedDate | CancelledDate | FilledDate			| Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
	| UKGovtBond     | 0		| 01/01/2018 12:00:00 |            |             |              |               | 01/01/2018 12:00:00	| MARKET | BUY       | GBX      |            | 100              | 1000          | 150         |     
	| UKGovtBond     | 1		| 01/02/2018 12:00:00 |            |             |              |               | 01/02/2018 12:00:00	| MARKET | SELL       | GBX      |            | 100              | 1000          | 150         |     
	And I have the wash trade rule parameter values
	| WindowHours | ClusteringPositionMinimumNumberOfTrades | ClusteringPercentageValueDifferenceThreshold | UseClustering |
	| 22           | 2                                       | 0.10                                         | true          |
	When I run the wash trade rule
	Then I will have 0 wash trade alerts
	   	 

@washtradecurrencies
Scenario: Two Trade For UKGovtBond in USD converts to GBX for absolute currency breach
	Given I have the orders for a universe from 01/01/2018 to 02/01/2018 :
	| SecurityName | OrderId | PlacedDate          | BookedDate | AmendedDate | RejectedDate | CancelledDate | FilledDate			| Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
	| UKGovtBond     | 0		| 01/01/2018 06:30:00 |            |             |              |               | 01/01/2018 06:30:00	| MARKET | BUY       | USD      |            | 10000000              | 1000          | 150         |     
	| UKGovtBond     | 1		| 01/01/2018 06:30:00 |            |             |              |               | 01/01/2018 06:30:00	| MARKET | SELL       | USD      |            | 10000000              | 1000          | 149         |     
	When I run the wash trade rule
	Then I will have 1 wash trade alerts