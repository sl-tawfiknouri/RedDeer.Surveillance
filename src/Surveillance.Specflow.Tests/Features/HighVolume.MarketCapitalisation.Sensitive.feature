Feature: HighVolume Market Capitalisation Sensitive Parameters
	In order to meet MAR compliance requirements
	I need to be able to detect when traders are executing trades
	At a volume where they are able to exert market manipulating pressure
	on the prices the market is trading at
	By measuring their security trades relative to the market cap of the underlying company

Background:
	Given I have the high volume rule parameter values
	| WindowHours | HighVolumePercentageDaily | HighVolumePercentageWindow | HighVolumePercentageMarketCap |  
	| 1           |							  |		                       | 0.01						   |

@highvolume
@highvolumemarketcap
@highvolumemarketcapsensitive
Scenario: Empty Universe yields no alerts
	Given I have the orders for a universe from 01/01/2018 to 03/01/2018 :
	| SecurityName | OrderId | PlacedDate          | BookedDate | AmendedDate | RejectedDate | CancelledDate | FilledDate          | Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
	When I run the high volume rule
	Then I will have 0 high volume alerts

@highvolume
@highvolumemarketcap
@highvolumemarketcapsensitive
Scenario: One order at market cap yields one alert
	Given I have the orders for a universe from 01/01/2018 to 03/01/2018 :
	| SecurityName | OrderId | PlacedDate          | BookedDate | AmendedDate | RejectedDate | CancelledDate | FilledDate          | Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
	| Vodafone     | 0       | 01/01/2018 09:30:00 |            |             |              |               | 01/01/2018 09:30:00 | MARKET | BUY       | GBX      |            | 10              | 1000          | 1000         |
	And With the interday market data :
	| SecurityName | Epoch               | OpenPrice | ClosePrice | HighIntradayPrice | LowIntradayPrice | ListedSecurities | MarketCap | DailyVolume | Currency |
	| Vodafone     | 01/01/2018 | 10        | 11         | 11.5              | 10               | 10               | 1000000  | 1000       | GBX      |
	When I run the high volume rule
	Then I will have 1 high volume alerts

@highvolume
@highvolumemarketcap
@highvolumemarketcapsensitive
Scenario: Two order one inside and one inside window at market cap yields one alert
	Given I have the high volume rule parameter values
	| WindowHours | HighVolumePercentageDaily | HighVolumePercentageWindow | HighVolumePercentageMarketCap |  
	| 25           |							  |		                       | 0.01						   |
	And I have the orders for a universe from 01/01/2018 to 03/01/2018 :
	| SecurityName | OrderId | PlacedDate          | BookedDate | AmendedDate | RejectedDate | CancelledDate | FilledDate          | Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
	| Vodafone     | 0       | 01/01/2018 15:30:00 |            |             |              |               | 01/01/2018 15:30:00 | MARKET | BUY       | GBX      |            | 10              | 500          | 500         |
	| Vodafone     | 0       | 01/02/2018 16:30:00 |            |             |              |               | 01/02/2018 16:30:00 | MARKET | BUY       | GBX      |            | 10              | 500          | 500         |
	And With the interday market data :
	| SecurityName | Epoch               | OpenPrice | ClosePrice | HighIntradayPrice | LowIntradayPrice | ListedSecurities | MarketCap | DailyVolume | Currency |
	| Vodafone     | 01/01/2018 | 10        | 11         | 11.5              | 10               | 10               | 1000000  | 1000       | GBX      |
	| Vodafone     | 01/02/2018 | 10        | 11         | 11.5              | 10               | 10               | 1000000  | 1000       | GBX      |
	When I run the high volume rule
	Then I will have 1 high volume alerts

