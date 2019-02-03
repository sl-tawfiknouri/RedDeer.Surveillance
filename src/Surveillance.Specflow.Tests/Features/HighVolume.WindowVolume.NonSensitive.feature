Feature: HighVolume Window Volume Non Sensitive Parameters
	In order to meet MAR compliance requirements
	I need to be able to detect when traders are executing trades
	At a volume where they are able to exert market manipulating pressure
	on the prices the market is trading at
	By measuring their security trades relative to the window volume traded of the company

Background:
	Given I have the high volume rule parameter values
	| WindowHours | HighVolumePercentageDaily | HighVolumePercentageWindow | HighVolumePercentageMarketCap |  
	| 1           |				  		      |	0.1	                       | 			     			   |

@highvolume
@highvolumewindow
@highvolumewindownonsensitive
Scenario: Empty Universe yields no alerts
	Given I have the orders for a universe from 01/01/2018 to 03/01/2018 :
	| SecurityName | OrderId | PlacedDate          | BookedDate | AmendedDate | RejectedDate | CancelledDate | FilledDate  | Type | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
	When I run the high volume rule
	Then I will have 0 high volume alerts

@highvolume
@highvolumewindow
@highvolumewindownonsensitive
Scenario: One order at window volume yields one alert
	Given I have the orders for a universe from 01/01/2018 to 03/01/2018 :
	| SecurityName | OrderId | PlacedDate          | FilledDate          | Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
	| Vodafone     | 0       | 01/01/2018 09:30:00 | 01/01/2018 09:30:00 | MARKET | BUY       | GBX      |            | 10              | 1000          | 1000         |
	And With the intraday market data :
	| SecurityName | Epoch      | Bid | Ask | Price | Currency | Volume |
	| Vodafone     | 01/01/2018  09:30:00| 1	  | 20  | 10    | GBX      | 5000  |
	| Vodafone     | 01/01/2018  09:29:00| 1	  | 20  | 10    | GBX      | 5000  |
	When I run the high volume rule
	Then I will have 1 high volume alerts

@highvolume
@highvolumewindow
@highvolumewindownonsensitive
Scenario: One order below window volume yields zero alerts
	Given I have the orders for a universe from 01/01/2018 to 03/01/2018 :
	| SecurityName | OrderId | PlacedDate          | FilledDate          | Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
	| Vodafone     | 0       | 01/01/2018 09:30:00 | 01/01/2018 09:30:00 | MARKET | BUY       | GBX      |            | 10              | 999          | 999         |
	And With the intraday market data :
	| SecurityName | Epoch      | Bid | Ask | Price | Currency | Volume |
	| Vodafone     | 01/01/2018 09:30:00 | 1	  | 20  | 10    | GBX      | 10000  |
	When I run the high volume rule
	Then I will have 0 high volume alerts

@highvolume
@highvolumewindow
@highvolumewindownonsensitive
Scenario: One order above window volume yields one alerts
	Given I have the orders for a universe from 01/01/2018 to 03/01/2018 :
	| SecurityName | OrderId | PlacedDate          | FilledDate          | Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
	| Vodafone     | 0       | 01/01/2018 09:30:00 | 01/01/2018 09:30:00 | MARKET | BUY       | GBX      |            | 10              | 1001          | 1001         |
	And With the intraday market data :
	| SecurityName | Epoch      | Bid | Ask | Price | Currency | Volume |
	| Vodafone     | 01/01/2018 09:30:00 | 1	  | 20  | 10    | GBX      | 10000  |
	When I run the high volume rule
	Then I will have 1 high volume alerts

#ok so fahad has some scenarios which are on the window boundaries, if we do these once I think we can copy a bit?? maybe not with the different scenrios tbh and we will need to split a single order into two

@highvolume
@highvolumewindow
@highvolumewindownonsensitive
Scenario: Two orders split just outside of window at window volume yields zero alert
	Given I have the orders for a universe from 01/01/2018 to 03/01/2018 :
	| SecurityName | OrderId | PlacedDate          | FilledDate          | Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
	| Vodafone     | 0       | 01/01/2018 09:30:00 | 01/01/2018 09:30:00 | MARKET | BUY       | GBX      |            | 10              | 500          | 500         |
	| Vodafone     | 0       | 01/01/2018 10:35:00 | 01/01/2018 10:35:00 | MARKET | BUY       | GBX      |            | 10              | 500          | 500         |
	And With the intraday market data :
	| SecurityName | Epoch      | Bid | Ask | Price | Currency | Volume |
	| Vodafone     | 01/01/2018  09:30:00| 1	  | 20  | 10    | GBX      | 5000  |
	| Vodafone     | 01/01/2018  09:29:00| 1	  | 20  | 10    | GBX      | 5000  |
	When I run the high volume rule
	Then I will have 0 high volume alerts

@highvolume
@highvolumewindow
@highvolumewindownonsensitive
Scenario: Two orders split just on window at window volume yields one alert
	Given I have the orders for a universe from 01/01/2018 to 03/01/2018 :
	| SecurityName | OrderId | PlacedDate          | FilledDate          | Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
	| Vodafone     | 0       | 01/01/2018 09:30:00 | 01/01/2018 09:30:00 | MARKET | BUY       | GBX      |            | 10              | 500          | 500         |
	| Vodafone     | 0       | 01/01/2018 10:30:00 | 01/01/2018 10:30:00 | MARKET | BUY       | GBX      |            | 10              | 500          | 500         |
	And With the intraday market data :
	| SecurityName | Epoch      | Bid | Ask | Price | Currency | Volume |
	| Vodafone     | 01/01/2018  09:30:00| 1	  | 20  | 10    | GBX      | 5000  |
	| Vodafone     | 01/01/2018  09:29:00| 1	  | 20  | 10    | GBX      | 5000  |
	When I run the high volume rule
	Then I will have 1 high volume alerts

@highvolume
@highvolumewindow
@highvolumewindownonsensitive
Scenario: Two orders split just inside window at window volume yields one alert
	Given I have the orders for a universe from 01/01/2018 to 03/01/2018 :
	| SecurityName | OrderId | PlacedDate          | FilledDate          | Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
	| Vodafone     | 0       | 01/01/2018 09:30:00 | 01/01/2018 09:30:00 | MARKET | BUY       | GBX      |            | 10              | 500          | 500         |
	| Vodafone     | 0       | 01/01/2018 10:25:00 | 01/01/2018 10:25:00 | MARKET | BUY       | GBX      |            | 10              | 500          | 500         |
	And With the intraday market data :
	| SecurityName | Epoch      | Bid | Ask | Price | Currency | Volume |
	| Vodafone     | 01/01/2018  09:30:00| 1	  | 20  | 10    | GBX      | 5000  |
	| Vodafone     | 01/01/2018  09:29:00| 1	  | 20  | 10    | GBX      | 5000  |
	When I run the high volume rule
	Then I will have 1 high volume alerts