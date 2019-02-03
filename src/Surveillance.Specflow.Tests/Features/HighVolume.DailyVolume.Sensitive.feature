Feature: HighVolume Daily Volume Sensitive Parameters
	In order to meet MAR compliance requirements
	I need to be able to detect when traders are executing trades
	At a volume where they are able to exert market manipulating pressure
	on the prices the market is trading at
	By measuring their security trades relative to the daily volume traded of the underlying company

Background:
	Given I have the high volume rule parameter values
	| WindowHours | HighVolumePercentageDaily | HighVolumePercentageWindow | HighVolumePercentageMarketCap |  
	| 1           |	0.01					  |		                       | 			     			   |

@highvolume
@highvolumeDaily
@highvolumeDailySensitive
Scenario: Empty Universe yields no alerts
	Given I have the orders for a universe from 01/01/2018 to 03/01/2018 :
	| SecurityName | OrderId | PlacedDate          | BookedDate | AmendedDate | RejectedDate | CancelledDate | FilledDate          | Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
	When I run the high volume rule
	Then I will have 0 high volume alerts

@highvolume
@highvolumeDaily
@highvolumeDailySensitive
Scenario: One order at daily volume yields one alert
	Given I have the orders for a universe from 01/01/2018 to 03/01/2018 :
	| SecurityName | OrderId | PlacedDate          | BookedDate | AmendedDate | RejectedDate | CancelledDate | FilledDate          | Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
	| Vodafone     | 0       | 01/01/2018 09:30:00 |            |             |              |               | 01/01/2018 09:30:00 | MARKET | BUY       | GBX      |            | 10              | 10          | 10         |
	And With the interday market data :
	| SecurityName | Epoch               | OpenPrice | ClosePrice | HighIntradayPrice | LowIntradayPrice | ListedSecurities | MarketCap | DailyVolume | Currency |
	| Vodafone     | 01/01/2018 | 10        | 11         | 11.5              | 10               | 10               | 1000000  | 1000       | GBX      |
	When I run the high volume rule
	Then I will have 1 high volume alerts

@highvolume
@highvolumeDaily
@highvolumeDailySensitive
Scenario: One order below daily volume yields zero alerts
	Given I have the orders for a universe from 01/01/2018 to 03/01/2018 :
	| SecurityName | OrderId | PlacedDate          | BookedDate | AmendedDate | RejectedDate | CancelledDate | FilledDate          | Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
	| Vodafone     | 0       | 01/01/2018 09:30:00 |            |             |              |               | 01/01/2018 09:30:00 | MARKET | BUY       | GBX      |            | 10              | 9          | 9         |
	And With the interday market data :
	| SecurityName | Epoch               | OpenPrice | ClosePrice | HighIntradayPrice | LowIntradayPrice | ListedSecurities | MarketCap | DailyVolume | Currency |
	| Vodafone     | 01/01/2018 | 10        | 11         | 11.5              | 10               | 10               | 1000000  | 1000       | GBX      |
	When I run the high volume rule
	Then I will have 0 high volume alerts

@highvolume
@highvolumeDaily
@highvolumeDailySensitive
Scenario: One order above daily volume yields one alerts
	Given I have the orders for a universe from 01/01/2018 to 03/01/2018 :
	| SecurityName | OrderId | PlacedDate          | BookedDate | AmendedDate | RejectedDate | CancelledDate | FilledDate          | Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
	| Vodafone     | 0       | 01/01/2018 09:30:00 |            |             |              |               | 01/01/2018 09:30:00 | MARKET | BUY       | GBX      |            | 10              | 11          | 11         |
	And With the interday market data :
	| SecurityName | Epoch               | OpenPrice | ClosePrice | HighIntradayPrice | LowIntradayPrice | ListedSecurities | MarketCap | DailyVolume | Currency |
	| Vodafone     | 01/01/2018 | 10        | 11         | 11.5              | 10               | 10               | 1000000  | 1000       | GBX      |
	When I run the high volume rule
	Then I will have 1 high volume alerts
	

@highvolume
@highvolumeDaily
@highvolumeDailySensitive
Scenario: Two order at daily volume on exact window yields one alert
	Given I have the orders for a universe from 01/01/2018 to 03/01/2018 :
	| SecurityName | OrderId | PlacedDate          | BookedDate | AmendedDate | RejectedDate | CancelledDate | FilledDate          | Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
	| Vodafone     | 0       | 01/01/2018 09:30:00 |            |             |              |               | 01/01/2018 09:30:00 | MARKET | BUY       | GBX      |            | 10              | 5          | 5         |
	| Vodafone     | 0       | 01/01/2018 10:30:00 |            |             |              |               | 01/01/2018 10:30:00 | MARKET | BUY       | GBX      |            | 10              | 5          | 5         |
	And With the interday market data :
	| SecurityName | Epoch               | OpenPrice | ClosePrice | HighIntradayPrice | LowIntradayPrice | ListedSecurities | MarketCap | DailyVolume | Currency |
	| Vodafone     | 01/01/2018 | 10        | 11         | 11.5              | 10               | 10               | 1000000  | 1000       | GBX      |
	When I run the high volume rule
	Then I will have 1 high volume alerts

@highvolume
@highvolumeDaily
@highvolumeDailySensitive
Scenario: Two order at daily volume but outside of window yields zero alert
	Given I have the orders for a universe from 01/01/2018 to 03/01/2018 :
	| SecurityName | OrderId | PlacedDate          | BookedDate | AmendedDate | RejectedDate | CancelledDate | FilledDate          | Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
	| Vodafone     | 0       | 01/01/2018 09:30:00 |            |             |              |               | 01/01/2018 09:30:00 | MARKET | BUY       | GBX      |            | 10              | 5          | 5         |
	| Vodafone     | 0       | 01/01/2018 10:35:00 |            |             |              |               | 01/01/2018 10:35:00 | MARKET | BUY       | GBX      |            | 10              | 5          | 5         |
	And With the interday market data :
	| SecurityName | Epoch               | OpenPrice | ClosePrice | HighIntradayPrice | LowIntradayPrice | ListedSecurities | MarketCap | DailyVolume | Currency |
	| Vodafone     | 01/01/2018 | 10        | 11         | 11.5              | 10               | 10               | 1000000  | 1000       | GBX      |
	When I run the high volume rule
	Then I will have 0 high volume alerts

@highvolume
@highvolumeDaily
@highvolumeDailySensitive
Scenario: Two order at daily volume and inside of window yields one alert
	Given I have the orders for a universe from 01/01/2018 to 03/01/2018 :
	| SecurityName | OrderId | PlacedDate          | BookedDate | AmendedDate | RejectedDate | CancelledDate | FilledDate          | Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
	| Vodafone     | 0       | 01/01/2018 09:30:00 |            |             |              |               | 01/01/2018 09:30:00 | MARKET | BUY       | GBX      |            | 10              | 5          | 5         |
	| Vodafone     | 0       | 01/01/2018 10:25:00 |            |             |              |               | 01/01/2018 10:25:00 | MARKET | BUY       | GBX      |            | 10              | 5          | 5         |
	And With the interday market data :
	| SecurityName | Epoch               | OpenPrice | ClosePrice | HighIntradayPrice | LowIntradayPrice | ListedSecurities | MarketCap | DailyVolume | Currency |
	| Vodafone     | 01/01/2018 | 10        | 11         | 11.5              | 10               | 10               | 1000000  | 1000       | GBX      |
	When I run the high volume rule
	Then I will have 1 high volume alerts