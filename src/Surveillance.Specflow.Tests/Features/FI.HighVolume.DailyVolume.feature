@fixedincomehighvolumedailyvolume
Feature: Fixed Income High Volume Daily Volume
	In order to meet MAR compliance requirements
	I need to be able to detect when traders are executing trades
	Which have unusually high volume as a proportion
	of the percentage of daily trading

Background:
			Given I have the fixed income high volume rule parameter values
			| WindowHours | FixedIncomeHighVolumePercentageDaily | FixedIncomeHighVolumePercentageWindow |
			| 1           | 0.01                                 |                                      |

@fixedincomehighvolumedailyvolume
Scenario: Empty Universe yields no alerts
	Given I have the orders for a universe from 01/01/2018 to 03/01/2018 :
	| SecurityName | OrderId | PlacedDate          | BookedDate | AmendedDate | RejectedDate | CancelledDate | FilledDate          | Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
	When I run the fixed income high volume rule
	Then I will have 0 fixed income high volume alerts

@fixedincomehighvolumedailyvolume
Scenario: One order at daily volume yields one alert
	Given I have the orders for a universe from 01/01/2018 to 03/01/2018 :
	| SecurityName | OrderId | PlacedDate          | BookedDate | AmendedDate | RejectedDate | CancelledDate | FilledDate          | Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
	| UKGovtBondSecondaryMarket     | 0       | 01/01/2018 09:30:00 |            |             |              |               | 01/01/2018 09:30:00 | MARKET | BUY       | GBX      |            | 10              | 10          | 10         |
	And With the interday market data :
	| SecurityName | Epoch               | OpenPrice | ClosePrice | HighIntradayPrice | LowIntradayPrice | ListedSecurities | MarketCap | DailyVolume | Currency |
	| UKGovtBondSecondaryMarket     | 01/01/2018 | 10        | 11         | 11.5              | 10               | 10               | 1000000  | 1000       | GBX      |
	When I run the fixed income high volume rule
	Then I will have 1 fixed income high volume alerts

@fixedincomehighvolumedailyvolume
Scenario: Two order one inside and one outside but next day for window at daily volume yields zero alert
	Given I have the high volume rule parameter values
	| WindowHours | HighVolumePercentageDaily | HighVolumePercentageWindow | HighVolumePercentageMarketCap |  
	| 23           |	0.01					  |		                       | 			     			   |
	And I have the orders for a universe from 01/01/2018 to 03/01/2018 :
	| SecurityName | OrderId | PlacedDate          | BookedDate | AmendedDate | RejectedDate | CancelledDate | FilledDate          | Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
	| UKGovtBondSecondaryMarket     | 0       | 01/01/2018 15:30:00 |            |             |              |               | 01/01/2018 15:30:00 | MARKET | BUY       | GBX      |            | 10              | 5          | 5         |
	| UKGovtBondSecondaryMarket     | 0       | 01/02/2018 16:30:00 |            |             |              |               | 01/02/2018 16:30:00 | MARKET | BUY       | GBX      |            | 10              | 5          | 5         |
	And With the interday market data :
	| SecurityName | Epoch               | OpenPrice | ClosePrice | HighIntradayPrice | LowIntradayPrice | ListedSecurities | MarketCap | DailyVolume | Currency |
	| UKGovtBondSecondaryMarket     | 01/01/2018 | 10        | 11         | 11.5              | 10               | 10               | 1000000  | 1000       | GBX      |
	| UKGovtBondSecondaryMarket     | 01/02/2018 | 10        | 11         | 11.5              | 10               | 10               | 1000000  | 1000       | GBX      |
	When I run the fixed income high volume rule
	Then I will have 0 fixed income high volume alerts