@highvolume
@highvolumemarketcap
@highvolumemarketcapsensitive
Scenario: Two order one inside and one outside window at market cap yields zero alert
	Given I have the high volume rule parameter values
	| WindowHours | HighVolumePercentageDaily | HighVolumePercentageWindow | HighVolumePercentageMarketCap |  
	| 23           |							  |		                       | 0.01						   |
	And I have the orders for a universe from 01/01/2018 to 03/01/2018 :
	| SecurityName | OrderId | PlacedDate          | BookedDate | AmendedDate | RejectedDate | CancelledDate | FilledDate          | Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
	| Vodafone     | 0       | 01/01/2018 15:30:00 |            |             |              |               | 01/01/2018 15:30:00 | MARKET | BUY       | GBX      |            | 10              | 500          | 500         |
	| Vodafone     | 0       | 01/02/2018 16:30:00 |            |             |              |               | 01/02/2018 16:30:00 | MARKET | BUY       | GBX      |            | 10              | 500          | 500         |
	And With the interday market data :
	| SecurityName | Epoch               | OpenPrice | ClosePrice | HighIntradayPrice | LowIntradayPrice | ListedSecurities | MarketCap | DailyVolume | Currency |
	| Vodafone     | 01/01/2018 | 10        | 11         | 11.5              | 10               | 10               | 100000000  | 1000       | GBX      |
	| Vodafone     | 01/02/2018 | 10        | 11         | 11.5              | 10               | 10               | 100000000  | 1000       | GBX      |
	When I run the high volume rule
	Then I will have 0 high volume alerts

@highvolume
@highvolumemarketcap
@highvolumemarketcapsensitive
Scenario: Two order one inside and one outside trading hours at market cap yields one alert
	Given I have the orders for a universe from 01/01/2018 to 03/01/2018 :
	| SecurityName | OrderId | PlacedDate          | BookedDate | AmendedDate | RejectedDate | CancelledDate | FilledDate          | Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
	| Vodafone     | 0       | 01/01/2018 15:30:00 |            |             |              |               | 01/01/2018 15:30:00 | MARKET | BUY       | GBX      |            | 10              | 500          | 500         |
	| Vodafone     | 0       | 01/01/2018 16:30:00 |            |             |              |               | 01/01/2018 16:30:00 | MARKET | BUY       | GBX      |            | 10              | 500          | 500         |
	And With the interday market data :
	| SecurityName | Epoch               | OpenPrice | ClosePrice | HighIntradayPrice | LowIntradayPrice | ListedSecurities | MarketCap | DailyVolume | Currency |
	| Vodafone     | 01/01/2018 | 10        | 11         | 11.5              | 10               | 10               | 1000000  | 1000       | GBX      |
	When I run the high volume rule
	Then I will have 1 high volume alerts

@highvolume
@highvolumemarketcap
@highvolumemarketcapsensitive
Scenario: One order at market cap partially filled yields one alert
	Given I have the orders for a universe from 01/01/2018 to 03/01/2018 :
	| SecurityName | OrderId | PlacedDate          | BookedDate | AmendedDate | RejectedDate | CancelledDate | FilledDate          | Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
	| Vodafone     | 0       | 01/01/2018 09:30:00 |            |             |              |               | 01/01/2018 09:30:00 | MARKET | BUY       | GBX      |            | 10              | 10000          | 1000         |
	And With the interday market data :
	| SecurityName | Epoch               | OpenPrice | ClosePrice | HighIntradayPrice | LowIntradayPrice | ListedSecurities | MarketCap | DailyVolume | Currency |
	| Vodafone     | 01/01/2018 | 10        | 11         | 11.5              | 10               | 10               | 1000000  | 1000       | GBX      |
	When I run the high volume rule
	Then I will have 1 high volume alerts

@highvolume
@highvolumemarketcap
@highvolumemarketcapsensitive
Scenario: One order at low volume market cap yields one alert
	Given I have the orders for a universe from 01/01/2018 to 03/01/2018 :
	| SecurityName | OrderId | PlacedDate          | BookedDate | AmendedDate | RejectedDate | CancelledDate | FilledDate          | Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
	| Vodafone     | 0       | 01/01/2018 09:30:00 |            |             |              |               | 01/01/2018 09:30:00 | MARKET | BUY       | GBX      |            | 10              | 1          | 1         |
	And With the interday market data :
	| SecurityName | Epoch               | OpenPrice | ClosePrice | HighIntradayPrice | LowIntradayPrice | ListedSecurities | MarketCap | DailyVolume | Currency |
	| Vodafone     | 01/01/2018 | 10        | 11         | 11.5              | 10               | 10               | 1000  | 1000       | GBX      |
	When I run the high volume rule
	Then I will have 1 high volume alerts

