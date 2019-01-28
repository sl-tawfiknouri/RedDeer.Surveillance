Feature: WashTrade Average Netting Sensitive Parameters
	In order to meet MAR compliance requirements
	I need to be able to detect when traders are executing trades
	with no meaningful change of ownership
	By netting their trades for average value change being below
	threshold parameters

Background:
	Given I have the wash trade rule average netting parameter values
	| WindowHours | MinimumNumberOfTrades | MaximumPositionChangeValue | MaximumAbsoluteValueChange | MaximumAbsoluteValueChangeCurrency |
	| 1           | 2                     | 0.01                       | 10000                      | GBP                                |

@washtrade
@washtradesensitive
Scenario: Empty Universe yields no alerts
	Given I have the orders for a universe from 01/01/2018 to 03/01/2018 :
	| SecurityName | OrderId | PlacedDate          | BookedDate | AmendedDate | RejectedDate | CancelledDate | FilledDate          | Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
	When I run the wash trade rule
	Then I will have 0 wash trade alerts

@washtrade
@washtradesensitive
Scenario: One Trade For Vodafone Universe yields no alerts
	Given I have the orders for a universe from 01/01/2018 to 03/01/2018 :
	| SecurityName | OrderId | PlacedDate          | BookedDate | AmendedDate | RejectedDate | CancelledDate | FilledDate          | Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
	| Vodafone     | 0       | 01/01/2018 09:30:00 |            |             |              |               | 01/01/2018 09:30:00 | MARKET | BUY       | GBX      |            | 10.01            | 1000          | 1000         |     
	When I run the wash trade rule
	Then I will have 0 wash trade alerts

@washtrade
@washtradesensitive
Scenario: One Trade For Barclays Universe yields no alerts
	Given I have the orders for a universe from 01/01/2018 to 03/01/2018 :
	| SecurityName | OrderId | PlacedDate          | BookedDate | AmendedDate | RejectedDate | CancelledDate | FilledDate          | Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
	| Barclays     | 0       | 01/01/2018 09:30:00 |            |             |              |               | 01/01/2018 09:30:00 | MARKET | BUY       | GBX      |            | 10.01            | 1000          | 1000         |     
	When I run the wash trade rule
	Then I will have 0 wash trade alerts

@washtrade
@washtradesensitive
Scenario: Two Trades In Wash Trade Universe yields one alert
	Given I have the orders for a universe from 01/01/2018 to 03/01/2018 :
	| SecurityName | OrderId | PlacedDate          | BookedDate | AmendedDate | RejectedDate | CancelledDate | FilledDate          | Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
	| Vodafone     | 0       | 01/01/2018 09:30:00 |            |             |              |               | 01/01/2018 09:30:00 | MARKET | BUY       | GBX      |            | 10.01            | 1000          | 1000         |     
	| Vodafone     | 1       | 01/01/2018 09:30:00 |            |             |              |               | 01/01/2018 09:30:00 | MARKET | SELL      | GBX      |            | 10.01            | 1000          | 1000         |     
	When I run the wash trade rule
	Then I will have 1 wash trade alerts

@washtrade
@washtradesensitive
Scenario: Two Trades In Wash Trade Universe For Different Securities yields one alert
	Given I have the orders for a universe from 01/01/2018 to 03/01/2018 :
	| SecurityName | OrderId | PlacedDate          | BookedDate | AmendedDate | RejectedDate | CancelledDate | FilledDate          | Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
	| Vodafone     | 0       | 01/01/2018 09:30:00 |            |             |              |               | 01/01/2018 09:30:00 | MARKET | BUY       | GBX      |            | 10.01            | 1000          | 1000         |     
	| Barclays     | 1       | 01/01/2018 09:30:00 |            |             |              |               | 01/01/2018 09:30:00 | MARKET | SELL      | GBX      |            | 10.01            | 1000          | 1000         |     
	When I run the wash trade rule
	Then I will have 0 wash trade alerts