@fixedincomehighvolumedailyvolume
Scenario: Two order one inside and one outside trading hours at daily volume yields one alert
	Given I have the orders for a universe from 01/01/2018 to 03/01/2018 :
	| SecurityName | OrderId | PlacedDate          | BookedDate | AmendedDate | RejectedDate | CancelledDate | FilledDate          | Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
	| UKGovtBondSecondaryMarket     | 0       | 01/01/2018 15:30:00 |            |             |              |               | 01/01/2018 15:30:00 | MARKET | BUY       | GBX      |            | 10              | 5          | 5         |
	| UKGovtBondSecondaryMarket     | 0       | 01/01/2018 16:30:00 |            |             |              |               | 01/01/2018 16:30:00 | MARKET | BUY       | GBX      |            | 10              | 5          | 5         |
	And With the interday market data :
	| SecurityName | Epoch               | OpenPrice | ClosePrice | HighIntradayPrice | LowIntradayPrice | ListedSecurities | MarketCap | DailyVolume | Currency |
	| UKGovtBondSecondaryMarket     | 01/01/2018 | 10        | 11         | 11.5              | 10               | 10               | 1000000  | 1000       | GBX      |
	When I run the fixed income high volume rule
	Then I will have 1 fixed income high volume alerts

@fixedincomehighvolumedailyvolume
Scenario: One order at daily volume partially filled yields one alert
	Given I have the orders for a universe from 01/01/2018 to 03/01/2018 :
	| SecurityName | OrderId | PlacedDate          | BookedDate | AmendedDate | RejectedDate | CancelledDate | FilledDate          | Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
	| UKGovtBondSecondaryMarket     | 0       | 01/01/2018 09:30:00 |            |             |              |               | 01/01/2018 09:30:00 | MARKET | BUY       | GBX      |            | 10              | 100          | 10         |
	And With the interday market data :
	| SecurityName | Epoch               | OpenPrice | ClosePrice | HighIntradayPrice | LowIntradayPrice | ListedSecurities | MarketCap | DailyVolume | Currency |
	| UKGovtBondSecondaryMarket     | 01/01/2018 | 10        | 11         | 11.5              | 10               | 10               | 1000000  | 1000       | GBX      |
	When I run the fixed income high volume rule
	Then I will have 1 fixed income high volume alerts

@fixedincomehighvolumedailyvolume
Scenario: One order at low daily volume yields one alert
	Given I have the orders for a universe from 01/01/2018 to 03/01/2018 :
	| SecurityName | OrderId | PlacedDate          | BookedDate | AmendedDate | RejectedDate | CancelledDate | FilledDate          | Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
	| UKGovtBondSecondaryMarket     | 0       | 01/01/2018 09:30:00 |            |             |              |               | 01/01/2018 09:30:00 | MARKET | BUY       | GBX      |            | 10              | 1          | 1         |
	And With the interday market data :
	| SecurityName | Epoch               | OpenPrice | ClosePrice | HighIntradayPrice | LowIntradayPrice | ListedSecurities | MarketCap | DailyVolume | Currency |
	| UKGovtBondSecondaryMarket     | 01/01/2018 | 10        | 11         | 11.5              | 10               | 10               | 1000000  | 100     | GBX      |
	When I run the fixed income high volume rule
	Then I will have 1 fixed income high volume alerts

@fixedincomehighvolumedailyvolume
Scenario: One order just buy at daily volume yields one alert
	Given I have the orders for a universe from 01/01/2018 to 03/01/2018 :
	| SecurityName | OrderId | PlacedDate          | BookedDate | AmendedDate | RejectedDate | CancelledDate | FilledDate          | Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
	| Barclays     | 0       | 01/01/2018 09:30:00 |            |             |              |               | 01/01/2018 09:30:00 | MARKET | BUY       | GBX      |            | 10              | 10          | 10         |
	And With the interday market data :
	| SecurityName | Epoch               | OpenPrice | ClosePrice | HighIntradayPrice | LowIntradayPrice | ListedSecurities | MarketCap | DailyVolume | Currency |
	| Barclays     | 01/01/2018 | 10        | 11         | 11.5              | 10               | 10               | 1000000  | 1000       | GBX      |
	When I run the fixed income high volume rule
	Then I will have 1 fixed income high volume alerts

