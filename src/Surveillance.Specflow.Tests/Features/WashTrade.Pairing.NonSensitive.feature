Feature: WashTrade Pairing Non Sensitive Parameters
	In order to meet MAR compliance requirements
	I need to be able to detect when traders are executing trades
	with no meaningful change of ownership
	By pairing their trades for average value change being below
	threshold parameters

Background:
	Given I have the wash trade rule parameter values
	| WindowHours | PairingPositionMinimumNumberOfPairedTrades | PairingPositionPercentagePriceChangeThresholdPerPair | PairingPositionPercentageVolumeDifferenceThreshold | PairingPositionMaximumAbsoluteCurrencyAmount | PairingPositionMaximumAbsoluteCurrency | UsePairing |
	| 1           | 2                                          | 0.10	                                              | 0.10                                               | 1000000                                      | GBX									   | true       |

@washtrade
@washtradepairing
@washtradenonsensitive
Scenario: Empty Universe yields no alerts
	Given I have the orders for a universe from 01/01/2018 to 03/01/2018 :
	| SecurityName | OrderId | PlacedDate          | BookedDate | AmendedDate | RejectedDate | CancelledDate | FilledDate          | Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
	When I run the wash trade rule
	Then I will have 0 wash trade alerts

@washtrade
@washtradepairing
@washtradenonsensitive
Scenario: One Trade For Vodafone yields no alerts
	Given I have the orders for a universe from 01/01/2018 to 03/01/2018 :
	| SecurityName | OrderId | PlacedDate          | BookedDate | AmendedDate | RejectedDate | CancelledDate | FilledDate          | Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
	| Vodafone     | 0       | 01/01/2018 09:30:00 |            |             |              |               | 01/01/2018 09:30:00 | MARKET | BUY       | GBX      |            | 100              | 1000          | 1000         |     
	When I run the wash trade rule
	Then I will have 0 wash trade alerts

@washtrade
@washtradepairing
@washtradenonsensitive
Scenario: One Trade For Barclays yields no alerts
	Given I have the orders for a universe from 01/01/2018 to 03/01/2018 :
	| SecurityName | OrderId | PlacedDate          | BookedDate | AmendedDate | RejectedDate | CancelledDate | FilledDate          | Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
	| Barclays     | 0       | 01/01/2018 09:30:00 |            |             |              |               | 01/01/2018 09:30:00 | MARKET | BUY       | GBX      |            | 100              | 1000          | 1000         |     
	When I run the wash trade rule
	Then I will have 0 wash trade alerts

@washtrade
@washtradepairing
@washtradenonsensitive
Scenario: Two Trades In Wash Trade For Different Securities yields no alert
	Given I have the orders for a universe from 01/01/2018 to 03/01/2018 :
	| SecurityName | OrderId | PlacedDate          | BookedDate | AmendedDate | RejectedDate | CancelledDate | FilledDate          | Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
	| Vodafone     | 0       | 01/01/2018 09:30:00 |            |             |              |               | 01/01/2018 09:30:00 | MARKET | BUY       | GBX      |            | 100              | 1000          | 1000         |     
	| Barclays     | 1       | 01/01/2018 09:30:00 |            |             |              |               | 01/01/2018 09:30:00 | MARKET | SELL      | GBX      |            | 100              | 1000          | 1000         |     
	When I run the wash trade rule
	Then I will have 0 wash trade alerts

@washtrade
@washtradepairing
@washtradenonsensitive
Scenario: Three Trades at same price point In Wash Trade yields no alerts
	Given I have the orders for a universe from 01/01/2018 to 03/01/2018 :
	| SecurityName | OrderId | PlacedDate          | BookedDate | AmendedDate | RejectedDate | CancelledDate | FilledDate          | Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
	| Vodafone     | 0       | 01/01/2018 09:30:00 |            |             |              |               | 01/01/2018 09:30:00 | MARKET | BUY       | GBX      |            | 100              | 1000          | 1000         |     
	| Vodafone     | 1       | 01/01/2018 09:30:00 |            |             |              |               | 01/01/2018 09:30:00 | MARKET | BUY       | GBX      |            | 100              | 1000          | 1000         |     
	| Vodafone     | 2       | 01/01/2018 09:30:00 |            |             |              |               | 01/01/2018 09:30:00 | MARKET | SELL      | GBX      |            | 100              | 1000          | 1000         |     
	When I run the wash trade rule
	Then I will have 0 wash trade alerts

