﻿Feature: HighVolume Market Capitalisation Non Sensitive Parameters
	In order to meet MAR compliance requirements
	I need to be able to detect when traders are executing trades
	At a volume where they are able to exert market manipulating pressure
	on the prices the market is trading at
	By measuring their security trades relative to the market cap of the underlying company

Background:
	Given I have the high volume rule parameter values
	| WindowHours | HighVolumePercentageDaily | HighVolumePercentageWindow | HighVolumePercentageMarketCap |  
	| 1           |							  |		                       | 0.2						   |

@highvolume
@highvolumemarketcap
@highvolumemarketcapnonsensitive
Scenario: Empty Universe yields no alerts
	Given I have the orders for a universe from 01/01/2018 to 03/01/2018 :
	| SecurityName | OrderId | PlacedDate          | BookedDate | AmendedDate | RejectedDate | CancelledDate | FilledDate          | Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
	When I run the high volume rule
	Then I will have 0 high volume alerts

@highvolume
@highvolumemarketcap
@highvolumemarketcapnonsensitive
Scenario: One order at market cap yields one alert
	Given I have the orders for a universe from 01/01/2018 to 03/01/2018 :
	| SecurityName | OrderId | PlacedDate          | BookedDate | AmendedDate | RejectedDate | CancelledDate | FilledDate          | Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
	| Vodafone     | 0       | 01/01/2018 09:30:00 |            |             |              |               | 01/01/2018 09:30:00 | MARKET | BUY       | GBX      |            | 100              | 10000          | 10000         |
	And With the interday market data :
	| SecurityName | Epoch               | OpenPrice | ClosePrice | HighIntradayPrice | LowIntradayPrice | ListedSecurities | MarketCap | DailyVolume | Currency |
	| Vodafone     | 01/01/2018 | 10        | 11         | 11.5              | 10               | 10               | 1000000  | 1000       | GBX      |
	When I run the high volume rule
	Then I will have 1 high volume alerts

@highvolume
@highvolumemarketcap
@highvolumemarketcapnonsensitive
Scenario: Two order one inside and one inside window but next day at market cap yields one alert
	Given I have the high volume rule parameter values
	| WindowHours | HighVolumePercentageDaily | HighVolumePercentageWindow | HighVolumePercentageMarketCap |  
	| 25           |							  |		                       | 0.2						   |
	And I have the orders for a universe from 01/01/2018 to 03/01/2018 :
	| SecurityName | OrderId | PlacedDate          | BookedDate | AmendedDate | RejectedDate | CancelledDate | FilledDate          | Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
	| Vodafone     | 0       | 01/01/2018 15:30:00 |            |             |              |               | 01/01/2018 15:30:00 | MARKET | BUY       | GBX      |            | 10              | 10000          | 10000         |
	| Vodafone     | 1       | 01/02/2018 16:30:00 |            |             |              |               | 01/02/2018 16:30:00 | MARKET | BUY       | GBX      |            | 10              | 10000          | 10000         |
	And With the interday market data :
	| SecurityName | Epoch               | OpenPrice | ClosePrice | HighIntradayPrice | LowIntradayPrice | ListedSecurities | MarketCap | DailyVolume | Currency |
	| Vodafone     | 01/01/2018 | 10        | 11         | 11.5              | 10               | 10               | 1000000  | 1000       | GBX      |
	| Vodafone     | 01/02/2018 | 10        | 11         | 11.5              | 10               | 10               | 1000000  | 1000       | GBX      |
	When I run the high volume rule
	Then I will have 1 high volume alerts