@fixedincomehighvolumedailyvolume
Scenario: One order just sell at daily volume yields one alert
	Given I have the orders for a universe from 01/01/2018 to 03/01/2018 :
	| SecurityName | OrderId | PlacedDate          | BookedDate | AmendedDate | RejectedDate | CancelledDate | FilledDate          | Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
	| Barclays     | 0       | 01/01/2018 09:30:00 |            |             |              |               | 01/01/2018 09:30:00 | MARKET | SELL       | GBX      |            | 10              | 10          | 10         |
	And With the interday market data :
	| SecurityName | Epoch               | OpenPrice | ClosePrice | HighIntradayPrice | LowIntradayPrice | ListedSecurities | MarketCap | DailyVolume | Currency |
	| Barclays     | 01/01/2018 | 10        | 11         | 11.5              | 10               | 10               | 1000000  | 1000       | GBX      |
	When I run the fixed income high volume rule
	Then I will have 1 fixed income high volume alerts

@fixedincomehighvolumedailyvolume
Scenario: One order in different exchange and currency at daily volume yields one alert
	Given I have the orders for a universe from 01/01/2018 to 03/01/2018 :
	| SecurityName | OrderId | PlacedDate          | BookedDate | AmendedDate | RejectedDate | CancelledDate | FilledDate          | Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
	| Nvidia     | 0       | 01/01/2018 09:30:00 |            |             |              |               | 01/01/2018 09:30:00 | MARKET | BUY       | USD      |            | 10              | 10          | 10         |
	And With the interday market data :
	| SecurityName | Epoch               | OpenPrice | ClosePrice | HighIntradayPrice | LowIntradayPrice | ListedSecurities | MarketCap | DailyVolume | Currency |
	| Nvidia     | 01/01/2018 | 10        | 11         | 11.5              | 10               | 10               | 1000000  | 1000       | USD      |
	When I run the fixed income high volume rule
	Then I will have 1 fixed income high volume alerts

@fixedincomehighvolumedailyvolume
Scenario: One order below daily volume yields zero alerts
	Given I have the orders for a universe from 01/01/2018 to 03/01/2018 :
	| SecurityName | OrderId | PlacedDate          | BookedDate | AmendedDate | RejectedDate | CancelledDate | FilledDate          | Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
	| UKGovtBondSecondaryMarket     | 0       | 01/01/2018 09:30:00 |            |             |              |               | 01/01/2018 09:30:00 | MARKET | BUY       | GBX      |            | 10              | 9          | 9         |
	And With the interday market data :
	| SecurityName | Epoch               | OpenPrice | ClosePrice | HighIntradayPrice | LowIntradayPrice | ListedSecurities | MarketCap | DailyVolume | Currency |
	| UKGovtBondSecondaryMarket     | 01/01/2018 | 10        | 11         | 11.5              | 10               | 10               | 1000000  | 1000       | GBX      |
	When I run the fixed income high volume rule
	Then I will have 0 fixed income high volume alerts

@fixedincomehighvolumedailyvolume
Scenario: One order above daily volume yields one alerts
	Given I have the orders for a universe from 01/01/2018 to 03/01/2018 :
	| SecurityName | OrderId | PlacedDate          | BookedDate | AmendedDate | RejectedDate | CancelledDate | FilledDate          | Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
	| UKGovtBondSecondaryMarket     | 0       | 01/01/2018 09:30:00 |            |             |              |               | 01/01/2018 09:30:00 | MARKET | BUY       | GBX      |            | 10              | 11          | 11         |
	And With the interday market data :
	| SecurityName | Epoch               | OpenPrice | ClosePrice | HighIntradayPrice | LowIntradayPrice | ListedSecurities | MarketCap | DailyVolume | Currency |
	| UKGovtBondSecondaryMarket     | 01/01/2018 | 10        | 11         | 11.5              | 10               | 10               | 1000000  | 1000       | GBX      |
	When I run the fixed income high volume rule
	Then I will have 1 fixed income high volume alerts
	