@washtrade
@washtradepairing
@washtradenonsensitive
Scenario: Two Trades In Wash Trade yields one alert
	Given I have the orders for a universe from 01/01/2018 to 03/01/2018 :
	| SecurityName | OrderId | PlacedDate          | BookedDate | AmendedDate | RejectedDate | CancelledDate | FilledDate          | Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
	| Vodafone     | 0       | 01/01/2018 09:30:00 |            |             |              |               | 01/01/2018 09:30:00 | MARKET | BUY       | GBX      |            | 100              | 1000          | 1000         |     
	| Vodafone     | 1       | 01/01/2018 09:30:00 |            |             |              |               | 01/01/2018 09:30:00 | MARKET | SELL      | GBX      |            | 100              | 1000          | 1000         |     
	When I run the wash trade rule
	Then I will have 1 wash trade alerts


@washtrade
@washtradepairing
@washtradenonsensitive
@timewindow
Scenario: Two Trade For Micron yields one alerts when within 1 hour
	Given I have the orders for a universe from 01/01/2018 to 03/01/2018 :
	| SecurityName | OrderId | PlacedDate          | BookedDate | AmendedDate | RejectedDate | CancelledDate | FilledDate			| Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
	| Micron     | 0		| 01/01/2018 09:30:00 |            |             |              |               | 01/01/2018 09:30:00	| MARKET | BUY       | GBX      |            | 100              | 1000          | 1000         |     
	| Micron     | 1		| 01/01/2018 10:00:00 |            |             |              |               | 01/01/2018 10:00:00	| MARKET | SELL       | GBX      |            | 100              | 1000          | 1000         |     
	When I run the wash trade rule
	Then I will have 1 wash trade alerts

@washtrade
@washtradepairing
@washtradenonsensitive
@timewindow
Scenario: Two Trade For Micron yields no alerts when 2 hours apart
	Given I have the orders for a universe from 01/01/2018 to 03/01/2018 :
	| SecurityName | OrderId | PlacedDate          | BookedDate | AmendedDate | RejectedDate | CancelledDate | FilledDate          | Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
	| Micron     | 0       | 01/01/2018 09:30:00 |            |             |              |               | 01/01/2018 09:30:00 | MARKET | BUY       | GBX      |            | 100              | 1000          | 1000         |     
	| Micron     | 1       | 01/01/2018 11:30:00 |            |             |              |               | 01/01/2018 11:30:00 | MARKET | SELL       | GBX      |            | 100              | 1000          | 1000         |     
	When I run the wash trade rule
	Then I will have 0 wash trade alerts

@washtrade
@washtradepairing
@washtradenonsensitive
@timewindow
Scenario: Two Trade For Micron yields one alerts when exactly 1 hour apart
	Given I have the orders for a universe from 01/01/2018 to 03/01/2018 :
	| SecurityName | OrderId | PlacedDate          | BookedDate | AmendedDate | RejectedDate | CancelledDate | FilledDate          | Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
	| Micron     | 0       | 01/01/2018 09:33:00 |            |             |              |               | 01/01/2018 09:33:00 | MARKET | BUY       | GBX      |            | 100              | 1000          | 1000         |     
	| Micron     | 1       | 01/01/2018 10:33:00 |            |             |              |               | 01/01/2018 10:33:00 | MARKET | SELL       | GBX      |            | 100              | 1000          | 1000         |     
	When I run the wash trade rule
	Then I will have 1 wash trade alerts

