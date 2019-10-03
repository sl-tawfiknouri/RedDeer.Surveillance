@FixedIncomeHighVolume
@FixedIncomeHighVolumeNonSensitive
Feature: Fixed Income High Volume Issuance Percentage Non Sensitive
	In order to meet MAR compliance requirements
	I need to be able to detect when traders are executing trades
	Which have unusually high volume as a proportion
	of the percentage issuance

Background:
			Given I have the fixed income high volume rule parameter values
			| WindowHours | FixedIncomeHighVolumePercentageDaily | FixedIncomeHighVolumePercentageWindow |
			| 1           |                                      | 0.01                                  |

Scenario: Empty Universe yields no alerts
	Given I have the orders for a universe from 01/01/2018 to 03/01/2018 :
	| SecurityName | OrderId | PlacedDate          | BookedDate | AmendedDate | RejectedDate | CancelledDate | FilledDate  | Type | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
	When I run the fixed income high volume rule
	Then I will have 0 fixed income high volume alerts


	Scenario: Single order yields no alerts
	Given I have the orders for a universe from 01/01/2018 to 03/01/2018 :
	| SecurityName              | OrderId | PlacedDate          | FilledDate          | Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
	| UKGovtBondSecondaryMarket | 0       | 01/01/2018 09:30:00 | 01/01/2018 09:30:00 | MARKET | BUY       | GBX      |            | 10               | 100           | 100          |
	And With the intraday market data :
	| SecurityName              | Epoch                | Bid | Ask | Price | Currency | Volume |
	| UKGovtBondSecondaryMarket | 01/01/2018  09:29:00 | 1   | 20  | 10    | GBX      | 10000   |
	| UKGovtBondSecondaryMarket | 01/01/2018  09:28:00 | 1   | 20  | 10    | GBX      | 10000   |
	When I run the fixed income high volume rule
	Then I will have 0 fixed income high volume alerts

	Scenario: Buy Sell orders within the time window yields two alerts
	Given I have the orders for a universe from 01/01/2018 to 03/01/2018 :
	| SecurityName              | OrderId | PlacedDate          | BookedDate          | Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume | CleanOrDirty |
	| UKGovtBondSecondaryMarket | 0       | 01/01/2018 09:30:00 | 01/01/2018 09:30:00 | MARKET | BUY       | GBX      |            | 10               | 100           | 10000          | Clean        |
	| UKGovtBondSecondaryMarket | 1       | 01/01/2018 09:31:00 | 01/01/2018 09:31:00 | MARKET | SELL      | GBX      |            | 12               | 100           | 10000          | Clean        |
		And With the intraday market data :
	| SecurityName              | Epoch                | Bid | Ask | Price | Currency | Volume |
	| UKGovtBondSecondaryMarket | 01/01/2018  09:29:00 | 1   | 20  | 10    | GBX      | 10000   |
	| UKGovtBondSecondaryMarket | 01/01/2018  09:28:00 | 1   | 20  | 10    | GBX      | 10000   |
	When I run the fixed income high volume rule
	Then I will have 2 fixed income high volume alerts

	Scenario: Buy Sell orders next day but within the time window yields Two alerts
	Given I have the fixed income high volume rule parameter values
	| WindowHours | FixedIncomeHighVolumePercentageDaily | FixedIncomeHighVolumePercentageWindow |
	| 1           |                                      | 0.01                                  |
	And I have the orders for a universe from 01/01/2018 to 03/01/2018 :
	| SecurityName              | OrderId | PlacedDate          | BookedDate          | Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume | CleanOrDirty |
	| UKGovtBondSecondaryMarket | 0       | 01/01/2018 09:30:00 | 01/01/2018 09:30:00 | MARKET | BUY       | GBX      |            | 10               | 100           | 1000          | Clean        |
	| UKGovtBondSecondaryMarket | 1       | 01/02/2018 09:31:00 | 01/02/2018 09:31:00 | MARKET | SELL      | GBX      |            | 12               | 100           | 1000          | Clean        |
	And With the intraday market data :
	| SecurityName              | Epoch                | Bid | Ask | Price | Currency | Volume |
	| UKGovtBondSecondaryMarket | 01/01/2018  09:29:00 | 1   | 20  | 10    | GBX      | 10000   |
	| UKGovtBondSecondaryMarket | 01/02/2018  09:28:00 | 1   | 20  | 10    | GBX      | 10000   |
	When I run the fixed income high volume rule
	Then I will have 2 fixed income high volume alerts

	
	Scenario: Buy Sell orders next day but Outside the Window yields No alerts
	Given I have the fixed income high volume rule parameter values
	| WindowHours | FixedIncomeHighVolumePercentageDaily | FixedIncomeHighVolumePercentageWindow |
	| 1           |                                      | 0.01                                  |
	And I have the orders for a universe from 01/01/2018 to 03/01/2018 :
	| SecurityName              | OrderId | PlacedDate          | BookedDate          | Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume | CleanOrDirty |
	| UKGovtBondSecondaryMarket | 0       | 01/01/2018 09:30:00 | 01/01/2018 09:30:00 | MARKET | BUY       | GBX      |            | 10               | 100           | 1000          | Clean        |
	| UKGovtBondSecondaryMarket | 1       | 01/01/2018 11:31:00 | 01/01/2018 09:31:00 | MARKET | SELL      | GBX      |            | 12               | 100           | 1000          | Clean        |
	And With the intraday market data :
	| SecurityName              | Epoch                | Bid | Ask | Price | Currency | Volume |
	| UKGovtBondSecondaryMarket | 01/01/2018  09:29:00 | 1   | 20  | 10    | GBX      | 1000  |
	| UKGovtBondSecondaryMarket | 01/01/2018  09:28:00 | 1   | 20  | 10    | GBX      | 1000 |
	When I run the fixed income high volume rule
	Then I will have 2 fixed income high volume alerts

	