@fixedincomehighvolumedailyvolume
Scenario: Two order at daily volume on exact window yields one alert
	Given I have the orders for a universe from 01/01/2018 to 03/01/2018 :
	| SecurityName | OrderId | PlacedDate          | BookedDate | AmendedDate | RejectedDate | CancelledDate | FilledDate          | Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
	| UKGovtBondSecondaryMarket     | 0       | 01/01/2018 09:30:00 |            |             |              |               | 01/01/2018 09:30:00 | MARKET | BUY       | GBX      |            | 10              | 5          | 5         |
	| UKGovtBondSecondaryMarket     | 0       | 01/01/2018 10:30:00 |            |             |              |               | 01/01/2018 10:30:00 | MARKET | BUY       | GBX      |            | 10              | 5          | 5         |
	And With the interday market data :
	| SecurityName | Epoch               | OpenPrice | ClosePrice | HighIntradayPrice | LowIntradayPrice | ListedSecurities | MarketCap | DailyVolume | Currency |
	| UKGovtBondSecondaryMarket     | 01/01/2018 | 10        | 11         | 11.5              | 10               | 10               | 1000000  | 1000       | GBX      |
	When I run the fixed income high volume rule
	Then I will have 1 fixed income high volume alerts

@fixedincomehighvolumedailyvolume
Scenario: Two order at daily volume but outside of window yields zero alert
	Given I have the orders for a universe from 01/01/2018 to 03/01/2018 :
	| SecurityName | OrderId | PlacedDate          | BookedDate | AmendedDate | RejectedDate | CancelledDate | FilledDate          | Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
	| UKGovtBondSecondaryMarket     | 0       | 01/01/2018 09:30:00 |            |             |              |               | 01/01/2018 09:30:00 | MARKET | BUY       | GBX      |            | 10              | 5          | 5         |
	| UKGovtBondSecondaryMarket     | 0       | 01/01/2018 10:35:00 |            |             |              |               | 01/01/2018 10:35:00 | MARKET | BUY       | GBX      |            | 10              | 5          | 5         |
	And With the interday market data :
	| SecurityName | Epoch               | OpenPrice | ClosePrice | HighIntradayPrice | LowIntradayPrice | ListedSecurities | MarketCap | DailyVolume | Currency |
	| UKGovtBondSecondaryMarket     | 01/01/2018 | 10        | 11         | 11.5              | 10               | 10               | 1000000  | 1000       | GBX      |
	When I run the fixed income high volume rule
	Then I will have 0 fixed income high volume alerts

@fixedincomehighvolumedailyvolume
Scenario: Two order at daily volume and inside of window yields one alert
	Given I have the orders for a universe from 01/01/2018 to 03/01/2018 :
	| SecurityName | OrderId | PlacedDate          | BookedDate | AmendedDate | RejectedDate | CancelledDate | FilledDate          | Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
	| UKGovtBondSecondaryMarket     | 0       | 01/01/2018 09:30:00 |            |             |              |               | 01/01/2018 09:30:00 | MARKET | BUY       | GBX      |            | 10              | 5          | 5         |
	| UKGovtBondSecondaryMarket     | 0       | 01/01/2018 10:25:00 |            |             |              |               | 01/01/2018 10:25:00 | MARKET | BUY       | GBX      |            | 10              | 5          | 5         |
	And With the interday market data :
	| SecurityName | Epoch               | OpenPrice | ClosePrice | HighIntradayPrice | LowIntradayPrice | ListedSecurities | MarketCap | DailyVolume | Currency |
	| UKGovtBondSecondaryMarket     | 01/01/2018 | 10        | 11         | 11.5              | 10               | 10               | 1000000  | 1000       | GBX      |
	When I run the fixed income high volume rule
	Then I will have 1 fixed income high volume alerts