@washtrade
@washtradesensitive
Scenario: Three Trades In Wash Trade Universe yields no alerts
	Given I have the orders for a universe from 01/01/2018 to 03/01/2018 :
	| SecurityName | OrderId | PlacedDate          | BookedDate | AmendedDate | RejectedDate | CancelledDate | FilledDate          | Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
	| Vodafone     | 0       | 01/01/2018 09:30:00 |            |             |              |               | 01/01/2018 09:30:00 | MARKET | BUY       | GBX      |            | 10.01            | 1000          | 1000         |     
	| Vodafone     | 1       | 01/01/2018 09:30:00 |            |             |              |               | 01/01/2018 09:30:00 | MARKET | BUY       | GBX      |            | 10.01            | 1000          | 1000         |     
	| Vodafone     | 2       | 01/01/2018 09:30:00 |            |             |              |               | 01/01/2018 09:30:00 | MARKET | SELL      | GBX      |            | 10.01            | 1000          | 1000         |     
	When I run the wash trade rule
	Then I will have 0 wash trade alerts

@washtrade
@washtradesensitive
Scenario: Four trades at two price points yields two alerts
	Given I have the orders for a universe from 01/01/2018 to 03/01/2018 :
	| SecurityName | OrderId | PlacedDate          | BookedDate | AmendedDate | RejectedDate | CancelledDate | FilledDate          | Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
	| Vodafone     | 0       | 01/01/2018 09:30:00 |            |             |              |               | 01/01/2018 09:30:00 | MARKET | BUY       | GBX      |            | 10.01            | 1000          | 1000         |     
	| Vodafone     | 1       | 01/01/2018 09:30:00 |            |             |              |               | 01/01/2018 09:30:00 | MARKET | SELL      | GBX      |            | 10.01            | 1000          | 1000         |     
	| Vodafone     | 2       | 01/01/2018 09:35:00 |            |             |              |               | 01/01/2018 09:35:00 | MARKET | BUY       | GBX      |            | 20.01            | 1000          | 1000         |     
	| Vodafone     | 3       | 01/01/2018 09:35:00 |            |             |              |               | 01/01/2018 09:35:00 | MARKET | SELL      | GBX      |            | 20.01            | 1000          | 1000         |     
	When I run the wash trade rule
	Then I will have 2 wash trade alerts

@washtrade
@washtradesensitive
Scenario: Five trades in two pairs with a single trade per three price points yields two alerts
	Given I have the orders for a universe from 01/01/2018 to 03/01/2018 :
	| SecurityName | OrderId | PlacedDate          | BookedDate | AmendedDate | RejectedDate | CancelledDate | FilledDate          | Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
	| Vodafone     | 0       | 01/01/2018 09:30:00 |            |             |              |               | 01/01/2018 09:30:00 | MARKET | BUY       | GBX      |            | 10.01            | 1000          | 1000         |     
	| Vodafone     | 1       | 01/01/2018 09:30:00 |            |             |              |               | 01/01/2018 09:30:00 | MARKET | SELL      | GBX      |            | 10.01            | 1000          | 1000         |     
	| Vodafone     | 2       | 01/01/2018 09:35:00 |            |             |              |               | 01/01/2018 09:35:00 | MARKET | BUY       | GBX      |            | 20.01            | 1000          | 1000         |     
	| Vodafone     | 3       | 01/01/2018 09:35:00 |            |             |              |               | 01/01/2018 09:35:00 | MARKET | SELL      | GBX      |            | 20.01            | 1000          | 1000         |     
	| Vodafone     | 4       | 01/01/2018 09:40:00 |            |             |              |               | 01/01/2018 09:40:00 | MARKET | BUY       | GBX      |            | 30.01            | 1000          | 1000         |     
	When I run the wash trade rule
	Then I will have 2 wash trade alerts


