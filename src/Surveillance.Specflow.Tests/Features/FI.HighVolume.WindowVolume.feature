@fixedincomehighvolumewindowvolume
Feature: Fixed Income High Volume Window Volume
	In order to meet MAR compliance requirements
	I need to be able to detect when traders are executing trades
	Which have unusually high volume as a proportion
	of the percentage of window trading

Background:
			Given I have the fixed income high volume rule parameter values
			| WindowHours | FixedIncomeHighVolumePercentageDaily | FixedIncomeHighVolumePercentageWindow |
			| 1           |                                      | 0.01                                 |

@fixedincomehighvolumewindowvolume
Scenario: Empty Universe yields no alerts
	Given I have the orders for a universe from 01/01/2018 to 03/01/2018 :
	| SecurityName | OrderId | PlacedDate          | BookedDate | AmendedDate | RejectedDate | CancelledDate | FilledDate  | Type | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
	When I run the fixed income high volume rule
	Then I will have 0 fixed income high volume alerts

@fixedincomehighvolumewindowvolume
Scenario: One order at window volume yields one alert
	Given I have the orders for a universe from 01/01/2018 to 03/01/2018 :
	| SecurityName | OrderId | PlacedDate          | FilledDate          | Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
	| UKGovtBondSecondaryMarket     | 0       | 01/01/2018 09:30:00 | 01/01/2018 09:30:00 | MARKET | BUY       | GBX      |            | 10              | 100          | 100         |
	And With the intraday market data :
	| SecurityName | Epoch      | Bid | Ask | Price | Currency | Volume |
	| UKGovtBondSecondaryMarket     | 01/01/2018  09:29:00| 1	  | 20  | 10    | GBX      | 5000  |
	| UKGovtBondSecondaryMarket     | 01/01/2018  09:28:00| 1	  | 20  | 10    | GBX      | 5000  |
	When I run the fixed income high volume rule
	Then I will have 1 fixed income high volume alerts

@fixedincomehighvolumewindowvolume
Scenario: Two order one inside and one inside window but next day at window volume yields one alert
	Given I have the fixed income high volume rule parameter values
	| WindowHours | FixedIncomeHighVolumePercentageDaily | FixedIncomeHighVolumePercentageWindow |
	| 25           |                                      | 0.01                                 |
	And I have the orders for a universe from 01/01/2018 to 03/01/2018 :
	| SecurityName | OrderId | PlacedDate          | FilledDate          | Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
	| UKGovtBondSecondaryMarket     | 0       | 01/01/2018 15:30:00 | 01/01/2018 15:30:00 | MARKET | BUY       | GBX      |            | 10              | 50          | 50         |
	| UKGovtBondSecondaryMarket     | 1       | 01/02/2018 14:30:00 | 01/02/2018 14:30:00 | MARKET | BUY       | GBX      |            | 10              | 50          | 10         |
	And With the intraday market data :
	| SecurityName | Epoch      | Bid | Ask | Price | Currency | Volume |
	| UKGovtBondSecondaryMarket     | 01/01/2018  09:30:00| 1	  | 20  | 10    | GBX      | 500  |
	| UKGovtBondSecondaryMarket     | 01/02/2018  09:29:00| 1	  | 20  | 10    | GBX      | 5500  |
	When I run the fixed income high volume rule
	Then I will have 2 fixed income high volume alerts

@fixedincomehighvolumewindowvolume
Scenario: Two order one inside and one outside window and next day at window volume yields zero alert
	Given I have the fixed income high volume rule parameter values
	| WindowHours | FixedIncomeHighVolumePercentageDaily | FixedIncomeHighVolumePercentageWindow |
	| 23           |                                      | 0.01                                 |
	And I have the orders for a universe from 01/01/2018 to 03/01/2018 :
	| SecurityName | OrderId | PlacedDate          | FilledDate          | Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
	| UKGovtBondSecondaryMarket     | 0       | 01/01/2018 15:30:00 | 01/01/2018 15:30:00 | MARKET | BUY       | GBX      |            | 10              | 50          | 25         |
	| UKGovtBondSecondaryMarket     | 0       | 01/02/2018 16:30:00 | 01/02/2018 16:30:00 | MARKET | BUY       | GBX      |            | 10              | 50          | 25         |
	And With the intraday market data :
	| SecurityName | Epoch      | Bid | Ask | Price | Currency | Volume |
	| UKGovtBondSecondaryMarket     | 01/01/2018  09:30:00| 1	  | 20  | 10    | GBX      | 5000  |
	| UKGovtBondSecondaryMarket     | 01/02/2018  09:29:00| 1	  | 20  | 10    | GBX      | 5000  |
	When I run the fixed income high volume rule
	Then I will have 0 fixed income high volume alerts