@highvolume
@highvolumemarketcap
@highvolumemarketcapnonsensitive
Scenario: Two order one inside and one outside window and next day at market cap yields zero alert
	Given I have the high volume rule parameter values
	| WindowHours | HighVolumePercentageDaily | HighVolumePercentageWindow | HighVolumePercentageMarketCap |  
	| 23           |							  |		                       | 0.2						   |
	And I have the orders for a universe from 01/01/2018 to 03/01/2018 :
	| SecurityName | OrderId | PlacedDate          | BookedDate | AmendedDate | RejectedDate | CancelledDate | FilledDate          | Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
	| Vodafone     | 0       | 01/01/2018 15:30:00 |            |             |              |               | 01/01/2018 15:30:00 | MARKET | BUY       | GBX      |            | 10              | 10000          | 10000         |
	| Vodafone     | 1       | 01/02/2018 16:30:00 |            |             |              |               | 01/02/2018 16:30:00 | MARKET | BUY       | GBX      |            | 10              | 10000          | 10000         |
	And With the interday market data :
	| SecurityName | Epoch               | OpenPrice | ClosePrice | HighIntradayPrice | LowIntradayPrice | ListedSecurities | MarketCap | DailyVolume | Currency |
	| Vodafone     | 01/01/2018 | 10        | 11         | 11.5              | 10               | 10               | 100000000  | 1000       | GBX      |
	| Vodafone     | 01/02/2018 | 10        | 11         | 11.5              | 10               | 10               | 100000000  | 1000       | GBX      |
	When I run the high volume rule
	Then I will have 0 high volume alerts

@highvolume
@highvolumemarketcap
@highvolumemarketcapnonsensitive
Scenario: Two order one inside and one outside trading hours at market cap yields one alert
	Given I have the orders for a universe from 01/01/2018 to 03/01/2018 :
	| SecurityName | OrderId | PlacedDate          | BookedDate | AmendedDate | RejectedDate | CancelledDate | FilledDate          | Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
	| Vodafone     | 0       | 01/01/2018 15:30:00 |            |             |              |               | 01/01/2018 15:30:00 | MARKET | BUY       | GBX      |            | 10              | 10000          | 10000         |
	| Vodafone     | 1       | 01/01/2018 16:30:00 |            |             |              |               | 01/01/2018 16:30:00 | MARKET | BUY       | GBX      |            | 10              | 10000          | 10000         |
	And With the interday market data :
	| SecurityName | Epoch               | OpenPrice | ClosePrice | HighIntradayPrice | LowIntradayPrice | ListedSecurities | MarketCap | DailyVolume | Currency |
	| Vodafone     | 01/01/2018 | 10        | 11         | 11.5              | 10               | 10               | 1000000  | 1000       | GBX      |
	When I run the high volume rule
	Then I will have 1 high volume alerts

@highvolume
@highvolumemarketcap
@highvolumemarketcapnonsensitive
Scenario: One order at market cap partially filled yields one alert
	Given I have the orders for a universe from 01/01/2018 to 03/01/2018 :
	| SecurityName | OrderId | PlacedDate          | BookedDate | AmendedDate | RejectedDate | CancelledDate | FilledDate          | Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
	| Vodafone     | 0       | 01/01/2018 09:30:00 |            |             |              |               | 01/01/2018 09:30:00 | MARKET | BUY       | GBX      |            | 10              | 200000          | 20000         |
	And With the interday market data :
	| SecurityName | Epoch               | OpenPrice | ClosePrice | HighIntradayPrice | LowIntradayPrice | ListedSecurities | MarketCap | DailyVolume | Currency |
	| Vodafone     | 01/01/2018 | 10        | 11         | 11.5              | 10               | 10               | 1000000  | 1000       | GBX      |
	When I run the high volume rule
	Then I will have 1 high volume alerts

@highvolume
@highvolumemarketcap
@highvolumemarketcapnonsensitive
Scenario: One order at low volume market cap yields one alert
	Given I have the orders for a universe from 01/01/2018 to 03/01/2018 :
	| SecurityName | OrderId | PlacedDate          | BookedDate | AmendedDate | RejectedDate | CancelledDate | FilledDate          | Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
	| Vodafone     | 0       | 01/01/2018 09:30:00 |            |             |              |               | 01/01/2018 09:30:00 | MARKET | BUY       | GBX      |            | 10              | 2          | 2         |
	And With the interday market data :
	| SecurityName | Epoch               | OpenPrice | ClosePrice | HighIntradayPrice | LowIntradayPrice | ListedSecurities | MarketCap | DailyVolume | Currency |
	| Vodafone     | 01/01/2018 | 10        | 11         | 11.5              | 10               | 10               | 10  | 100       | GBX      |
	When I run the high volume rule
	Then I will have 1 high volume alerts


