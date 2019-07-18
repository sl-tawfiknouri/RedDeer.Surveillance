@highvolumevenuefilter
Feature: High Volume Venue Filter Parameters
	In order to meet MAR compliance requirements
	I need to be able to detect when traders are executing trades
	That are either within or outside of a venues trading activity
	For a given rule as a refining parameter

Background:
	Given I have the high volume venue filter parameter values
	| WindowHours | Min | Max |
	| 1           | 0.4 | 0.6 |

Scenario: Empty Universe yields no alerts
	Given I have the orders for a universe from 01/01/2018 to 03/01/2018 :
	| SecurityName | OrderId | PlacedDate          | BookedDate | AmendedDate | RejectedDate | CancelledDate | FilledDate          | Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
	When I run the high volume venue filter
	Then I will have 0 filter passed orders

Scenario: One Trade For Vodafone yields passed order with no market data
	Given I have the orders for a universe from 01/01/2018 to 03/01/2018 :
	| SecurityName | OrderId | PlacedDate          | BookedDate | AmendedDate | RejectedDate | CancelledDate | FilledDate          | Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
	| Vodafone     | 0       | 01/01/2018 09:30:00 |            |             |              |               | 01/01/2018 09:30:00 | MARKET | BUY       | GBX      |            | 100              | 1000          | 1000         |     
	When I run the high volume venue filter
	Then I will have 3 filter passed orders

Scenario: One Trade For Vodafone and within range yields one passed orders
	Given I have the orders for a universe from 01/01/2018 to 03/01/2018 :
	| SecurityName | OrderId | PlacedDate          | BookedDate | AmendedDate | RejectedDate | CancelledDate | FilledDate          | Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
	| Vodafone     | 0       | 01/01/2018 09:30:00 |            |             |              |               | 01/01/2018 09:30:00 | MARKET | BUY       | GBX      |            | 100              | 1000          | 1000         |     
	And With the intraday market data :
	| SecurityName | Epoch			     | Bid | Ask | Price | Currency | Volume |
	| Vodafone     | 01/01/2018  09:28:00| 1	  | 20  | 10    | GBX      | 1000  |
	| Vodafone     | 01/01/2018  09:29:00| 1	  | 20  | 10    | GBX      | 1000  |
	When I run the high volume venue filter
	Then I will have 3 filter passed orders

Scenario: Ten Trade For Vodafone and within range yields ten passed orders
	Given I have the orders for a universe from 01/01/2018 to 03/01/2018 :
	| SecurityName | OrderId | PlacedDate          | BookedDate | AmendedDate | RejectedDate | CancelledDate | FilledDate          | Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
	| Vodafone     | 0       | 01/01/2018 09:30:00 |            |             |              |               | 01/01/2018 09:30:00 | MARKET | BUY       | GBX      |            | 100              | 1000          | 100          |     
	| Vodafone     | 1       | 01/01/2018 09:30:00 |            |             |              |               | 01/01/2018 09:31:00 | MARKET | BUY       | GBX      |            | 100              | 1000          | 100          |     
	| Vodafone     | 2       | 01/01/2018 09:30:00 |            |             |              |               | 01/01/2018 09:32:00 | MARKET | BUY       | GBX      |            | 100              | 1000          | 100          |     
	| Vodafone     | 3       | 01/01/2018 09:30:00 |            |             |              |               | 01/01/2018 09:33:00 | MARKET | BUY       | GBX      |            | 100              | 1000          | 100          |     
	| Vodafone     | 4       | 01/01/2018 09:30:00 |            |             |              |               | 01/01/2018 09:34:00 | MARKET | BUY       | GBX      |            | 100              | 1000          | 100          |     
	| Vodafone     | 5       | 01/01/2018 09:30:00 |            |             |              |               | 01/01/2018 09:35:00 | MARKET | BUY       | GBX      |            | 100              | 1000          | 100          |     
	| Vodafone     | 6       | 01/01/2018 09:30:00 |            |             |              |               | 01/01/2018 09:36:00 | MARKET | BUY       | GBX      |            | 100              | 1000          | 100          |     
	| Vodafone     | 7       | 01/01/2018 09:30:00 |            |             |              |               | 01/01/2018 09:37:00 | MARKET | BUY       | GBX      |            | 100              | 1000          | 100          |     
	| Vodafone     | 8       | 01/01/2018 09:30:00 |            |             |              |               | 01/01/2018 09:38:00 | MARKET | BUY       | GBX      |            | 100              | 1000          | 100          |     
	| Vodafone     | 9       | 01/01/2018 09:30:00 |            |             |              |               | 01/01/2018 09:39:00 | MARKET | BUY       | GBX      |            | 100              | 1000          | 100          |     
	And With the intraday market data :
	| SecurityName | Epoch			     | Bid | Ask | Price | Currency | Volume |
	| Vodafone     | 01/01/2018  09:28:00| 1	  | 20  | 10    | GBX      | 1000  |
	| Vodafone     | 01/01/2018  09:29:00| 1	  | 20  | 10    | GBX      | 1000  |
	When I run the high volume venue filter
	Then I will have 30 filter passed orders