@fixedincomehighvolumewindowvolume
Scenario: Two order one inside and one outside trading hours at window volume yields one alert
	Given I have the orders for a universe from 01/01/2018 to 03/01/2018 :
	| SecurityName | OrderId | PlacedDate          | FilledDate          | Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
	| UKGovtBondSecondaryMarket     | 0       | 01/01/2018 15:30:00 | 01/01/2018 15:30:00 | MARKET | BUY       | GBX      |            | 10              | 50          | 50         |
	| UKGovtBondSecondaryMarket     | 0       | 01/01/2018 16:30:00 | 01/01/2018 16:30:00 | MARKET | BUY       | GBX      |            | 10              | 50          | 50         |
	And With the intraday market data :
	| SecurityName | Epoch      | Bid | Ask | Price | Currency | Volume |
	| UKGovtBondSecondaryMarket     | 01/01/2018  09:30:00| 1	  | 20  | 10    | GBX      | 5000  |
	| UKGovtBondSecondaryMarket     | 01/01/2018  09:29:00| 1	  | 20  | 10    | GBX      | 5000  |
	When I run the fixed income high volume rule
	Then I will have 1 fixed income high volume alerts

@fixedincomehighvolumewindowvolume
Scenario: One order at window volume partially filled yields one alert
	Given I have the orders for a universe from 01/01/2018 to 03/01/2018 :
	| SecurityName | OrderId | PlacedDate          | FilledDate          | Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
	| UKGovtBondSecondaryMarket     | 0       | 01/01/2018 09:30:00 | 01/01/2018 09:30:00 | MARKET | BUY       | GBX      |            | 10              | 1000          | 100         |
	And With the intraday market data :
	| SecurityName | Epoch      | Bid | Ask | Price | Currency | Volume |
	| UKGovtBondSecondaryMarket     | 01/01/2018  09:30:00| 1	  | 20  | 10    | GBX      | 5000  |
	| UKGovtBondSecondaryMarket     | 01/01/2018  09:29:00| 1	  | 20  | 10    | GBX      | 5000  |
	When I run the fixed income high volume rule
	Then I will have 1 fixed income high volume alerts

@fixedincomehighvolumewindowvolume
Scenario: One order at low volume window volume yields one alert
	Given I have the orders for a universe from 01/01/2018 to 03/01/2018 :
	| SecurityName | OrderId | PlacedDate          | FilledDate          | Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
	| UKGovtBondSecondaryMarket     | 0       | 01/01/2018 09:30:00 | 01/01/2018 09:30:00 | MARKET | BUY       | GBX      |            | 10              | 1          | 1         |
	And With the intraday market data :
	| SecurityName | Epoch      | Bid | Ask | Price | Currency | Volume |
	| UKGovtBondSecondaryMarket     | 01/01/2018  09:30:00| 1	  | 20  | 10    | GBX      | 50  |
	| UKGovtBondSecondaryMarket     | 01/01/2018  09:29:00| 1	  | 20  | 10    | GBX      | 50  |
	When I run the fixed income high volume rule
	Then I will have 1 fixed income high volume alerts

@fixedincomehighvolumewindowvolume
Scenario: One order just buy at window volume yields one alert
	Given I have the orders for a universe from 01/01/2018 to 03/01/2018 :
	| SecurityName | OrderId | PlacedDate          | FilledDate          | Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
	| UKGovtBondSecondaryMarket     | 0       | 01/01/2018 09:30:00 | 01/01/2018 09:30:00 | MARKET | BUY       | GBX      |            | 10              | 100          | 100         |
	And With the intraday market data :
	| SecurityName | Epoch      | Bid | Ask | Price | Currency | Volume |
	| UKGovtBondSecondaryMarket     | 01/01/2018  09:30:00| 1	  | 20  | 10    | GBX      | 5000  |
	| UKGovtBondSecondaryMarket     | 01/01/2018  09:29:00| 1	  | 20  | 10    | GBX      | 5000  |
	When I run the fixed income high volume rule
	Then I will have 1 fixed income high volume alerts

@fixedincomehighvolumewindowvolume
Scenario: One order just sell at window volume yields one alert
	Given I have the orders for a universe from 01/01/2018 to 03/01/2018 :
	| SecurityName | OrderId | PlacedDate          | FilledDate          | Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
	| UKGovtBondSecondaryMarket     | 0       | 01/01/2018 09:30:00 | 01/01/2018 09:30:00 | MARKET | SELL       | GBX      |            | 10              | 100          | 100         |
	And With the intraday market data :
	| SecurityName | Epoch      | Bid | Ask | Price | Currency | Volume |
	| UKGovtBondSecondaryMarket     | 01/01/2018  09:30:00| 1	  | 20  | 10    | GBX      | 5000  |
	| UKGovtBondSecondaryMarket     | 01/01/2018  09:29:00| 1	  | 20  | 10    | GBX      | 5000  |
	When I run the fixed income high volume rule
	Then I will have 1 fixed income high volume alerts