@highvolume
@highvolumemarketcap
@highvolumemarketcapsensitive
Scenario: One order just buy at market cap yields one alert
	Given I have the orders for a universe from 01/01/2018 to 03/01/2018 :
	| SecurityName | OrderId | PlacedDate          | BookedDate | AmendedDate | RejectedDate | CancelledDate | FilledDate          | Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
	| Barclays     | 0       | 01/01/2018 09:30:00 |            |             |              |               | 01/01/2018 09:30:00 | MARKET | BUY       | GBX      |            | 10              | 1000          | 1000         |
	And With the interday market data :
	| SecurityName | Epoch               | OpenPrice | ClosePrice | HighIntradayPrice | LowIntradayPrice | ListedSecurities | MarketCap | DailyVolume | Currency |
	| Barclays     | 01/01/2018 | 10        | 11         | 11.5              | 10               | 10               | 1000000  | 1000       | GBX      |
	When I run the high volume rule
	Then I will have 1 high volume alerts

@highvolume
@highvolumemarketcap
@highvolumemarketcapsensitive
Scenario: One order just sell at market cap yields one alert
	Given I have the orders for a universe from 01/01/2018 to 03/01/2018 :
	| SecurityName | OrderId | PlacedDate          | BookedDate | AmendedDate | RejectedDate | CancelledDate | FilledDate          | Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
	| Barclays     | 0       | 01/01/2018 09:30:00 |            |             |              |               | 01/01/2018 09:30:00 | MARKET | SELL       | GBX      |            | 10              | 1000          | 1000         |
	And With the interday market data :
	| SecurityName | Epoch               | OpenPrice | ClosePrice | HighIntradayPrice | LowIntradayPrice | ListedSecurities | MarketCap | DailyVolume | Currency |
	| Barclays     | 01/01/2018 | 10        | 11         | 11.5              | 10               | 10               | 1000000  | 1000       | GBX      |
	When I run the high volume rule
	Then I will have 1 high volume alerts


@highvolumemarketcap
@highvolumemarketcapsensitive
Scenario: One order in different currency and exchange at market cap yields one alert
	Given I have the orders for a universe from 01/01/2018 to 03/01/2018 :
	| SecurityName | OrderId | PlacedDate          | BookedDate | AmendedDate | RejectedDate | CancelledDate | FilledDate          | Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
	| Nvidia     | 0       | 01/01/2018 09:30:00 |            |             |              |               | 01/01/2018 09:30:00 | MARKET | BUY       | USD      |            | 10              | 1000          | 1000         |
	And With the interday market data :
	| SecurityName | Epoch               | OpenPrice | ClosePrice | HighIntradayPrice | LowIntradayPrice | ListedSecurities | MarketCap | DailyVolume | Currency |
	| Nvidia     | 01/01/2018 | 10        | 11         | 11.5              | 10               | 10               | 1000000  | 1000       | USD      |
	When I run the high volume rule
	Then I will have 1 high volume alerts

@highvolume
@highvolumemarketcap
@highvolumemarketcapsensitive
Scenario: One order below market cap yields zero alerts
	Given I have the orders for a universe from 01/01/2018 to 03/01/2018 :
	| SecurityName | OrderId | PlacedDate          | BookedDate | AmendedDate | RejectedDate | CancelledDate | FilledDate          | Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
	| Vodafone     | 0       | 01/01/2018 09:30:00 |            |             |              |               | 01/01/2018 09:30:00 | MARKET | BUY       | GBX      |            | 10              | 999          | 999         |
	And With the interday market data :
	| SecurityName | Epoch               | OpenPrice | ClosePrice | HighIntradayPrice | LowIntradayPrice | ListedSecurities | MarketCap | DailyVolume | Currency |
	| Vodafone     | 01/01/2018 | 10        | 11         | 11.5              | 10               | 10               | 100000000  | 1000       | GBX      |
	When I run the high volume rule
	Then I will have 0 high volume alerts