Scenario: Ten Trade For Vodafone and within range but trailing one over range yields ten passed orders
	Given I have the orders for a universe from 01/01/2018 to 03/01/2018 :
	| SecurityName | OrderId | PlacedDate          | BookedDate | AmendedDate | RejectedDate | CancelledDate | FilledDate          | Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
	| Vodafone     | 0       | 01/01/2018 09:30:00 |            |             |              |               | 01/01/2018 09:30:00 | MARKET | BUY       | GBX      |            | 100              | 1000          | 100          |     
	| Vodafone     | 1       | 01/01/2018 09:30:00 |            |             |              |               | 01/01/2018 09:31:00 | MARKET | BUY       | GBX      |            | 100              | 1000          | 100          |     
	| Vodafone     | 2       | 01/01/2018 09:30:00 |            |             |              |               | 01/01/2018 09:32:00 | MARKET | BUY       | GBX      |            | 100              | 1000          | 100          |     
	| Vodafone     | 3       | 01/01/2018 09:30:00 |            |             |              |               | 01/01/2018 09:33:00 | MARKET | BUY       | GBX      |            | 100              | 1000          | 100          |     
	| Vodafone     | 4       | 01/01/2018 09:30:00 |            |             |              |               | 01/01/2018 09:34:00 | MARKET | BUY       | GBX      |            | 100              | 1000          | 100          |     
	| Vodafone     | 5       | 01/01/2018 09:30:00 |            |             |              |               | 01/01/2018 09:35:00 | MARKET | BUY       | GBX      |            | 100              | 1000          | 100          |     
	| Vodafone     | 6       | 01/01/2018 09:30:00 |            |             |              |               | 01/01/2018 09:36:00 | MARKET | BUY       | GBX      |            | 100              | 1000          | 100          |     
	| Vodafone     | 7       | 01/01/2018 09:30:00 |            |             |              |               | 01/01/2018 09:37:00 | MARKET | BUY       | GBX      |            | 100              | 1000          | 100          |     
	| Vodafone     | 8       | 01/01/2018 09:30:00 |            |             |              |               | 01/01/2018 09:38:00 | MARKET | BUY       | GBX      |            | 100              | 1000          | 100          |     
	| Vodafone     | 9       | 01/01/2018 09:30:00 |            |             |              |               | 01/01/2018 09:39:00 | MARKET | BUY       | GBX      |            | 100              | 1000          | 100          |     
	| Vodafone     | 10      | 01/01/2018 09:30:00 |            |             |              |               | 01/01/2018 10:39:00 | MARKET | BUY       | GBX      |            | 100              | 1000          | 2000         |     
	And With the intraday market data :
	| SecurityName | Epoch			     | Bid | Ask | Price | Currency | Volume |
	| Vodafone     | 01/01/2018  09:28:00| 1	  | 20  | 10    | GBX      | 1000  |
	| Vodafone     | 01/01/2018  09:29:00| 1	  | 20  | 10    | GBX      | 1000  |
	| Vodafone     | 01/01/2018  09:45:00| 1	  | 20  | 10    | GBX      | 1000  |
	When I run the high volume venue filter
	Then I will have 30 filter passed orders