@fixedincomehighvolumewindowvolume
Scenario: One order in at a different exchange and in a different currency at window volume yields one alert
	Given I have the orders for a universe from 01/01/2018 to 03/01/2018 :
	| SecurityName | OrderId | PlacedDate          | FilledDate          | Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
	| Nvidia     | 0       | 01/01/2018 17:30:00 | 01/01/2018 17:30:00 | MARKET | BUY       | USD      |            | 10              | 100          | 100         |
	And With the intraday market data :
	| SecurityName | Epoch      | Bid | Ask | Price | Currency | Volume |
	| Nvidia     | 01/01/2018  17:30:00| 1	  | 20  | 10    | USD      | 5000  |
	| Nvidia     | 01/01/2018  17:29:00| 1	  | 20  | 10    | USD      | 5000  |
	When I run the fixed income high volume rule
	Then I will have 1 fixed income high volume alerts

@fixedincomehighvolumewindowvolume
Scenario: One order below window volume yields zero alerts
	Given I have the orders for a universe from 01/01/2018 to 03/01/2018 :
	| SecurityName | OrderId | PlacedDate          | FilledDate          | Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
	| UKGovtBondSecondaryMarket     | 0       | 01/01/2018 09:30:00 | 01/01/2018 09:30:00 | MARKET | BUY       | GBX      |            | 10              | 99          | 99         |
	And With the intraday market data :
	| SecurityName | Epoch      | Bid | Ask | Price | Currency | Volume |
	| UKGovtBondSecondaryMarket     | 01/01/2018 09:30:00 | 1	  | 20  | 10    | GBX      | 10000  |
	When I run the fixed income high volume rule
	Then I will have 0 fixed income high volume alerts

@fixedincomehighvolumewindowvolume
Scenario: One order above window volume yields one alerts
	Given I have the orders for a universe from 01/01/2018 to 03/01/2018 :
	| SecurityName | OrderId | PlacedDate          | FilledDate          | Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
	| UKGovtBondSecondaryMarket     | 0       | 01/01/2018 09:30:00 | 01/01/2018 09:30:00 | MARKET | BUY       | GBX      |            | 10              | 101          | 101         |
	And With the intraday market data :
	| SecurityName | Epoch      | Bid | Ask | Price | Currency | Volume |
	| UKGovtBondSecondaryMarket     | 01/01/2018 09:30:00 | 1	  | 20  | 10    | GBX      | 10000  |
	When I run the fixed income high volume rule
	Then I will have 1 fixed income high volume alerts

@fixedincomehighvolumewindowvolume
Scenario: Two orders split just outside of window at window volume yields zero alert
	Given I have the orders for a universe from 01/01/2018 to 03/01/2018 :
	| SecurityName | OrderId | PlacedDate          | FilledDate          | Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
	| UKGovtBondSecondaryMarket     | 0       | 01/01/2018 09:30:00 | 01/01/2018 09:30:00 | MARKET | BUY       | GBX      |            | 10              | 50          | 50         |
	| UKGovtBondSecondaryMarket     | 0       | 01/01/2018 10:35:00 | 01/01/2018 10:35:00 | MARKET | BUY       | GBX      |            | 10              | 50          | 50         |
	And With the intraday market data :
	| SecurityName | Epoch      | Bid | Ask | Price | Currency | Volume |
	| UKGovtBondSecondaryMarket     | 01/01/2018  09:30:00| 1	  | 20  | 10    | GBX      | 5000  |
	| UKGovtBondSecondaryMarket     | 01/01/2018  09:29:00| 1	  | 20  | 10    | GBX      | 5000  |
	When I run the fixed income high volume rule
	Then I will have 0 fixed income high volume alerts

@fixedincomehighvolumewindowvolume
Scenario: Two orders split just on window at window volume yields one alert
	Given I have the orders for a universe from 01/01/2018 to 03/01/2018 :
	| SecurityName | OrderId | PlacedDate          | FilledDate          | Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
	| UKGovtBondSecondaryMarket     | 0       | 01/01/2018 09:30:00 | 01/01/2018 09:30:00 | MARKET | BUY       | GBX      |            | 10              | 50          | 50         |
	| UKGovtBondSecondaryMarket     | 0       | 01/01/2018 10:30:00 | 01/01/2018 10:30:00 | MARKET | BUY       | GBX      |            | 10              | 50          | 50         |
	And With the intraday market data :
	| SecurityName | Epoch      | Bid | Ask | Price | Currency | Volume |
	| UKGovtBondSecondaryMarket     | 01/01/2018  09:30:00| 1	  | 20  | 10    | GBX      | 5000  |
	| UKGovtBondSecondaryMarket     | 01/01/2018  09:29:00| 1	  | 20  | 10    | GBX      | 5000  |
	When I run the fixed income high volume rule
	Then I will have 1 fixed income high volume alerts