@highvolume
@highvolumemarketcap
@highvolumemarketcapsensitive
Scenario: One order above market cap yields one alerts
	Given I have the orders for a universe from 01/01/2018 to 03/01/2018 :
	| SecurityName | OrderId | PlacedDate          | BookedDate | AmendedDate | RejectedDate | CancelledDate | FilledDate          | Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
	| Vodafone     | 0       | 01/01/2018 09:30:00 |            |             |              |               | 01/01/2018 09:30:00 | MARKET | BUY       | GBX      |            | 10              | 1001          | 1000         |
	And With the interday market data :
	| SecurityName | Epoch               | OpenPrice | ClosePrice | HighIntradayPrice | LowIntradayPrice | ListedSecurities | MarketCap | DailyVolume | Currency |
	| Vodafone     | 01/01/2018 | 10        | 11         | 11.5              | 10               | 10               | 1000000  | 1000       | GBX      |
	When I run the high volume rule
	Then I will have 1 high volume alerts


@highvolume
@highvolumemarketcap
@highvolumemarketcapsensitive
Scenario: Two order at market cap at window yields one alert
	Given I have the orders for a universe from 01/01/2018 to 03/01/2018 :
	| SecurityName | OrderId | PlacedDate          | BookedDate | AmendedDate | RejectedDate | CancelledDate | FilledDate          | Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
	| Vodafone     | 0       | 01/01/2018 09:30:00 |            |             |              |               | 01/01/2018 09:30:00 | MARKET | BUY       | GBX      |            | 10              | 500          | 500         |
	| Vodafone     | 0       | 01/01/2018 10:30:00 |            |             |              |               | 01/01/2018 10:30:00 | MARKET | BUY       | GBX      |            | 10              | 500          | 500         |
	And With the interday market data :
	| SecurityName | Epoch      | OpenPrice | ClosePrice | HighIntradayPrice | LowIntradayPrice | ListedSecurities | MarketCap | DailyVolume | Currency |
	| Vodafone     | 01/01/2018 | 10        | 11         | 11.5              | 10               | 10               | 1000000  | 1000       | GBX      |
	When I run the high volume rule
	Then I will have 1 high volume alerts

@highvolume
@highvolumemarketcap
@highvolumemarketcapsensitive
Scenario: Two order at market cap but just outside window yields zero alerts
	Given I have the orders for a universe from 01/01/2018 to 03/01/2018 :
	| SecurityName | OrderId | PlacedDate          | BookedDate | AmendedDate | RejectedDate | CancelledDate | FilledDate          | Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
	| Vodafone     | 0       | 01/01/2018 09:30:00 |            |             |              |               | 01/01/2018 09:30:00 | MARKET | BUY       | GBX      |            | 10              | 500          | 500         |
	| Vodafone     | 0       | 01/01/2018 10:31:00 |            |             |              |               | 01/01/2018 10:31:00 | MARKET | BUY       | GBX      |            | 10              | 500          | 500         |
	And With the interday market data :
	| SecurityName | Epoch      | OpenPrice | ClosePrice | HighIntradayPrice | LowIntradayPrice | ListedSecurities | MarketCap | DailyVolume | Currency |
	| Vodafone     | 01/01/2018 | 10        | 11         | 11.5              | 10               | 10               | 100000000  | 1000       | GBX      |
	When I run the high volume rule
	Then I will have 0 high volume alerts

@highvolume
@highvolumemarketcap
@highvolumemarketcapsensitive
Scenario: Two order at market cap and inside window yields one alert
	Given I have the orders for a universe from 01/01/2018 to 03/01/2018 :
	| SecurityName | OrderId | PlacedDate          | BookedDate | AmendedDate | RejectedDate | CancelledDate | FilledDate          | Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
	| Vodafone     | 0       | 01/01/2018 09:30:00 |            |             |              |               | 01/01/2018 09:30:00 | MARKET | BUY       | GBX      |            | 10              | 500          | 500         |
	| Vodafone     | 0       | 01/01/2018 10:25:00 |            |             |              |               | 01/01/2018 10:25:00 | MARKET | BUY       | GBX      |            | 10              | 500          | 500         |
	And With the interday market data :
	| SecurityName | Epoch      | OpenPrice | ClosePrice | HighIntradayPrice | LowIntradayPrice | ListedSecurities | MarketCap | DailyVolume | Currency |
	| Vodafone     | 01/01/2018 | 10        | 11         | 11.5              | 10               | 10               | 1000000  | 1000       | GBX      |
	When I run the high volume rule
	Then I will have 1 high volume alerts