@washtrade
@washtradepairing
@washtradenonsensitive
@percentofvaluechange
Scenario: Two Trades In Wash Trade but outside of relative percentage change yields zero alerts
	Given I have the orders for a universe from 01/01/2018 to 03/01/2018 :
	| SecurityName | OrderId | PlacedDate          | BookedDate | AmendedDate | RejectedDate | CancelledDate | FilledDate          | Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
	| BAE     | 0       | 01/01/2018 09:30:00 |            |             |              |               | 01/01/2018 09:30:00 | MARKET | BUY       | GBX      |            | 100              | 1000          | 1000         |     
	| BAE     | 1       | 01/01/2018 09:30:00 |            |             |              |               | 01/01/2018 09:30:00 | MARKET | SELL      | GBX      |            | 80              | 1000          | 1000         |     
	When I run the wash trade rule
	Then I will have 0 wash trade alerts

@washtrade
@washtradepairing
@washtradenonsensitive
@percentofvaluechange
Scenario: Two Trades In Wash Trade and on relative percentage change boundary yields one alerts
	Given I have the orders for a universe from 01/01/2018 to 03/01/2018 :
	| SecurityName | OrderId | PlacedDate          | BookedDate | AmendedDate | RejectedDate | CancelledDate | FilledDate          | Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
	| BAE     | 0       | 01/01/2018 09:30:00 |            |             |              |               | 01/01/2018 09:30:00 | MARKET | BUY       | GBX      |            | 100              | 1000          | 1000         |     
	| BAE     | 1       | 01/01/2018 09:30:00 |            |             |              |               | 01/01/2018 09:30:00 | MARKET | SELL      | GBX      |            | 110              | 1000          | 1000         |     
	When I run the wash trade rule
	Then I will have 1 wash trade alerts

@washtrade
@washtradepairing
@washtradenonsensitive
@percentofvaluechange
Scenario: Two Trades In Wash Trade and inside of relative percentage change yields one alerts
	Given I have the orders for a universe from 01/01/2018 to 03/01/2018 :
	| SecurityName | OrderId | PlacedDate          | BookedDate | AmendedDate | RejectedDate | CancelledDate | FilledDate          | Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
	| BAE     | 0       | 01/01/2018 09:30:00 |            |             |              |               | 01/01/2018 09:30:00 | MARKET | BUY       | GBX      |            | 100              | 1000          | 1000         |     
	| BAE     | 1       | 01/01/2018 09:30:00 |            |             |              |               | 01/01/2018 09:30:00 | MARKET | SELL      | GBX      |            | 109              | 1000          | 1000         |     
	When I run the wash trade rule
	Then I will have 1 wash trade alerts

@washtrade
@washtradepairing
@washtradenonsensitive
@percentofvolumechange
Scenario: Two Trades In Wash Trade but outside of relative percentage volume change yields zero alerts
	Given I have the orders for a universe from 01/01/2018 to 03/01/2018 :
	| SecurityName | OrderId | PlacedDate          | BookedDate | AmendedDate | RejectedDate | CancelledDate | FilledDate     | Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
	| AMD     | 0       | 01/01/2018 09:30:00 |            |             |              |               | 01/01/2018 09:30:00 | MARKET | BUY       | GBX      |            | 100              | 1000          | 1000         |     
	| AMD     | 1       | 01/01/2018 09:30:00 |            |             |              |               | 01/01/2018 09:30:00 | MARKET | SELL      | GBX      |            | 97               | 1200          | 1200         |     
	When I run the wash trade rule
	Then I will have 0 wash trade alerts

@washtrade
@washtradepairing
@washtradenonsensitive
@percentofvolumechange
Scenario: Two Trades In Wash Trade and on relative percentage volume change boundary yields one alerts
	Given I have the orders for a universe from 01/01/2018 to 03/01/2018 :
	| SecurityName | OrderId | PlacedDate          | BookedDate | AmendedDate | RejectedDate | CancelledDate | FilledDate     | Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
	| AMD     | 0       | 01/01/2018 09:30:00 |            |             |              |               | 01/01/2018 09:30:00 | MARKET | BUY       | GBX      |            | 100              | 1000          | 1000         |     
	| AMD     | 1       | 01/01/2018 09:30:00 |            |             |              |               | 01/01/2018 09:30:00 | MARKET | SELL      | GBX      |            | 97               | 1000          | 1000         |     
	When I run the wash trade rule
	Then I will have 1 wash trade alerts