@highvolume
@highvolumemarketcap
@highvolumemarketcapnonsensitive
Scenario: One order just buy at market cap yields one alert
	Given I have the orders for a universe from 01/01/2018 to 03/01/2018 :
	| SecurityName | OrderId | PlacedDate          | BookedDate | AmendedDate | RejectedDate | CancelledDate | FilledDate          | Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
	| Barclays     | 0       | 01/01/2018 09:30:00 |            |             |              |               | 01/01/2018 09:30:00 | MARKET | BUY       | GBX      |            | 10              | 20000          | 20000         |
	And With the interday market data :
	| SecurityName | Epoch               | OpenPrice | ClosePrice | HighIntradayPrice | LowIntradayPrice | ListedSecurities | MarketCap | DailyVolume | Currency |
	| Barclays     | 01/01/2018 | 10        | 11         | 11.5              | 10               | 10               | 1000000  | 1000       | GBX      |
	When I run the high volume rule
	Then I will have 1 high volume alerts

@highvolume
@highvolumemarketcap
@highvolumemarketcapnonsensitive
Scenario: One order just sell at market cap yields one alert
	Given I have the orders for a universe from 01/01/2018 to 03/01/2018 :
	| SecurityName | OrderId | PlacedDate          | BookedDate | AmendedDate | RejectedDate | CancelledDate | FilledDate          | Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
	| Barclays     | 0       | 01/01/2018 09:30:00 |            |             |              |               | 01/01/2018 09:30:00 | MARKET | SELL       | GBX      |            | 10              | 20000          | 20000         |
	And With the interday market data :
	| SecurityName | Epoch               | OpenPrice | ClosePrice | HighIntradayPrice | LowIntradayPrice | ListedSecurities | MarketCap | DailyVolume | Currency |
	| Barclays     | 01/01/2018 | 10        | 11         | 11.5              | 10               | 10               | 1000000  | 1000       | GBX      |
	When I run the high volume rule
	Then I will have 1 high volume alerts

@highvolume
@highvolumemarketcap
@highvolumemarketcapnonsensitive
Scenario: One order in different currency and exchange at market cap yields one alert
	Given I have the orders for a universe from 01/01/2018 to 03/01/2018 :
	| SecurityName | OrderId | PlacedDate          | BookedDate | AmendedDate | RejectedDate | CancelledDate | FilledDate          | Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
	| Nvidia     | 0       | 01/01/2018 09:30:00 |            |             |              |               | 01/01/2018 09:30:00 | MARKET | BUY       | USD      |            | 10              | 20000          | 20000         |
	And With the interday market data :
	| SecurityName | Epoch               | OpenPrice | ClosePrice | HighIntradayPrice | LowIntradayPrice | ListedSecurities | MarketCap | DailyVolume | Currency |
	| Nvidia     | 01/01/2018 | 10        | 11         | 11.5              | 10               | 10               | 1000000  | 1000       | USD      |
	When I run the high volume rule
	Then I will have 1 high volume alerts

@highvolume
@highvolumemarketcap
@highvolumemarketcapnonsensitive
Scenario: One order below market cap yields zero alerts
	Given I have the orders for a universe from 01/01/2018 to 03/01/2018 :
	| SecurityName | OrderId | PlacedDate          | BookedDate | AmendedDate | RejectedDate | CancelledDate | FilledDate          | Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
	| Vodafone     | 0       | 01/01/2018 09:30:00 |            |             |              |               | 01/01/2018 09:30:00 | MARKET | BUY       | GBX      |            | 10              | 19999          | 19999         |
	And With the interday market data :
	| SecurityName | Epoch               | OpenPrice | ClosePrice | HighIntradayPrice | LowIntradayPrice | ListedSecurities | MarketCap | DailyVolume | Currency |
	| Vodafone     | 01/01/2018 | 10        | 11         | 11.5              | 10               | 10               | 100000000  | 1000       | GBX      |
	When I run the high volume rule
	Then I will have 0 high volume alerts