@fixedincomehighvolumewindowvolume
Scenario: Two orders split just inside window at window volume yields one alert
	Given I have the orders for a universe from 01/01/2018 to 03/01/2018 :
	| SecurityName | OrderId | PlacedDate          | FilledDate          | Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
	| UKGovtBondSecondaryMarket     | 0       | 01/01/2018 09:30:00 | 01/01/2018 09:30:00 | MARKET | BUY       | GBX      |            | 10              | 50          | 50         |
	| UKGovtBondSecondaryMarket     | 0       | 01/01/2018 10:25:00 | 01/01/2018 10:25:00 | MARKET | BUY       | GBX      |            | 10              | 50          | 50         |
	And With the intraday market data :
	| SecurityName | Epoch      | Bid | Ask | Price | Currency | Volume |
	| UKGovtBondSecondaryMarket     | 01/01/2018  09:30:00| 1	  | 20  | 10    | GBX      | 5000  |
	| UKGovtBondSecondaryMarket     | 01/01/2018  09:29:00| 1	  | 20  | 10    | GBX      | 5000  |
	When I run the fixed income high volume rule
	Then I will have 1 fixed income high volume alerts

@fixedincomehighvolumewindowvolume
Scenario: One order at window volume but with market data on preceding day yields one alert
	Given I have the fixed income high volume rule parameter values
	| WindowHours | FixedIncomeHighVolumePercentageDaily | FixedIncomeHighVolumePercentageWindow |
	| 100           |                                      | 0.01                                 |
	And I have the orders for a universe from 01/01/2018 to 03/01/2018 :
	| SecurityName | OrderId | PlacedDate          | FilledDate          | Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
	| UKGovtBondSecondaryMarket     | 0       | 01/02/2018 09:30:00 | 01/02/2018 09:30:00 | MARKET | BUY       | GBX      |            | 10              | 100          | 100         |
	And With the intraday market data :
	| SecurityName | Epoch				 | Bid | Ask | Price | Currency | Volume |
	| UKGovtBondSecondaryMarket     | 01/01/2018  09:30:00| 1	  | 20  | 10    | GBX      | 2500  |
	| UKGovtBondSecondaryMarket     | 01/01/2018  09:29:00| 1	  | 20  | 10    | GBX      | 2500  |
	| UKGovtBondSecondaryMarket     | 01/02/2018  09:30:00| 1	  | 20  | 10    | GBX      | 2500  |
	| UKGovtBondSecondaryMarket     | 01/02/2018  09:29:00| 1	  | 20  | 10    | GBX      | 2500  |
	When I run the fixed income high volume rule
	Then I will have 1 fixed income high volume alerts

@fixedincomehighvolumewindowvolume
Scenario: One order at window volume but with missing data on preceding day yields one alert
		Given I have the fixed income high volume rule parameter values
		| WindowHours | FixedIncomeHighVolumePercentageDaily | FixedIncomeHighVolumePercentageWindow |
		| 1000        |                                      | 0.01                                 |
	And I have the orders for a universe from 01/01/2018 to 03/01/2018 :
	| SecurityName | OrderId | PlacedDate          | FilledDate          | Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
	| UKGovtBondSecondaryMarket     | 0       | 01/04/2018 09:30:00 | 01/04/2018 09:30:00 | MARKET | BUY       | GBX      |            | 10              | 99          | 99         |
	And With the intraday market data :
	| SecurityName | Epoch				 | Bid | Ask | Price | Currency | Volume |
	| UKGovtBondSecondaryMarket     | 01/01/2018  09:30:00| 1	  | 20  | 10    | GBX      | 2500  |
	| UKGovtBondSecondaryMarket     | 01/02/2018  09:30:00| 1	  | 20  | 10    | GBX      | 2500  |
	| UKGovtBondSecondaryMarket     | 01/03/2018  09:30:00| 1	  | 20  | 10    | GBX      | 2500  |
	| UKGovtBondSecondaryMarket     | 01/04/2018  09:29:00| 1	  | 20  | 10    | GBX      | 2500  |
	When I run the fixed income high volume rule
	Then I will have 0 fixed income high volume alerts