@washtrade
@washtradepairing
@washtradenonsensitive
@percentofvolumechange
Scenario: Two Trades In Wash Trade and inside of relative percentage volume change yields one alerts
	Given I have the orders for a universe from 01/01/2018 to 03/01/2018 :
	| SecurityName | OrderId | PlacedDate          | BookedDate | AmendedDate | RejectedDate | CancelledDate | FilledDate     | Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
	| AMD     | 0       | 01/01/2018 09:30:00 |            |             |              |               | 01/01/2018 09:30:00 | MARKET | BUY       | GBX      |            | 100              | 1000          | 1000         |     
	| AMD     | 1       | 01/01/2018 09:30:00 |            |             |              |               | 01/01/2018 09:30:00 | MARKET | SELL      | GBX      |            | 97               | 1099          | 1099         |     
	When I run the wash trade rule
	Then I will have 1 wash trade alerts


@washtrade
@washtradepairing
@washtradenonsensitive
@maximumabsolutevaluechange
Scenario: Two Trades In Wash Trade and inside of absolute value change yields one alerts
	Given I have the orders for a universe from 01/01/2018 to 03/01/2018 :
	| SecurityName | OrderId | PlacedDate          | BookedDate | AmendedDate | RejectedDate | CancelledDate | FilledDate     | Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
	| Vodafone     | 0       | 01/01/2018 09:30:00 |            |             |              |               | 01/01/2018 09:30:00 | MARKET | BUY       | GBX      |            | 1000000              | 1000000         | 1000000        |     
	| Vodafone     | 1       | 01/01/2018 09:30:00 |            |             |              |               | 01/01/2018 09:30:00 | MARKET | SELL      | GBX      |            | 1000000              | 1000000          | 1000000         |     
	When I run the wash trade rule
	Then I will have 1 wash trade alerts

@washtrade
@washtradepairing
@washtradenonsensitive
@maximumabsolutevaluechange
Scenario: Two Trades In Wash Trade and exactly absolute value change yields one alerts
	Given I have the orders for a universe from 01/01/2018 to 03/01/2018 :
	| SecurityName | OrderId | PlacedDate          | BookedDate | AmendedDate | RejectedDate | CancelledDate | FilledDate     | Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
	| Vodafone     | 0       | 01/01/2018 09:30:00 |            |             |              |               | 01/01/2018 09:30:00 | MARKET | BUY       | GBX      |            | 1000000              | 1000000         | 1000000        |     
	| Vodafone     | 1       | 01/01/2018 09:30:00 |            |             |              |               | 01/01/2018 09:30:00 | MARKET | SELL      | GBX      |            | 1000000              | 999999          | 999999         |     
	When I run the wash trade rule
	Then I will have 1 wash trade alerts

@washtrade
@washtradepairing
@washtradenonsensitive
@maximumabsolutevaluechange
Scenario: Two Trades In Wash Trade and outside of absolute value change change yields zero alerts
	Given I have the orders for a universe from 01/01/2018 to 03/01/2018 :
	| SecurityName | OrderId | PlacedDate          | BookedDate | AmendedDate | RejectedDate | CancelledDate | FilledDate     | Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
	| Vodafone     | 0       | 01/01/2018 09:30:00 |            |             |              |               | 01/01/2018 09:30:00 | MARKET | BUY       | GBX      |            | 1000000              | 1000000         | 1000000        |     
	| Vodafone     | 1       | 01/01/2018 09:30:00 |            |             |              |               | 01/01/2018 09:30:00 | MARKET | SELL      | GBX      |            | 1000000              | 999998          | 999998         |     
	When I run the wash trade rule
	Then I will have 0 wash trade alerts



@washtrade
@washtradepairing
@washtradenonsensitive
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

