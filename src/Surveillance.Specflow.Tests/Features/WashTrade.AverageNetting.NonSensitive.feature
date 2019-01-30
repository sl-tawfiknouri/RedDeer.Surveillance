Feature: WashTrade Average Netting Non-Sensitive Parameters
	In order to meet MAR compliance requirements
	I need to be able to detect when traders are executing trades
	with no meaningful change of ownership
	By netting their trades for average value change being below
	threshold parameters

Background:
	Given I have the wash trade rule parameter values
	| WindowHours | MinimumNumberOfTrades | MaximumPositionChangeValue | MaximumAbsoluteValueChange | MaximumAbsoluteValueChangeCurrency | UseAverageNetting |
	| 1           | 2                     | 0.10                       | 1000000                    | GBX                                | true              |

@washtrade
@washtradeAverageNetting
@washtradenonsensitive
Scenario: Empty Universe yields no alerts
	Given I have the orders for a universe from 01/01/2018 to 03/01/2018 :
	| SecurityName | OrderId | PlacedDate          | BookedDate | AmendedDate | RejectedDate | CancelledDate | FilledDate          | Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
	When I run the wash trade rule
	Then I will have 0 wash trade alerts

@washtrade
@washtradeAverageNetting
@washtradenonsensitive
Scenario: One Trade For Vodafone yields no alerts
	Given I have the orders for a universe from 01/01/2018 to 03/01/2018 :
	| SecurityName | OrderId | PlacedDate          | BookedDate | AmendedDate | RejectedDate | CancelledDate | FilledDate          | Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
	| Vodafone     | 0       | 01/01/2018 09:30:00 |            |             |              |               | 01/01/2018 09:30:00 | MARKET | BUY       | GBX      |            | 100              | 1000          | 1000         |     
	When I run the wash trade rule
	Then I will have 0 wash trade alerts


@washtrade
@washtradeAverageNetting
@washtradenonsensitive
@timewindow
Scenario: Two Trade For Nvidia yields one alerts when within 1 hour
	Given I have the orders for a universe from 01/01/2018 to 03/01/2018 :
	| SecurityName | OrderId | PlacedDate          | BookedDate | AmendedDate | RejectedDate | CancelledDate | FilledDate			| Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
	| Nvidia     | 0		| 01/01/2018 09:30:00 |            |             |              |               | 01/01/2018 09:30:00	| MARKET | BUY       | GBX      |            | 100              | 1000          | 1000         |     
	| Nvidia     | 1		| 01/01/2018 10:00:00 |            |             |              |               | 01/01/2018 10:00:00	| MARKET | SELL       | GBX      |            | 100              | 1000          | 1000         |     
	When I run the wash trade rule
	Then I will have 1 wash trade alerts

@washtrade
@washtradeAverageNetting
@washtradenonsensitive
@timewindow
Scenario: Two Trade For Nvidia yields no alerts when 2 hours apart
	Given I have the orders for a universe from 01/01/2018 to 03/01/2018 :
	| SecurityName | OrderId | PlacedDate          | BookedDate | AmendedDate | RejectedDate | CancelledDate | FilledDate          | Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
	| Nvidia     | 0       | 01/01/2018 09:30:00 |            |             |              |               | 01/01/2018 09:30:00 | MARKET | BUY       | GBX      |            | 100              | 1000          | 1000         |     
	| Nvidia     | 1       | 01/01/2018 11:30:00 |            |             |              |               | 01/01/2018 11:30:00 | MARKET | SELL       | GBX      |            | 100              | 1000          | 1000         |     
	When I run the wash trade rule
	Then I will have 0 wash trade alerts

@washtrade
@washtradeAverageNetting
@washtradenonsensitive
@timewindow
Scenario: Two Trade For Nvidia yields one alerts when exactly 1 hour apart
	Given I have the orders for a universe from 01/01/2018 to 03/01/2018 :
	| SecurityName | OrderId | PlacedDate          | BookedDate | AmendedDate | RejectedDate | CancelledDate | FilledDate          | Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
	| Barclays     | 0       | 01/01/2018 09:33:00 |            |             |              |               | 01/01/2018 09:33:00 | MARKET | BUY       | GBX      |            | 100              | 1000          | 1000         |     
	| Barclays     | 1       | 01/01/2018 10:33:00 |            |             |              |               | 01/01/2018 10:33:00 | MARKET | SELL       | GBX      |            | 100              | 1000          | 1000         |     
	When I run the wash trade rule
	Then I will have 1 wash trade alerts