@highvolume
@highvolumemarketcap
@highvolumemarketcapnonsensitive
Scenario: One order above market cap yields one alerts
	Given I have the orders for a universe from 01/01/2018 to 03/01/2018 :
	| SecurityName | OrderId | PlacedDate          | BookedDate | AmendedDate | RejectedDate | CancelledDate | FilledDate          | Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
	| Vodafone     | 0       | 01/01/2018 09:30:00 |            |             |              |               | 01/01/2018 09:30:00 | MARKET | BUY       | GBX      |            | 10              | 20001          | 20001         |
	And With the interday market data :
	| SecurityName | Epoch               | OpenPrice | ClosePrice | HighIntradayPrice | LowIntradayPrice | ListedSecurities | MarketCap | DailyVolume | Currency |
	| Vodafone     | 01/01/2018 | 10        | 11         | 11.5              | 10               | 10               | 1000000  | 1000       | GBX      |
	When I run the high volume rule
	Then I will have 1 high volume alerts



@highvolume
@highvolumemarketcap
@highvolumemarketcapnonsensitive
Scenario: Two order at market cap at window yields one alert
	Given I have the orders for a universe from 01/01/2018 to 03/01/2018 :
	| SecurityName | OrderId | PlacedDate          | BookedDate | AmendedDate | RejectedDate | CancelledDate | FilledDate          | Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
	| Vodafone     | 0       | 01/01/2018 09:30:00 |            |             |              |               | 01/01/2018 09:30:00 | MARKET | BUY       | GBX      |            | 10              | 10000          | 10000         |
	| Vodafone     | 0       | 01/01/2018 10:30:00 |            |             |              |               | 01/01/2018 10:30:00 | MARKET | BUY       | GBX      |            | 10              | 10000          | 10000         |
	And With the interday market data :
	| SecurityName | Epoch      | OpenPrice | ClosePrice | HighIntradayPrice | LowIntradayPrice | ListedSecurities | MarketCap | DailyVolume | Currency |
	| Vodafone     | 01/01/2018 | 10        | 11         | 11.5              | 10               | 10               | 1000000  | 1000       | GBX      |
	When I run the high volume rule
	Then I will have 1 high volume alerts

@highvolume
@highvolumemarketcap
@highvolumemarketcapnonsensitive
Scenario: Two order at market cap but just outside window yields zero alerts
	Given I have the orders for a universe from 01/01/2018 to 03/01/2018 :
	| SecurityName | OrderId | PlacedDate          | BookedDate | AmendedDate | RejectedDate | CancelledDate | FilledDate          | Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
	| Vodafone     | 0       | 01/01/2018 09:30:00 |            |             |              |               | 01/01/2018 09:30:00 | MARKET | BUY       | GBX      |            | 10              | 10000          | 10000         |
	| Vodafone     | 0       | 01/01/2018 10:31:00 |            |             |              |               | 01/01/2018 10:31:00 | MARKET | BUY       | GBX      |            | 10              | 10000          | 10000         |
	And With the interday market data :
	| SecurityName | Epoch      | OpenPrice | ClosePrice | HighIntradayPrice | LowIntradayPrice | ListedSecurities | MarketCap | DailyVolume | Currency |
	| Vodafone     | 01/01/2018 | 10        | 11         | 11.5              | 10               | 10               | 100000000  | 1000       | GBX      |
	When I run the high volume rule
	Then I will have 0 high volume alerts

@highvolume
@highvolumemarketcap
@highvolumemarketcapnonsensitive
Scenario: Two order at market cap and inside window yields one alert
	Given I have the orders for a universe from 01/01/2018 to 03/01/2018 :
	| SecurityName | OrderId | PlacedDate          | BookedDate | AmendedDate | RejectedDate | CancelledDate | FilledDate          | Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
	| Vodafone     | 0       | 01/01/2018 09:30:00 |            |             |              |               | 01/01/2018 09:30:00 | MARKET | BUY       | GBX      |            | 10              | 10000          | 10000         |
	| Vodafone     | 0       | 01/01/2018 10:25:00 |            |             |              |               | 01/01/2018 10:25:00 | MARKET | BUY       | GBX      |            | 10              | 10000          | 10000         |
	And With the interday market data :
	| SecurityName | Epoch      | OpenPrice | ClosePrice | HighIntradayPrice | LowIntradayPrice | ListedSecurities | MarketCap | DailyVolume | Currency |
	| Vodafone     | 01/01/2018 | 10        | 11         | 11.5              | 10               | 10               | 1000000  | 1000       | GBX      |
	When I run the high volume rule
	Then I will have 1 high volume alerts