@washtrade
@washtradepairing
@washtradenonsensitive
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


@washtrade
@washtradepairing
@washtradenonsensitive
@washtradelosses
Scenario: Two Trade For Nvidia yields one alerts with losses
	Given I have the orders for a universe from 01/01/2018 to 03/01/2018 :
	| SecurityName | OrderId | PlacedDate          | BookedDate | AmendedDate | RejectedDate | CancelledDate | FilledDate			| Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
	| Nvidia     | 0		| 01/01/2018 09:30:00 |            |             |              |               | 01/01/2018 09:30:00	| MARKET | BUY       | GBX      |            | 100              | 1000          | 1000         |     
	| Nvidia     | 0		| 01/01/2018 09:30:00 |            |             |              |               | 01/01/2018 09:30:00	| MARKET | BUY       | GBX      |            | 100              | 1000          | 1000         |     
	| Nvidia     | 1		| 01/01/2018 10:00:00 |            |             |              |               | 01/01/2018 10:00:00	| MARKET | SELL       | GBX      |            | 99              | 1000          | 1000         |     
	| Nvidia     | 1		| 01/01/2018 10:00:00 |            |             |              |               | 01/01/2018 10:00:00	| MARKET | SELL       | GBX      |            | 98              | 1000          | 1000         |     
	When I run the wash trade rule
	Then I will have 1 wash trade alerts


@washtrade
@washtradepairing
@washtradenonsensitive
@washtradepartialfill
Scenario: Two Trade For Nvidia with partial fills yields one alerts
	Given I have the orders for a universe from 01/01/2018 to 03/01/2018 :
	| SecurityName | OrderId | PlacedDate          | BookedDate | AmendedDate | RejectedDate | CancelledDate | FilledDate			| Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
	| Nvidia     | 0		| 01/01/2018 09:30:00 |            |             |              |               | 01/01/2018 09:30:00	| MARKET | BUY       | GBX      |            | 100              | 1000          | 150         |     
	| Nvidia     | 1		| 01/01/2018 10:00:00 |            |             |              |               | 01/01/2018 10:00:00	| MARKET | SELL       | GBX      |            | 100              | 1000          | 150         |     
	When I run the wash trade rule
	Then I will have 1 wash trade alerts



@washtrade
@washtradepairing
@washtradenonsensitive
@washtradeearlyorder
Scenario: Two Trade For Nvidia with pre market order times yields one alerts
	Given I have the orders for a universe from 01/01/2018 to 02/01/2018 :
	| SecurityName | OrderId | PlacedDate          | BookedDate | AmendedDate | RejectedDate | CancelledDate | FilledDate			| Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
	| Nvidia     | 0		| 01/01/2018 06:30:00 |            |             |              |               | 01/01/2018 06:30:00	| MARKET | BUY       | GBX      |            | 100              | 1000          | 150         |     
	| Nvidia     | 1		| 01/01/2018 06:30:00 |            |             |              |               | 01/01/2018 06:30:00	| MARKET | SELL       | GBX      |            | 100              | 1000          | 150         |     
	When I run the wash trade rule
	Then I will have 1 wash trade alerts

@washtrade
@washtradepairing
@washtradenonsensitive
@washtradelateorder
Scenario: Two Trade For Nvidia with post market order times yields one alerts
	Given I have the orders for a universe from 01/01/2018 to 02/01/2018 :
	| SecurityName | OrderId | PlacedDate          | BookedDate | AmendedDate | RejectedDate | CancelledDate | FilledDate			| Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
	| Nvidia     | 0		| 01/01/2018 20:30:00 |            |             |              |               | 01/01/2018 20:30:00	| MARKET | BUY       | GBX      |            | 100              | 1000          | 150         |     
	| Nvidia     | 1		| 01/01/2018 20:30:00 |            |             |              |               | 01/01/2018 20:30:00	| MARKET | SELL       | GBX      |            | 100              | 1000          | 150         |     
	When I run the wash trade rule
	Then I will have 1 wash trade alerts