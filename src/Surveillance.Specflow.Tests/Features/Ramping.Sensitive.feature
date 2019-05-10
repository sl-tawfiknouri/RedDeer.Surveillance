@ramping
@rampingsensitive
Feature: Ramping Sensitive Parameters
		In order to meet MAR compliance requirements
		I need to be able to detect when traders are placing orders
		which are designed to control the price of a security in the market

Background:
			Given I have the ramping rule parameter values
			| WindowHours | AutoCorrelationCoefficient | ThresholdOrdersExecutedInWindow | ThresholdVolumePercentageWindow | 
			| 1           | 0.7                        | 3                               |                                 |
		
Scenario: Empty Universe yields no alerts
		 Given I have the orders for a universe from 01/01/2018 to 01/05/2018 :
         | SecurityName | OrderId | PlacedDate | FilledDate | Type | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
         When I run the ramping rule
		 Then I will have 0 ramping alerts

Scenario: Ramping with increasing prices matching increasing order fill values raises 2 alerts
		Given I have the orders for a universe from 01/01/2019 to 01/01/2019 :
         | SecurityName | OrderId | PlacedDate			| FilledDate			| Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
         | Barclays     | 1       | 01/01/2019 13:50:00 | 01/01/2019 13:50:00   | Market | Buy       | GBX      |            | 100              | 500           | 500          |
         | Barclays     | 2       | 01/01/2019 13:51:00 | 01/01/2019 13:51:00   | Market | Buy       | GBX      |            | 101              | 500           | 500          |
         | Barclays     | 3       | 01/01/2019 13:52:00 | 01/01/2019 13:52:00   | Market | Buy       | GBX      |            | 102              | 500           | 500          |
         | Barclays     | 4       | 01/01/2019 13:53:00 | 01/01/2019 13:53:00   | Market | Buy       | GBX      |            | 103              | 500           | 500          |
         | Barclays     | 5       | 01/01/2019 13:54:00 | 01/01/2019 13:54:00   | Market | Buy       | GBX      |            | 104              | 500           | 500          |
		And With the intraday market data :
		| SecurityName | Epoch			      | Bid | Ask | Price	 | Currency | Volume |
		| Barclays     | 01/01/2019  13:50:00 | 1	  | 20  | 100    | GBX      | 5000  |
		| Barclays     | 01/01/2019  13:51:00 | 1	  | 20  | 101    | GBX      | 5000  |
		| Barclays     | 01/01/2019  13:52:00 | 1	  | 20  | 102    | GBX      | 5000  |
		| Barclays     | 01/01/2019  13:53:00 | 1	  | 20  | 103    | GBX      | 5000  |
		| Barclays     | 01/01/2019  13:54:00 | 1	  | 20  | 104    | GBX      | 5000  |
		| Barclays     | 01/01/2019  13:55:00 | 1	  | 20  | 105    | GBX      | 5000  |
		 When I run the ramping rule
		 Then I will have 2 ramping alerts

Scenario: Ramping with increasing prices matching increasing order fill values raises 7 alerts
		Given I have the orders for a universe from 01/01/2019 to 01/01/2019 :
         | SecurityName | OrderId | PlacedDate			| FilledDate			| Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
         | Barclays     | 1       | 01/01/2019 13:50:00 | 01/01/2019 13:50:00   | Market | Buy       | GBX      |            | 100              | 500           | 500          |
         | Barclays     | 2       | 01/01/2019 13:51:00 | 01/01/2019 13:51:00   | Market | Buy       | GBX      |            | 101              | 500           | 500          |
         | Barclays     | 3       | 01/01/2019 13:52:00 | 01/01/2019 13:52:00   | Market | Buy       | GBX      |            | 102              | 500           | 500          |
         | Barclays     | 4       | 01/01/2019 13:53:00 | 01/01/2019 13:53:00   | Market | Buy       | GBX      |            | 103              | 500           | 500          |
         | Barclays     | 5       | 01/01/2019 13:54:00 | 01/01/2019 13:54:00   | Market | Buy       | GBX      |            | 104              | 500           | 500          |
         | Barclays     | 6       | 01/01/2019 13:55:00 | 01/01/2019 13:55:00   | Market | Buy       | GBX      |            | 105              | 500           | 500          |
         | Barclays     | 7       | 01/01/2019 13:56:00 | 01/01/2019 13:56:00   | Market | Buy       | GBX      |            | 106              | 500           | 500          |
         | Barclays     | 8       | 01/01/2019 13:57:00 | 01/01/2019 13:57:00   | Market | Buy       | GBX      |            | 107              | 500           | 500          |
         | Barclays     | 9       | 01/01/2019 13:58:00 | 01/01/2019 13:58:00   | Market | Buy       | GBX      |            | 108              | 500           | 500          |
         | Barclays     | 10      | 01/01/2019 13:59:00 | 01/01/2019 13:59:00   | Market | Buy       | GBX      |            | 109              | 500           | 500          |
		And With the intraday market data :
		| SecurityName | Epoch			      | Bid | Ask | Price	 | Currency | Volume |
		| Barclays     | 01/01/2019  13:50:00 | 1	  | 20  | 100    | GBX      | 5000  |
		| Barclays     | 01/01/2019  13:51:00 | 1	  | 20  | 101    | GBX      | 5000  |
		| Barclays     | 01/01/2019  13:52:00 | 1	  | 20  | 102    | GBX      | 5000  |
		| Barclays     | 01/01/2019  13:53:00 | 1	  | 20  | 103    | GBX      | 5000  |
		| Barclays     | 01/01/2019  13:54:00 | 1	  | 20  | 104    | GBX      | 5000  |
		| Barclays     | 01/01/2019  13:55:00 | 1	  | 20  | 105    | GBX      | 5000  |
		| Barclays     | 01/01/2019  13:56:00 | 1	  | 20  | 106    | GBX      | 5000  |
		| Barclays     | 01/01/2019  13:57:00 | 1	  | 20  | 107    | GBX      | 5000  |
		| Barclays     | 01/01/2019  13:58:00 | 1	  | 20  | 108    | GBX      | 5000  |
		| Barclays     | 01/01/2019  13:59:00 | 1	  | 20  | 109    | GBX      | 5000  |
		 When I run the ramping rule
		 Then I will have 7 ramping alerts

Scenario: Ramping with increasing prices matching increasing order fill values raises 8 alerts
		Given I have the orders for a universe from 01/01/2019 to 01/01/2019 :
		| SecurityName | OrderId | PlacedDate			| FilledDate			| Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
		| Barclays     | 1       | 01/01/2019 13:50:00 | 01/01/2019 13:50:00   | Market | Buy       | GBX      |            | 100              | 500           | 500          |
		| Barclays     | 2       | 01/01/2019 13:51:00 | 01/01/2019 13:51:00   | Market | Sell      | GBX      |            | 101              | 500           | 500          |
		| Barclays     | 3       | 01/01/2019 13:52:00 | 01/01/2019 13:52:00   | Market | Buy       | GBX      |            | 102              | 500           | 500          |
		| Barclays     | 4       | 01/01/2019 13:53:00 | 01/01/2019 13:53:00   | Market | Sell      | GBX      |            | 103              | 500           | 500          |
		| Barclays     | 5       | 01/01/2019 13:54:00 | 01/01/2019 13:54:00   | Market | Buy       | GBX      |            | 104              | 500           | 500          |
		| Barclays     | 6       | 01/01/2019 13:55:00 | 01/01/2019 13:55:00   | Market | Sell      | GBX      |            | 105              | 500           | 500          |
		| Barclays     | 7       | 01/01/2019 13:56:00 | 01/01/2019 13:56:00   | Market | Buy       | GBX      |            | 106              | 500           | 500          |
		| Barclays     | 8       | 01/01/2019 13:57:00 | 01/01/2019 13:57:00   | Market | Sell      | GBX      |            | 107              | 500           | 500          |
		| Barclays     | 9       | 01/01/2019 13:58:00 | 01/01/2019 13:58:00   | Market | Buy       | GBX      |            | 108              | 500           | 500          |
		| Barclays     | 10      | 01/01/2019 13:59:00 | 01/01/2019 13:59:00   | Market | Sell      | GBX      |            | 109              | 500           | 500          |
		And With the intraday market data :
		| SecurityName | Epoch			      | Bid | Ask | Price	 | Currency | Volume |
		| Barclays     | 01/01/2019  13:50:00 | 1	  | 20  | 100    | GBX      | 5000  |
		| Barclays     | 01/01/2019  13:51:00 | 1	  | 20  | 101    | GBX      | 5000  |
		| Barclays     | 01/01/2019  13:52:00 | 1	  | 20  | 102    | GBX      | 5000  |
		| Barclays     | 01/01/2019  13:53:00 | 1	  | 20  | 103    | GBX      | 5000  |
		| Barclays     | 01/01/2019  13:54:00 | 1	  | 20  | 104    | GBX      | 5000  |
		| Barclays     | 01/01/2019  13:55:00 | 1	  | 20  | 104    | GBX      | 5000  |
		| Barclays     | 01/01/2019  13:56:00 | 1	  | 20  | 103    | GBX      | 5000  |
		| Barclays     | 01/01/2019  13:57:00 | 1	  | 20  | 102    | GBX      | 5000  |
		| Barclays     | 01/01/2019  13:58:00 | 1	  | 20  | 101    | GBX      | 5000  |
		| Barclays     | 01/01/2019  13:59:00 | 1	  | 20  | 102    | GBX      | 5000  |
		When I run the ramping rule
		Then I will have 0 ramping alerts

Scenario: Ramping with increasing prices matching increasing order fill values raises 9 alerts
		Given I have the orders for a universe from 01/01/2019 to 01/01/2019 :
		| SecurityName | OrderId | PlacedDate			| FilledDate			| Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
		| Barclays     | 11      | 01/01/2019 10:50:00 | 01/01/2019 10:50:00   | Market | Buy       | GBX      |            | 100              | 500           | 500          |
		| Barclays     | 12      | 01/01/2019 10:51:00 | 01/01/2019 10:51:00   | Market | Buy       | GBX      |            | 101              | 500           | 500          |
		| Barclays     | 13      | 01/01/2019 10:52:00 | 01/01/2019 10:52:00   | Market | Buy       | GBX      |            | 102              | 500           | 500          |
		| Barclays     | 1       | 01/01/2019 13:50:00 | 01/01/2019 13:50:00   | Market | Buy       | GBX      |            | 100              | 500           | 500          |
		| Barclays     | 2       | 01/01/2019 13:51:00 | 01/01/2019 13:51:00   | Market | Sell      | GBX      |            | 101              | 500           | 500          |
		| Barclays     | 3       | 01/01/2019 13:52:00 | 01/01/2019 13:52:00   | Market | Buy       | GBX      |            | 102              | 500           | 500          |
		| Barclays     | 4       | 01/01/2019 13:53:00 | 01/01/2019 13:53:00   | Market | Sell      | GBX      |            | 103              | 500           | 500          |
		| Barclays     | 5       | 01/01/2019 13:54:00 | 01/01/2019 13:54:00   | Market | Buy       | GBX      |            | 104              | 500           | 500          |
		| Barclays     | 6       | 01/01/2019 13:55:00 | 01/01/2019 13:55:00   | Market | Sell      | GBX      |            | 105              | 500           | 500          |
		| Barclays     | 7       | 01/01/2019 13:56:00 | 01/01/2019 13:56:00   | Market | Buy       | GBX      |            | 106              | 500           | 500          |
		| Barclays     | 8       | 01/01/2019 13:57:00 | 01/01/2019 13:57:00   | Market | Sell      | GBX      |            | 107              | 500           | 500          |
		| Barclays     | 9       | 01/01/2019 13:58:00 | 01/01/2019 13:58:00   | Market | Buy       | GBX      |            | 108              | 500           | 500          |
		| Barclays     | 10      | 01/01/2019 13:59:00 | 01/01/2019 13:59:00   | Market | Sell      | GBX      |            | 109              | 500           | 500          |
		And With the intraday market data :
		| SecurityName | Epoch			      | Bid | Ask | Price	 | Currency | Volume |
		| Barclays     | 01/01/2019  10:50:00 | 1	  | 20  | 100    | GBX      | 5000  |
		| Barclays     | 01/01/2019  10:51:00 | 1	  | 20  | 101    | GBX      | 5000  |
		| Barclays     | 01/01/2019  10:52:00 | 1	  | 20  | 102    | GBX      | 5000  |
		| Barclays     | 01/01/2019  13:50:00 | 1	  | 20  | 100    | GBX      | 5000  |
		| Barclays     | 01/01/2019  13:51:00 | 1	  | 20  | 101    | GBX      | 5000  |
		| Barclays     | 01/01/2019  13:52:00 | 1	  | 20  | 102    | GBX      | 5000  |
		| Barclays     | 01/01/2019  13:53:00 | 1	  | 20  | 103    | GBX      | 5000  |
		| Barclays     | 01/01/2019  13:54:00 | 1	  | 20  | 104    | GBX      | 5000  |
		| Barclays     | 01/01/2019  13:55:00 | 1	  | 20  | 104    | GBX      | 5000  |
		| Barclays     | 01/01/2019  13:56:00 | 1	  | 20  | 103    | GBX      | 5000  |
		| Barclays     | 01/01/2019  13:57:00 | 1	  | 20  | 102    | GBX      | 5000  |
		| Barclays     | 01/01/2019  13:58:00 | 1	  | 20  | 101    | GBX      | 5000  |
		| Barclays     | 01/01/2019  13:59:00 | 1	  | 20  | 102    | GBX      | 5000  |
		When I run the ramping rule
		Then I will have 0 ramping alerts

Scenario: Ramping with increasing prices matching increasing order fill values raises 10 alerts
		Given I have the orders for a universe from 01/01/2019 to 01/01/2019 :
		| SecurityName | OrderId | PlacedDate			| FilledDate			| Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
		| Barclays     | 11      | 01/01/2019 10:50:00 | 01/01/2019 10:50:00   | Market | Buy       | GBX      |            | 100              | 500           | 500          |
		| Barclays     | 12      | 01/01/2019 10:51:00 | 01/01/2019 10:51:00   | Market | Buy       | GBX      |            | 101              | 500           | 500          |
		| Barclays     | 13      | 01/01/2019 10:52:00 | 01/01/2019 10:52:00   | Market | Buy       | GBX      |            | 102              | 500           | 500          |
		| Barclays     | 1       | 01/01/2019 13:50:00 | 01/01/2019 13:50:00   | Market | Buy       | GBX      |            | 100              | 500           | 500          |
		| Barclays     | 2       | 01/01/2019 13:51:00 | 01/01/2019 13:51:00   | Market | Sell      | GBX      |            | 101              | 500           | 500          |
		| Barclays     | 3       | 01/01/2019 13:52:00 | 01/01/2019 13:52:00   | Market | Buy       | GBX      |            | 102              | 500           | 500          |
		| Barclays     | 4       | 01/01/2019 13:53:00 | 01/01/2019 13:53:00   | Market | Sell      | GBX      |            | 103              | 500           | 500          |
		| Barclays     | 5       | 01/01/2019 13:54:00 | 01/01/2019 13:54:00   | Market | Buy       | GBX      |            | 104              | 500           | 500          |
		| Barclays     | 6       | 01/01/2019 13:55:00 | 01/01/2019 13:55:00   | Market | Sell      | GBX      |            | 105              | 500           | 500          |
		| Barclays     | 7       | 01/01/2019 13:56:00 | 01/01/2019 13:56:00   | Market | Buy       | GBX      |            | 106              | 500           | 500          |
		| Barclays     | 8       | 01/01/2019 13:57:00 | 01/01/2019 13:57:00   | Market | Sell      | GBX      |            | 107              | 500           | 500          |
		| Barclays     | 9       | 01/01/2019 13:58:00 | 01/01/2019 13:58:00   | Market | Buy       | GBX      |            | 108              | 500           | 500          |
		| Barclays     | 10      | 01/01/2019 13:59:00 | 01/01/2019 13:59:00   | Market | Sell      | GBX      |            | 109              | 500           | 500          |
		And With the intraday market data :
		| SecurityName | Epoch			      | Bid | Ask | Price	 | Currency | Volume |
		| Barclays     | 01/01/2019  10:50:00 | 1	  | 20  | 100    | GBX      | 5000  |
		| Barclays     | 01/01/2019  10:51:00 | 1	  | 20  | 101    | GBX      | 5000  |
		| Barclays     | 01/01/2019  10:52:00 | 1	  | 20  | 102    | GBX      | 5000  |
		| Barclays     | 01/01/2019  13:50:00 | 1	  | 20  | 100    | GBX      | 5000  |
		| Barclays     | 01/01/2019  13:51:00 | 1	  | 20  | 101    | GBX      | 5000  |
		| Barclays     | 01/01/2019  13:52:00 | 1	  | 20  | 102    | GBX      | 5000  |
		| Barclays     | 01/01/2019  13:53:00 | 1	  | 20  | 103    | GBX      | 5000  |
		| Barclays     | 01/01/2019  13:54:00 | 1	  | 20  | 104    | GBX      | 5000  |
		| Barclays     | 01/01/2019  13:55:00 | 1	  | 20  | 104    | GBX      | 5000  |
		| Barclays     | 01/01/2019  13:56:00 | 1	  | 20  | 103    | GBX      | 5000  |
		| Barclays     | 01/01/2019  13:57:00 | 1	  | 20  | 102    | GBX      | 5000  |
		| Barclays     | 01/01/2019  13:58:00 | 1	  | 20  | 101    | GBX      | 5000  |
		| Barclays     | 01/01/2019  13:59:00 | 1	  | 20  | 102    | GBX      | 5000  |
		When I run the ramping rule
		Then I will have 0 ramping alerts

		# ACCEPTANCE TEST SECTION 

		# ***** B U Y S *****
	Scenario: Ramping buys with increasing prices yields 10 alerts
		Given I have the orders for a universe from 01/01/2019 to 01/01/2019 :
		| SecurityName | OrderId | PlacedDate			| FilledDate			| Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
		| Barclays     | 1       | 01/01/2019 13:50:00 | 01/01/2019 13:50:00   | Market | Buy       | GBX      |            | 100              | 500           | 500          |
		| Barclays     | 2       | 01/01/2019 13:51:00 | 01/01/2019 13:51:00   | Market | Buy       | GBX      |            | 101              | 500           | 500          |
		| Barclays     | 3       | 01/01/2019 13:52:00 | 01/01/2019 13:52:00   | Market | Buy       | GBX      |            | 102              | 500           | 500          |
		| Barclays     | 4       | 01/01/2019 13:53:00 | 01/01/2019 13:53:00   | Market | Buy       | GBX      |            | 103              | 500           | 500          |
		| Barclays     | 5       | 01/01/2019 13:54:00 | 01/01/2019 13:54:00   | Market | Buy       | GBX      |            | 104              | 500           | 500          |
		| Barclays     | 6       | 01/01/2019 13:55:00 | 01/01/2019 13:55:00   | Market | Buy       | GBX      |            | 105              | 500           | 500          |
		| Barclays     | 7       | 01/01/2019 13:56:00 | 01/01/2019 13:56:00   | Market | Buy       | GBX      |            | 106              | 500           | 500          |
		| Barclays     | 8       | 01/01/2019 13:57:00 | 01/01/2019 13:57:00   | Market | Buy       | GBX      |            | 107              | 500           | 500          |
		| Barclays     | 9       | 01/01/2019 13:58:00 | 01/01/2019 13:58:00   | Market | Buy       | GBX      |            | 108              | 500           | 500          |
		| Barclays     | 10      | 01/01/2019 13:59:00 | 01/01/2019 13:59:00   | Market | Buy       | GBX      |            | 109              | 500           | 500          |
		And With the intraday market data :
		| SecurityName | Epoch			      | Bid | Ask | Price	 | Currency | Volume |
		| Barclays     | 01/01/2019  13:50:00 | 1	  | 20  | 100    | GBX      | 5000  |
		| Barclays     | 01/01/2019  13:51:00 | 1	  | 20  | 101    | GBX      | 5000  |
		| Barclays     | 01/01/2019  13:52:00 | 1	  | 20  | 102    | GBX      | 5000  |
		| Barclays     | 01/01/2019  13:53:00 | 1	  | 20  | 103    | GBX      | 5000  |
		| Barclays     | 01/01/2019  13:54:00 | 1	  | 20  | 104    | GBX      | 5000  |
		| Barclays     | 01/01/2019  13:55:00 | 1	  | 20  | 105    | GBX      | 5000  |
		| Barclays     | 01/01/2019  13:56:00 | 1	  | 20  | 106    | GBX      | 5000  |
		| Barclays     | 01/01/2019  13:57:00 | 1	  | 20  | 107    | GBX      | 5000  |
		| Barclays     | 01/01/2019  13:58:00 | 1	  | 20  | 108    | GBX      | 5000  |
		| Barclays     | 01/01/2019  13:59:00 | 1	  | 20  | 109    | GBX      | 5000  |
		When I run the ramping rule
		Then I will have 7 ramping alerts

	Scenario: Ramping buys with stagnant prices yields 10 alerts
		Given I have the orders for a universe from 01/01/2019 to 01/01/2019 :
		| SecurityName | OrderId | PlacedDate			| FilledDate			| Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
		| Barclays     | 1       | 01/01/2019 13:50:00 | 01/01/2019 13:50:00   | Market | Buy        | GBX      |            | 100              | 500           | 500          |
		| Barclays     | 2       | 01/01/2019 13:51:00 | 01/01/2019 13:51:00   | Market | Buy        | GBX      |            | 101              | 500           | 500          |
		| Barclays     | 3       | 01/01/2019 13:52:00 | 01/01/2019 13:52:00   | Market | Buy        | GBX      |            | 100              | 500           | 500          |
		| Barclays     | 4       | 01/01/2019 13:53:00 | 01/01/2019 13:53:00   | Market | Buy        | GBX      |            | 100              | 500           | 500          |
		| Barclays     | 5       | 01/01/2019 13:54:00 | 01/01/2019 13:54:00   | Market | Buy        | GBX      |            | 100              | 500           | 500          |
		| Barclays     | 6       | 01/01/2019 13:55:00 | 01/01/2019 13:55:00   | Market | Buy        | GBX      |            | 100              | 500           | 500          |
		| Barclays     | 7       | 01/01/2019 13:56:00 | 01/01/2019 13:56:00   | Market | Buy        | GBX      |            | 101              | 500           | 500          |
		| Barclays     | 8       | 01/01/2019 13:57:00 | 01/01/2019 13:57:00   | Market | Buy        | GBX      |            | 101              | 500           | 500          |
		| Barclays     | 9       | 01/01/2019 13:58:00 | 01/01/2019 13:58:00   | Market | Buy        | GBX      |            | 100              | 500           | 500          |
		| Barclays     | 10      | 01/01/2019 13:59:00 | 01/01/2019 13:59:00   | Market | Buy        | GBX      |            | 101              | 500           | 500          |
		And With the intraday market data :
		| SecurityName | Epoch			      | Bid | Ask | Price	 | Currency | Volume |
		| Barclays     | 01/01/2019  13:50:00 | 1	  | 20  | 100    | GBX      | 5000  |
		| Barclays     | 01/01/2019  13:51:00 | 1	  | 20  | 100    | GBX      | 5000  |
		| Barclays     | 01/01/2019  13:52:00 | 1	  | 20  | 100    | GBX      | 5000  |
		| Barclays     | 01/01/2019  13:53:00 | 1	  | 20  | 100    | GBX      | 5000  |
		| Barclays     | 01/01/2019  13:54:00 | 1	  | 20  | 100    | GBX      | 5000  |
		| Barclays     | 01/01/2019  13:55:00 | 1	  | 20  | 99    | GBX      | 5000  |
		| Barclays     | 01/01/2019  13:56:00 | 1	  | 20  | 100    | GBX      | 5000  |
		| Barclays     | 01/01/2019  13:57:00 | 1	  | 20  | 101    | GBX      | 5000  |
		| Barclays     | 01/01/2019  13:58:00 | 1	  | 20  | 100    | GBX      | 5000  |
		| Barclays     | 01/01/2019  13:59:00 | 1	  | 20  | 100    | GBX      | 5000  |
		When I run the ramping rule
		Then I will have 0 ramping alerts

	Scenario: Ramping buys with decreasing prices yields 0 alerts
		Given I have the orders for a universe from 01/01/2019 to 01/01/2019 :
		| SecurityName | OrderId | PlacedDate			| FilledDate			| Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
		| Barclays     | 1       | 01/01/2019 13:50:00 | 01/01/2019 13:50:00   | Market | Buy        | GBX      |            | 100              | 500           | 500          |
		| Barclays     | 2       | 01/01/2019 13:51:00 | 01/01/2019 13:51:00   | Market | Buy        | GBX      |            | 99              | 500           | 500          |
		| Barclays     | 3       | 01/01/2019 13:52:00 | 01/01/2019 13:52:00   | Market | Buy        | GBX      |            | 98              | 500           | 500          |
		| Barclays     | 4       | 01/01/2019 13:53:00 | 01/01/2019 13:53:00   | Market | Buy        | GBX      |            | 97               | 500           | 500          |
		| Barclays     | 5       | 01/01/2019 13:54:00 | 01/01/2019 13:54:00   | Market | Buy        | GBX      |            | 96               | 500           | 500          |
		| Barclays     | 6       | 01/01/2019 13:55:00 | 01/01/2019 13:55:00   | Market | Buy        | GBX      |            | 95               | 500           | 500          |
		| Barclays     | 7       | 01/01/2019 13:56:00 | 01/01/2019 13:56:00   | Market | Buy        | GBX      |            | 94               | 500           | 500          |
		| Barclays     | 8       | 01/01/2019 13:57:00 | 01/01/2019 13:57:00   | Market | Buy        | GBX      |            | 93               | 500           | 500          |
		| Barclays     | 9       | 01/01/2019 13:58:00 | 01/01/2019 13:58:00   | Market | Buy        | GBX      |            | 92               | 500           | 500          |
		| Barclays     | 10      | 01/01/2019 13:59:00 | 01/01/2019 13:59:00   | Market | Buy        | GBX      |            | 91               | 500           | 500          |
		And With the intraday market data :
		| SecurityName | Epoch			      | Bid | Ask | Price	 | Currency | Volume |
		| Barclays     | 01/01/2019  13:50:00 | 1	  | 20  | 100    | GBX      | 5000  |
		| Barclays     | 01/01/2019  13:51:00 | 1	  | 20  | 99    | GBX      | 5000  |
		| Barclays     | 01/01/2019  13:52:00 | 1	  | 20  | 98    | GBX      | 5000  |
		| Barclays     | 01/01/2019  13:53:00 | 1	  | 20  | 97     | GBX      | 5000  |
		| Barclays     | 01/01/2019  13:54:00 | 1	  | 20  | 96     | GBX      | 5000  |
		| Barclays     | 01/01/2019  13:55:00 | 1	  | 20  | 95     | GBX      | 5000  |
		| Barclays     | 01/01/2019  13:56:00 | 1	  | 20  | 94     | GBX      | 5000  |
		| Barclays     | 01/01/2019  13:57:00 | 1	  | 20  | 93     | GBX      | 5000  |
		| Barclays     | 01/01/2019  13:58:00 | 1	  | 20  | 92     | GBX      | 5000  |
		| Barclays     | 01/01/2019  13:59:00 | 1	  | 20  | 91     | GBX      | 5000  |
		When I run the ramping rule
		Then I will have 0 ramping alerts

	Scenario: Ramping buys with oscilliating prices yields 10 alerts
		Given I have the orders for a universe from 01/01/2019 to 01/01/2019 :
		| SecurityName | OrderId | PlacedDate			| FilledDate			| Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
		| Barclays     | 1       | 01/01/2019 13:50:00 | 01/01/2019 13:50:00   | Market | Buy        | GBX      |            | 100              | 500           | 500          |
		| Barclays     | 2       | 01/01/2019 13:51:00 | 01/01/2019 13:51:00   | Market | Buy        | GBX      |            | 101              | 500           | 500          |
		| Barclays     | 3       | 01/01/2019 13:52:00 | 01/01/2019 13:52:00   | Market | Buy        | GBX      |            | 100              | 500           | 500          |
		| Barclays     | 4       | 01/01/2019 13:53:00 | 01/01/2019 13:53:00   | Market | Buy        | GBX      |            | 100              | 500           | 500          |
		| Barclays     | 5       | 01/01/2019 13:54:00 | 01/01/2019 13:54:00   | Market | Buy        | GBX      |            | 101              | 500           | 500          |
		| Barclays     | 6       | 01/01/2019 13:55:00 | 01/01/2019 13:55:00   | Market | Buy        | GBX      |            | 102              | 500           | 500          |
		| Barclays     | 7       | 01/01/2019 13:56:00 | 01/01/2019 13:56:00   | Market | Buy        | GBX      |            | 101              | 500           | 500          |
		| Barclays     | 8       | 01/01/2019 13:57:00 | 01/01/2019 13:57:00   | Market | Buy        | GBX      |            | 100              | 500           | 500          |
		| Barclays     | 9       | 01/01/2019 13:58:00 | 01/01/2019 13:58:00   | Market | Buy        | GBX      |            | 99             | 500           | 500          |
		| Barclays     | 10      | 01/01/2019 13:59:00 | 01/01/2019 13:59:00   | Market | Buy        | GBX      |            | 100              | 500           | 500          |
		And With the intraday market data :
		| SecurityName | Epoch			      | Bid | Ask | Price	 | Currency | Volume |
		| Barclays     | 01/01/2019  13:50:00 | 1	  | 20  | 100    | GBX      | 5000  |
		| Barclays     | 01/01/2019  13:51:00 | 1	  | 20  | 101    | GBX      | 5000  |
		| Barclays     | 01/01/2019  13:52:00 | 1	  | 20  | 100    | GBX      | 5000  |
		| Barclays     | 01/01/2019  13:53:00 | 1	  | 20  | 101    | GBX      | 5000  |
		| Barclays     | 01/01/2019  13:54:00 | 1	  | 20  | 102    | GBX      | 5000  |
		| Barclays     | 01/01/2019  13:55:00 | 1	  | 20  | 101    | GBX      | 5000  |
		| Barclays     | 01/01/2019  13:56:00 | 1	  | 20  | 100    | GBX      | 5000  |
		| Barclays     | 01/01/2019  13:57:00 | 1	  | 20  | 99     | GBX      | 5000  |
		| Barclays     | 01/01/2019  13:58:00 | 1	  | 20  | 99     | GBX      | 5000  |
		| Barclays     | 01/01/2019  13:59:00 | 1	  | 20  | 100    | GBX      | 5000  |
		When I run the ramping rule
		Then I will have 0 ramping alerts

		# ***** S E L L S *****
	Scenario: Ramping sells with increasing prices yields 10 alerts
		Given I have the orders for a universe from 01/01/2019 to 01/01/2019 :
		| SecurityName | OrderId | PlacedDate			| FilledDate			| Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
		| Barclays     | 1       | 01/01/2019 13:50:00 | 01/01/2019 13:50:00   | Market | Sell       | GBX      |            | 100              | 500           | 500          |
		| Barclays     | 2       | 01/01/2019 13:51:00 | 01/01/2019 13:51:00   | Market | Sell       | GBX      |            | 101              | 500           | 500          |
		| Barclays     | 3       | 01/01/2019 13:52:00 | 01/01/2019 13:52:00   | Market | Sell       | GBX      |            | 102              | 500           | 500          |
		| Barclays     | 4       | 01/01/2019 13:53:00 | 01/01/2019 13:53:00   | Market | Sell       | GBX      |            | 103              | 500           | 500          |
		| Barclays     | 5       | 01/01/2019 13:54:00 | 01/01/2019 13:54:00   | Market | Sell       | GBX      |            | 104              | 500           | 500          |
		| Barclays     | 6       | 01/01/2019 13:55:00 | 01/01/2019 13:55:00   | Market | Sell       | GBX      |            | 105              | 500           | 500          |
		| Barclays     | 7       | 01/01/2019 13:56:00 | 01/01/2019 13:56:00   | Market | Sell       | GBX      |            | 106              | 500           | 500          |
		| Barclays     | 8       | 01/01/2019 13:57:00 | 01/01/2019 13:57:00   | Market | Sell       | GBX      |            | 107              | 500           | 500          |
		| Barclays     | 9       | 01/01/2019 13:58:00 | 01/01/2019 13:58:00   | Market | Sell       | GBX      |            | 108              | 500           | 500          |
		| Barclays     | 10      | 01/01/2019 13:59:00 | 01/01/2019 13:59:00   | Market | Sell       | GBX      |            | 109              | 500           | 500          |
		And With the intraday market data :
		| SecurityName | Epoch			      | Bid | Ask | Price	 | Currency | Volume |
		| Barclays     | 01/01/2019  13:50:00 | 1	  | 20  | 100    | GBX      | 5000  |
		| Barclays     | 01/01/2019  13:51:00 | 1	  | 20  | 101    | GBX      | 5000  |
		| Barclays     | 01/01/2019  13:52:00 | 1	  | 20  | 102    | GBX      | 5000  |
		| Barclays     | 01/01/2019  13:53:00 | 1	  | 20  | 103    | GBX      | 5000  |
		| Barclays     | 01/01/2019  13:54:00 | 1	  | 20  | 104    | GBX      | 5000  |
		| Barclays     | 01/01/2019  13:55:00 | 1	  | 20  | 105    | GBX      | 5000  |
		| Barclays     | 01/01/2019  13:56:00 | 1	  | 20  | 106    | GBX      | 5000  |
		| Barclays     | 01/01/2019  13:57:00 | 1	  | 20  | 107    | GBX      | 5000  |
		| Barclays     | 01/01/2019  13:58:00 | 1	  | 20  | 108    | GBX      | 5000  |
		| Barclays     | 01/01/2019  13:59:00 | 1	  | 20  | 109    | GBX      | 5000  |
		When I run the ramping rule
		Then I will have 0 ramping alerts

	Scenario: Ramping sells with stagnant prices yields 10 alerts
		Given I have the orders for a universe from 01/01/2019 to 01/01/2019 :
		| SecurityName | OrderId | PlacedDate			| FilledDate			| Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
		| Barclays     | 1       | 01/01/2019 13:50:00 | 01/01/2019 13:50:00   | Market | Sell        | GBX      |            | 100              | 500           | 500          |
		| Barclays     | 2       | 01/01/2019 13:51:00 | 01/01/2019 13:51:00   | Market | Sell        | GBX      |            | 101              | 500           | 500          |
		| Barclays     | 3       | 01/01/2019 13:52:00 | 01/01/2019 13:52:00   | Market | Sell        | GBX      |            | 100              | 500           | 500          |
		| Barclays     | 4       | 01/01/2019 13:53:00 | 01/01/2019 13:53:00   | Market | Sell        | GBX      |            | 100              | 500           | 500          |
		| Barclays     | 5       | 01/01/2019 13:54:00 | 01/01/2019 13:54:00   | Market | Sell        | GBX      |            | 100              | 500           | 500          |
		| Barclays     | 6       | 01/01/2019 13:55:00 | 01/01/2019 13:55:00   | Market | Sell        | GBX      |            | 100              | 500           | 500          |
		| Barclays     | 7       | 01/01/2019 13:56:00 | 01/01/2019 13:56:00   | Market | Sell        | GBX      |            | 101              | 500           | 500          |
		| Barclays     | 8       | 01/01/2019 13:57:00 | 01/01/2019 13:57:00   | Market | Sell        | GBX      |            | 101              | 500           | 500          |
		| Barclays     | 9       | 01/01/2019 13:58:00 | 01/01/2019 13:58:00   | Market | Sell        | GBX      |            | 100              | 500           | 500          |
		| Barclays     | 10      | 01/01/2019 13:59:00 | 01/01/2019 13:59:00   | Market | Sell        | GBX      |            | 101              | 500           | 500          |
		And With the intraday market data :
		| SecurityName | Epoch			      | Bid | Ask | Price	 | Currency | Volume |
		| Barclays     | 01/01/2019  13:50:00 | 1	  | 20  | 100    | GBX      | 5000  |
		| Barclays     | 01/01/2019  13:51:00 | 1	  | 20  | 100    | GBX      | 5000  |
		| Barclays     | 01/01/2019  13:52:00 | 1	  | 20  | 100    | GBX      | 5000  |
		| Barclays     | 01/01/2019  13:53:00 | 1	  | 20  | 100    | GBX      | 5000  |
		| Barclays     | 01/01/2019  13:54:00 | 1	  | 20  | 100    | GBX      | 5000  |
		| Barclays     | 01/01/2019  13:55:00 | 1	  | 20  | 99    | GBX      | 5000  |
		| Barclays     | 01/01/2019  13:56:00 | 1	  | 20  | 100    | GBX      | 5000  |
		| Barclays     | 01/01/2019  13:57:00 | 1	  | 20  | 101    | GBX      | 5000  |
		| Barclays     | 01/01/2019  13:58:00 | 1	  | 20  | 100    | GBX      | 5000  |
		| Barclays     | 01/01/2019  13:59:00 | 1	  | 20  | 100    | GBX      | 5000  |
		When I run the ramping rule
		Then I will have 0 ramping alerts

	Scenario: Ramping sells with decreasing prices yields 0 alerts
		Given I have the orders for a universe from 01/01/2019 to 01/01/2019 :
		| SecurityName | OrderId | PlacedDate			| FilledDate			| Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
		| Barclays     | 1       | 01/01/2019 13:50:00 | 01/01/2019 13:50:00   | Market | Sell        | GBX      |            | 100              | 500           | 500          |
		| Barclays     | 2       | 01/01/2019 13:51:00 | 01/01/2019 13:51:00   | Market | Sell        | GBX      |            | 99              | 500           | 500          |
		| Barclays     | 3       | 01/01/2019 13:52:00 | 01/01/2019 13:52:00   | Market | Sell        | GBX      |            | 98              | 500           | 500          |
		| Barclays     | 4       | 01/01/2019 13:53:00 | 01/01/2019 13:53:00   | Market | Sell        | GBX      |            | 97               | 500           | 500          |
		| Barclays     | 5       | 01/01/2019 13:54:00 | 01/01/2019 13:54:00   | Market | Sell        | GBX      |            | 96               | 500           | 500          |
		| Barclays     | 6       | 01/01/2019 13:55:00 | 01/01/2019 13:55:00   | Market | Sell        | GBX      |            | 95               | 500           | 500          |
		| Barclays     | 7       | 01/01/2019 13:56:00 | 01/01/2019 13:56:00   | Market | Sell        | GBX      |            | 94               | 500           | 500          |
		| Barclays     | 8       | 01/01/2019 13:57:00 | 01/01/2019 13:57:00   | Market | Sell        | GBX      |            | 93               | 500           | 500          |
		| Barclays     | 9       | 01/01/2019 13:58:00 | 01/01/2019 13:58:00   | Market | Sell        | GBX      |            | 92               | 500           | 500          |
		| Barclays     | 10      | 01/01/2019 13:59:00 | 01/01/2019 13:59:00   | Market | Sell        | GBX      |            | 91               | 500           | 500          |
		And With the intraday market data :
		| SecurityName | Epoch			      | Bid | Ask | Price	 | Currency | Volume |
		| Barclays     | 01/01/2019  13:50:00 | 1	  | 20  | 100    | GBX      | 5000  |
		| Barclays     | 01/01/2019  13:51:00 | 1	  | 20  | 99    | GBX      | 5000  |
		| Barclays     | 01/01/2019  13:52:00 | 1	  | 20  | 98    | GBX      | 5000  |
		| Barclays     | 01/01/2019  13:53:00 | 1	  | 20  | 97     | GBX      | 5000  |
		| Barclays     | 01/01/2019  13:54:00 | 1	  | 20  | 96     | GBX      | 5000  |
		| Barclays     | 01/01/2019  13:55:00 | 1	  | 20  | 95     | GBX      | 5000  |
		| Barclays     | 01/01/2019  13:56:00 | 1	  | 20  | 94     | GBX      | 5000  |
		| Barclays     | 01/01/2019  13:57:00 | 1	  | 20  | 93     | GBX      | 5000  |
		| Barclays     | 01/01/2019  13:58:00 | 1	  | 20  | 92     | GBX      | 5000  |
		| Barclays     | 01/01/2019  13:59:00 | 1	  | 20  | 91     | GBX      | 5000  |
		When I run the ramping rule
		Then I will have 7 ramping alerts

	Scenario: Ramping sells with oscilliating prices yields 10 alerts
		Given I have the orders for a universe from 01/01/2019 to 01/01/2019 :
		| SecurityName | OrderId | PlacedDate			| FilledDate			| Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
		| Barclays     | 1       | 01/01/2019 13:50:00 | 01/01/2019 13:50:00   | Market | Sell       | GBX      |            | 100              | 500           | 500          |
		| Barclays     | 2       | 01/01/2019 13:51:00 | 01/01/2019 13:51:00   | Market | Sell       | GBX      |            | 101              | 500           | 500          |
		| Barclays     | 3       | 01/01/2019 13:52:00 | 01/01/2019 13:52:00   | Market | Sell       | GBX      |            | 100              | 500           | 500          |
		| Barclays     | 4       | 01/01/2019 13:53:00 | 01/01/2019 13:53:00   | Market | Sell       | GBX      |            | 100              | 500           | 500          |
		| Barclays     | 5       | 01/01/2019 13:54:00 | 01/01/2019 13:54:00   | Market | Sell       | GBX      |            | 101              | 500           | 500          |
		| Barclays     | 6       | 01/01/2019 13:55:00 | 01/01/2019 13:55:00   | Market | Sell       | GBX      |            | 102              | 500           | 500          |
		| Barclays     | 7       | 01/01/2019 13:56:00 | 01/01/2019 13:56:00   | Market | Sell       | GBX      |            | 101              | 500           | 500          |
		| Barclays     | 8       | 01/01/2019 13:57:00 | 01/01/2019 13:57:00   | Market | Sell       | GBX      |            | 100              | 500           | 500          |
		| Barclays     | 9       | 01/01/2019 13:58:00 | 01/01/2019 13:58:00   | Market | Sell       | GBX      |            | 99             | 500           | 500          |
		| Barclays     | 10      | 01/01/2019 13:59:00 | 01/01/2019 13:59:00   | Market | Sell       | GBX      |            | 100              | 500           | 500          |
		And With the intraday market data :
		| SecurityName | Epoch			      | Bid | Ask | Price	 | Currency | Volume |
		| Barclays     | 01/01/2019  13:50:00 | 1	  | 20  | 100    | GBX      | 5000  |
		| Barclays     | 01/01/2019  13:51:00 | 1	  | 20  | 101    | GBX      | 5000  |
		| Barclays     | 01/01/2019  13:52:00 | 1	  | 20  | 100    | GBX      | 5000  |
		| Barclays     | 01/01/2019  13:53:00 | 1	  | 20  | 101    | GBX      | 5000  |
		| Barclays     | 01/01/2019  13:54:00 | 1	  | 20  | 102    | GBX      | 5000  |
		| Barclays     | 01/01/2019  13:55:00 | 1	  | 20  | 101    | GBX      | 5000  |
		| Barclays     | 01/01/2019  13:56:00 | 1	  | 20  | 100    | GBX      | 5000  |
		| Barclays     | 01/01/2019  13:57:00 | 1	  | 20  | 99     | GBX      | 5000  |
		| Barclays     | 01/01/2019  13:58:00 | 1	  | 20  | 99     | GBX      | 5000  |
		| Barclays     | 01/01/2019  13:59:00 | 1	  | 20  | 100    | GBX      | 5000  |
		When I run the ramping rule
		Then I will have 0 ramping alerts



		# ***** C O V E R S *****

			Scenario: Ramping covers with increasing prices yields 10 alerts
		Given I have the orders for a universe from 01/01/2019 to 01/01/2019 :
		| SecurityName | OrderId | PlacedDate			| FilledDate			| Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
		| Barclays     | 1       | 01/01/2019 13:50:00 | 01/01/2019 13:50:00   | Market | Cover      | GBX      |            | 100              | 500           | 500          |
		| Barclays     | 2       | 01/01/2019 13:51:00 | 01/01/2019 13:51:00   | Market | Cover      | GBX      |            | 101              | 500           | 500          |
		| Barclays     | 3       | 01/01/2019 13:52:00 | 01/01/2019 13:52:00   | Market | Cover      | GBX      |            | 102              | 500           | 500          |
		| Barclays     | 4       | 01/01/2019 13:53:00 | 01/01/2019 13:53:00   | Market | Cover      | GBX      |            | 103              | 500           | 500          |
		| Barclays     | 5       | 01/01/2019 13:54:00 | 01/01/2019 13:54:00   | Market | Cover      | GBX      |            | 104              | 500           | 500          |
		| Barclays     | 6       | 01/01/2019 13:55:00 | 01/01/2019 13:55:00   | Market | Cover      | GBX      |            | 105              | 500           | 500          |
		| Barclays     | 7       | 01/01/2019 13:56:00 | 01/01/2019 13:56:00   | Market | Cover      | GBX      |            | 106              | 500           | 500          |
		| Barclays     | 8       | 01/01/2019 13:57:00 | 01/01/2019 13:57:00   | Market | Cover      | GBX      |            | 107              | 500           | 500          |
		| Barclays     | 9       | 01/01/2019 13:58:00 | 01/01/2019 13:58:00   | Market | Cover      | GBX      |            | 108              | 500           | 500          |
		| Barclays     | 10      | 01/01/2019 13:59:00 | 01/01/2019 13:59:00   | Market | Cover      | GBX      |            | 109              | 500           | 500          |
		And With the intraday market data :
		| SecurityName | Epoch			      | Bid | Ask | Price	 | Currency | Volume |
		| Barclays     | 01/01/2019  13:50:00 | 1	  | 20  | 100    | GBX      | 5000  |
		| Barclays     | 01/01/2019  13:51:00 | 1	  | 20  | 101    | GBX      | 5000  |
		| Barclays     | 01/01/2019  13:52:00 | 1	  | 20  | 102    | GBX      | 5000  |
		| Barclays     | 01/01/2019  13:53:00 | 1	  | 20  | 103    | GBX      | 5000  |
		| Barclays     | 01/01/2019  13:54:00 | 1	  | 20  | 104    | GBX      | 5000  |
		| Barclays     | 01/01/2019  13:55:00 | 1	  | 20  | 105    | GBX      | 5000  |
		| Barclays     | 01/01/2019  13:56:00 | 1	  | 20  | 106    | GBX      | 5000  |
		| Barclays     | 01/01/2019  13:57:00 | 1	  | 20  | 107    | GBX      | 5000  |
		| Barclays     | 01/01/2019  13:58:00 | 1	  | 20  | 108    | GBX      | 5000  |
		| Barclays     | 01/01/2019  13:59:00 | 1	  | 20  | 109    | GBX      | 5000  |
		When I run the ramping rule
		Then I will have 7 ramping alerts

	Scenario: Ramping covers with stagnant prices yields 10 alerts
		Given I have the orders for a universe from 01/01/2019 to 01/01/2019 :
		| SecurityName | OrderId | PlacedDate			| FilledDate			| Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
		| Barclays     | 1       | 01/01/2019 13:50:00 | 01/01/2019 13:50:00   | Market | Cover      | GBX      |            | 100              | 500           | 500          |
		| Barclays     | 2       | 01/01/2019 13:51:00 | 01/01/2019 13:51:00   | Market | Cover      | GBX      |            | 101              | 500           | 500          |
		| Barclays     | 3       | 01/01/2019 13:52:00 | 01/01/2019 13:52:00   | Market | Cover      | GBX      |            | 100              | 500           | 500          |
		| Barclays     | 4       | 01/01/2019 13:53:00 | 01/01/2019 13:53:00   | Market | Cover      | GBX      |            | 100              | 500           | 500          |
		| Barclays     | 5       | 01/01/2019 13:54:00 | 01/01/2019 13:54:00   | Market | Cover      | GBX      |            | 100              | 500           | 500          |
		| Barclays     | 6       | 01/01/2019 13:55:00 | 01/01/2019 13:55:00   | Market | Cover      | GBX      |            | 100              | 500           | 500          |
		| Barclays     | 7       | 01/01/2019 13:56:00 | 01/01/2019 13:56:00   | Market | Cover      | GBX      |            | 101              | 500           | 500          |
		| Barclays     | 8       | 01/01/2019 13:57:00 | 01/01/2019 13:57:00   | Market | Cover      | GBX      |            | 101              | 500           | 500          |
		| Barclays     | 9       | 01/01/2019 13:58:00 | 01/01/2019 13:58:00   | Market | Cover      | GBX      |            | 100              | 500           | 500          |
		| Barclays     | 10      | 01/01/2019 13:59:00 | 01/01/2019 13:59:00   | Market | Cover      | GBX      |            | 101              | 500           | 500          |
		And With the intraday market data :
		| SecurityName | Epoch			      | Bid | Ask | Price	 | Currency | Volume |
		| Barclays     | 01/01/2019  13:50:00 | 1	  | 20  | 100    | GBX      | 5000  |
		| Barclays     | 01/01/2019  13:51:00 | 1	  | 20  | 100    | GBX      | 5000  |
		| Barclays     | 01/01/2019  13:52:00 | 1	  | 20  | 100    | GBX      | 5000  |
		| Barclays     | 01/01/2019  13:53:00 | 1	  | 20  | 100    | GBX      | 5000  |
		| Barclays     | 01/01/2019  13:54:00 | 1	  | 20  | 100    | GBX      | 5000  |
		| Barclays     | 01/01/2019  13:55:00 | 1	  | 20  | 99    | GBX      | 5000  |
		| Barclays     | 01/01/2019  13:56:00 | 1	  | 20  | 100    | GBX      | 5000  |
		| Barclays     | 01/01/2019  13:57:00 | 1	  | 20  | 101    | GBX      | 5000  |
		| Barclays     | 01/01/2019  13:58:00 | 1	  | 20  | 100    | GBX      | 5000  |
		| Barclays     | 01/01/2019  13:59:00 | 1	  | 20  | 100    | GBX      | 5000  |
		When I run the ramping rule
		Then I will have 0 ramping alerts

	Scenario: Ramping covers with decreasing prices yields 0 alerts
		Given I have the orders for a universe from 01/01/2019 to 01/01/2019 :
		| SecurityName | OrderId | PlacedDate			| FilledDate			| Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
		| Barclays     | 1       | 01/01/2019 13:50:00 | 01/01/2019 13:50:00   | Market | Cover      | GBX      |            | 100              | 500           | 500          |
		| Barclays     | 2       | 01/01/2019 13:51:00 | 01/01/2019 13:51:00   | Market | Cover      | GBX      |            | 99              | 500           | 500          |
		| Barclays     | 3       | 01/01/2019 13:52:00 | 01/01/2019 13:52:00   | Market | Cover      | GBX      |            | 98              | 500           | 500          |
		| Barclays     | 4       | 01/01/2019 13:53:00 | 01/01/2019 13:53:00   | Market | Cover      | GBX      |            | 97               | 500           | 500          |
		| Barclays     | 5       | 01/01/2019 13:54:00 | 01/01/2019 13:54:00   | Market | Cover      | GBX      |            | 96               | 500           | 500          |
		| Barclays     | 6       | 01/01/2019 13:55:00 | 01/01/2019 13:55:00   | Market | Cover      | GBX      |            | 95               | 500           | 500          |
		| Barclays     | 7       | 01/01/2019 13:56:00 | 01/01/2019 13:56:00   | Market | Cover      | GBX      |            | 94               | 500           | 500          |
		| Barclays     | 8       | 01/01/2019 13:57:00 | 01/01/2019 13:57:00   | Market | Cover      | GBX      |            | 93               | 500           | 500          |
		| Barclays     | 9       | 01/01/2019 13:58:00 | 01/01/2019 13:58:00   | Market | Cover      | GBX      |            | 92               | 500           | 500          |
		| Barclays     | 10      | 01/01/2019 13:59:00 | 01/01/2019 13:59:00   | Market | Cover      | GBX      |            | 91               | 500           | 500          |
		And With the intraday market data :
		| SecurityName | Epoch			      | Bid | Ask | Price	 | Currency | Volume |
		| Barclays     | 01/01/2019  13:50:00 | 1	  | 20  | 100    | GBX      | 5000  |
		| Barclays     | 01/01/2019  13:51:00 | 1	  | 20  | 99    | GBX      | 5000  |
		| Barclays     | 01/01/2019  13:52:00 | 1	  | 20  | 98    | GBX      | 5000  |
		| Barclays     | 01/01/2019  13:53:00 | 1	  | 20  | 97     | GBX      | 5000  |
		| Barclays     | 01/01/2019  13:54:00 | 1	  | 20  | 96     | GBX      | 5000  |
		| Barclays     | 01/01/2019  13:55:00 | 1	  | 20  | 95     | GBX      | 5000  |
		| Barclays     | 01/01/2019  13:56:00 | 1	  | 20  | 94     | GBX      | 5000  |
		| Barclays     | 01/01/2019  13:57:00 | 1	  | 20  | 93     | GBX      | 5000  |
		| Barclays     | 01/01/2019  13:58:00 | 1	  | 20  | 92     | GBX      | 5000  |
		| Barclays     | 01/01/2019  13:59:00 | 1	  | 20  | 91     | GBX      | 5000  |
		When I run the ramping rule
		Then I will have 0 ramping alerts

	Scenario: Ramping covers with oscilliating prices yields 10 alerts
		Given I have the orders for a universe from 01/01/2019 to 01/01/2019 :
		| SecurityName | OrderId | PlacedDate			| FilledDate			| Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
		| Barclays     | 1       | 01/01/2019 13:50:00 | 01/01/2019 13:50:00   | Market | Cover      | GBX      |            | 100              | 500           | 500          |
		| Barclays     | 2       | 01/01/2019 13:51:00 | 01/01/2019 13:51:00   | Market | Cover      | GBX      |            | 101              | 500           | 500          |
		| Barclays     | 3       | 01/01/2019 13:52:00 | 01/01/2019 13:52:00   | Market | Cover      | GBX      |            | 100              | 500           | 500          |
		| Barclays     | 4       | 01/01/2019 13:53:00 | 01/01/2019 13:53:00   | Market | Cover      | GBX      |            | 100              | 500           | 500          |
		| Barclays     | 5       | 01/01/2019 13:54:00 | 01/01/2019 13:54:00   | Market | Cover      | GBX      |            | 101              | 500           | 500          |
		| Barclays     | 6       | 01/01/2019 13:55:00 | 01/01/2019 13:55:00   | Market | Cover      | GBX      |            | 102              | 500           | 500          |
		| Barclays     | 7       | 01/01/2019 13:56:00 | 01/01/2019 13:56:00   | Market | Cover      | GBX      |            | 101              | 500           | 500          |
		| Barclays     | 8       | 01/01/2019 13:57:00 | 01/01/2019 13:57:00   | Market | Cover      | GBX      |            | 100              | 500           | 500          |
		| Barclays     | 9       | 01/01/2019 13:58:00 | 01/01/2019 13:58:00   | Market | Cover      | GBX      |            | 99             | 500           | 500          |
		| Barclays     | 10      | 01/01/2019 13:59:00 | 01/01/2019 13:59:00   | Market | Cover      | GBX      |            | 100              | 500           | 500          |
		And With the intraday market data :
		| SecurityName | Epoch			      | Bid | Ask | Price	 | Currency | Volume |
		| Barclays     | 01/01/2019  13:50:00 | 1	  | 20  | 100    | GBX      | 5000  |
		| Barclays     | 01/01/2019  13:51:00 | 1	  | 20  | 101    | GBX      | 5000  |
		| Barclays     | 01/01/2019  13:52:00 | 1	  | 20  | 100    | GBX      | 5000  |
		| Barclays     | 01/01/2019  13:53:00 | 1	  | 20  | 101    | GBX      | 5000  |
		| Barclays     | 01/01/2019  13:54:00 | 1	  | 20  | 102    | GBX      | 5000  |
		| Barclays     | 01/01/2019  13:55:00 | 1	  | 20  | 101    | GBX      | 5000  |
		| Barclays     | 01/01/2019  13:56:00 | 1	  | 20  | 100    | GBX      | 5000  |
		| Barclays     | 01/01/2019  13:57:00 | 1	  | 20  | 99     | GBX      | 5000  |
		| Barclays     | 01/01/2019  13:58:00 | 1	  | 20  | 99     | GBX      | 5000  |
		| Barclays     | 01/01/2019  13:59:00 | 1	  | 20  | 100    | GBX      | 5000  |
		When I run the ramping rule
		Then I will have 0 ramping alerts


		# ***** S H O R T S *****

	Scenario: Ramping shorts with increasing prices yields 10 alerts
		Given I have the orders for a universe from 01/01/2019 to 01/01/2019 :
		| SecurityName | OrderId | PlacedDate			| FilledDate			| Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
		| Barclays     | 1       | 01/01/2019 13:50:00 | 01/01/2019 13:50:00   | Market | Short       | GBX      |            | 100              | 500           | 500          |
		| Barclays     | 2       | 01/01/2019 13:51:00 | 01/01/2019 13:51:00   | Market | Short       | GBX      |            | 101              | 500           | 500          |
		| Barclays     | 3       | 01/01/2019 13:52:00 | 01/01/2019 13:52:00   | Market | Short       | GBX      |            | 102              | 500           | 500          |
		| Barclays     | 4       | 01/01/2019 13:53:00 | 01/01/2019 13:53:00   | Market | Short       | GBX      |            | 103              | 500           | 500          |
		| Barclays     | 5       | 01/01/2019 13:54:00 | 01/01/2019 13:54:00   | Market | Short       | GBX      |            | 104              | 500           | 500          |
		| Barclays     | 6       | 01/01/2019 13:55:00 | 01/01/2019 13:55:00   | Market | Short       | GBX      |            | 105              | 500           | 500          |
		| Barclays     | 7       | 01/01/2019 13:56:00 | 01/01/2019 13:56:00   | Market | Short       | GBX      |            | 106              | 500           | 500          |
		| Barclays     | 8       | 01/01/2019 13:57:00 | 01/01/2019 13:57:00   | Market | Short       | GBX      |            | 107              | 500           | 500          |
		| Barclays     | 9       | 01/01/2019 13:58:00 | 01/01/2019 13:58:00   | Market | Short       | GBX      |            | 108              | 500           | 500          |
		| Barclays     | 10      | 01/01/2019 13:59:00 | 01/01/2019 13:59:00   | Market | Short       | GBX      |            | 109              | 500           | 500          |
		And With the intraday market data :
		| SecurityName | Epoch			      | Bid | Ask | Price	 | Currency | Volume |
		| Barclays     | 01/01/2019  13:50:00 | 1	  | 20  | 100    | GBX      | 5000  |
		| Barclays     | 01/01/2019  13:51:00 | 1	  | 20  | 101    | GBX      | 5000  |
		| Barclays     | 01/01/2019  13:52:00 | 1	  | 20  | 102    | GBX      | 5000  |
		| Barclays     | 01/01/2019  13:53:00 | 1	  | 20  | 103    | GBX      | 5000  |
		| Barclays     | 01/01/2019  13:54:00 | 1	  | 20  | 104    | GBX      | 5000  |
		| Barclays     | 01/01/2019  13:55:00 | 1	  | 20  | 105    | GBX      | 5000  |
		| Barclays     | 01/01/2019  13:56:00 | 1	  | 20  | 106    | GBX      | 5000  |
		| Barclays     | 01/01/2019  13:57:00 | 1	  | 20  | 107    | GBX      | 5000  |
		| Barclays     | 01/01/2019  13:58:00 | 1	  | 20  | 108    | GBX      | 5000  |
		| Barclays     | 01/01/2019  13:59:00 | 1	  | 20  | 109    | GBX      | 5000  |
		When I run the ramping rule
		Then I will have 0 ramping alerts

	Scenario: Ramping shorts with stagnant prices yields 10 alerts
		Given I have the orders for a universe from 01/01/2019 to 01/01/2019 :
		| SecurityName | OrderId | PlacedDate			| FilledDate			| Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
		| Barclays     | 1       | 01/01/2019 13:50:00 | 01/01/2019 13:50:00   | Market | Short        | GBX      |            | 100              | 500           | 500          |
		| Barclays     | 2       | 01/01/2019 13:51:00 | 01/01/2019 13:51:00   | Market | Short        | GBX      |            | 101              | 500           | 500          |
		| Barclays     | 3       | 01/01/2019 13:52:00 | 01/01/2019 13:52:00   | Market | Short        | GBX      |            | 100              | 500           | 500          |
		| Barclays     | 4       | 01/01/2019 13:53:00 | 01/01/2019 13:53:00   | Market | Short        | GBX      |            | 100              | 500           | 500          |
		| Barclays     | 5       | 01/01/2019 13:54:00 | 01/01/2019 13:54:00   | Market | Short        | GBX      |            | 100              | 500           | 500          |
		| Barclays     | 6       | 01/01/2019 13:55:00 | 01/01/2019 13:55:00   | Market | Short        | GBX      |            | 100              | 500           | 500          |
		| Barclays     | 7       | 01/01/2019 13:56:00 | 01/01/2019 13:56:00   | Market | Short        | GBX      |            | 101              | 500           | 500          |
		| Barclays     | 8       | 01/01/2019 13:57:00 | 01/01/2019 13:57:00   | Market | Short        | GBX      |            | 101              | 500           | 500          |
		| Barclays     | 9       | 01/01/2019 13:58:00 | 01/01/2019 13:58:00   | Market | Short        | GBX      |            | 100              | 500           | 500          |
		| Barclays     | 10      | 01/01/2019 13:59:00 | 01/01/2019 13:59:00   | Market | Short        | GBX      |            | 101              | 500           | 500          |
		And With the intraday market data :
		| SecurityName | Epoch			      | Bid | Ask | Price	 | Currency | Volume |
		| Barclays     | 01/01/2019  13:50:00 | 1	  | 20  | 100    | GBX      | 5000  |
		| Barclays     | 01/01/2019  13:51:00 | 1	  | 20  | 100    | GBX      | 5000  |
		| Barclays     | 01/01/2019  13:52:00 | 1	  | 20  | 100    | GBX      | 5000  |
		| Barclays     | 01/01/2019  13:53:00 | 1	  | 20  | 100    | GBX      | 5000  |
		| Barclays     | 01/01/2019  13:54:00 | 1	  | 20  | 100    | GBX      | 5000  |
		| Barclays     | 01/01/2019  13:55:00 | 1	  | 20  | 99    | GBX      | 5000  |
		| Barclays     | 01/01/2019  13:56:00 | 1	  | 20  | 100    | GBX      | 5000  |
		| Barclays     | 01/01/2019  13:57:00 | 1	  | 20  | 101    | GBX      | 5000  |
		| Barclays     | 01/01/2019  13:58:00 | 1	  | 20  | 100    | GBX      | 5000  |
		| Barclays     | 01/01/2019  13:59:00 | 1	  | 20  | 100    | GBX      | 5000  |
		When I run the ramping rule
		Then I will have 0 ramping alerts

	Scenario: Ramping shorts with decreasing prices yields 0 alerts
		Given I have the orders for a universe from 01/01/2019 to 01/01/2019 :
		| SecurityName | OrderId | PlacedDate			| FilledDate			| Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
		| Barclays     | 1       | 01/01/2019 13:50:00 | 01/01/2019 13:50:00   | Market | Short        | GBX      |            | 100              | 500           | 500          |
		| Barclays     | 2       | 01/01/2019 13:51:00 | 01/01/2019 13:51:00   | Market | Short        | GBX      |            | 99              | 500           | 500          |
		| Barclays     | 3       | 01/01/2019 13:52:00 | 01/01/2019 13:52:00   | Market | Short        | GBX      |            | 98              | 500           | 500          |
		| Barclays     | 4       | 01/01/2019 13:53:00 | 01/01/2019 13:53:00   | Market | Short        | GBX      |            | 97               | 500           | 500          |
		| Barclays     | 5       | 01/01/2019 13:54:00 | 01/01/2019 13:54:00   | Market | Short        | GBX      |            | 96               | 500           | 500          |
		| Barclays     | 6       | 01/01/2019 13:55:00 | 01/01/2019 13:55:00   | Market | Short        | GBX      |            | 95               | 500           | 500          |
		| Barclays     | 7       | 01/01/2019 13:56:00 | 01/01/2019 13:56:00   | Market | Short        | GBX      |            | 94               | 500           | 500          |
		| Barclays     | 8       | 01/01/2019 13:57:00 | 01/01/2019 13:57:00   | Market | Short        | GBX      |            | 93               | 500           | 500          |
		| Barclays     | 9       | 01/01/2019 13:58:00 | 01/01/2019 13:58:00   | Market | Short        | GBX      |            | 92               | 500           | 500          |
		| Barclays     | 10      | 01/01/2019 13:59:00 | 01/01/2019 13:59:00   | Market | Short        | GBX      |            | 91               | 500           | 500          |
		And With the intraday market data :
		| SecurityName | Epoch			      | Bid | Ask | Price	 | Currency | Volume |
		| Barclays     | 01/01/2019  13:50:00 | 1	  | 20  | 100    | GBX      | 5000  |
		| Barclays     | 01/01/2019  13:51:00 | 1	  | 20  | 99    | GBX      | 5000  |
		| Barclays     | 01/01/2019  13:52:00 | 1	  | 20  | 98    | GBX      | 5000  |
		| Barclays     | 01/01/2019  13:53:00 | 1	  | 20  | 97     | GBX      | 5000  |
		| Barclays     | 01/01/2019  13:54:00 | 1	  | 20  | 96     | GBX      | 5000  |
		| Barclays     | 01/01/2019  13:55:00 | 1	  | 20  | 95     | GBX      | 5000  |
		| Barclays     | 01/01/2019  13:56:00 | 1	  | 20  | 94     | GBX      | 5000  |
		| Barclays     | 01/01/2019  13:57:00 | 1	  | 20  | 93     | GBX      | 5000  |
		| Barclays     | 01/01/2019  13:58:00 | 1	  | 20  | 92     | GBX      | 5000  |
		| Barclays     | 01/01/2019  13:59:00 | 1	  | 20  | 91     | GBX      | 5000  |
		When I run the ramping rule
		Then I will have 7 ramping alerts

	Scenario: Ramping shorts with oscilliating prices yields 10 alerts
		Given I have the orders for a universe from 01/01/2019 to 01/01/2019 :
		| SecurityName | OrderId | PlacedDate			| FilledDate			| Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
		| Barclays     | 1       | 01/01/2019 13:50:00 | 01/01/2019 13:50:00   | Market | Short       | GBX      |            | 100              | 500           | 500          |
		| Barclays     | 2       | 01/01/2019 13:51:00 | 01/01/2019 13:51:00   | Market | Short       | GBX      |            | 101              | 500           | 500          |
		| Barclays     | 3       | 01/01/2019 13:52:00 | 01/01/2019 13:52:00   | Market | Short       | GBX      |            | 100              | 500           | 500          |
		| Barclays     | 4       | 01/01/2019 13:53:00 | 01/01/2019 13:53:00   | Market | Short       | GBX      |            | 100              | 500           | 500          |
		| Barclays     | 5       | 01/01/2019 13:54:00 | 01/01/2019 13:54:00   | Market | Short       | GBX      |            | 101              | 500           | 500          |
		| Barclays     | 6       | 01/01/2019 13:55:00 | 01/01/2019 13:55:00   | Market | Short       | GBX      |            | 102              | 500           | 500          |
		| Barclays     | 7       | 01/01/2019 13:56:00 | 01/01/2019 13:56:00   | Market | Short       | GBX      |            | 101              | 500           | 500          |
		| Barclays     | 8       | 01/01/2019 13:57:00 | 01/01/2019 13:57:00   | Market | Short       | GBX      |            | 100              | 500           | 500          |
		| Barclays     | 9       | 01/01/2019 13:58:00 | 01/01/2019 13:58:00   | Market | Short       | GBX      |            | 99             | 500           | 500          |
		| Barclays     | 10      | 01/01/2019 13:59:00 | 01/01/2019 13:59:00   | Market | Short       | GBX      |            | 100              | 500           | 500          |
		And With the intraday market data :
		| SecurityName | Epoch			      | Bid | Ask | Price	 | Currency | Volume |
		| Barclays     | 01/01/2019  13:50:00 | 1	  | 20  | 100    | GBX      | 5000  |
		| Barclays     | 01/01/2019  13:51:00 | 1	  | 20  | 101    | GBX      | 5000  |
		| Barclays     | 01/01/2019  13:52:00 | 1	  | 20  | 100    | GBX      | 5000  |
		| Barclays     | 01/01/2019  13:53:00 | 1	  | 20  | 101    | GBX      | 5000  |
		| Barclays     | 01/01/2019  13:54:00 | 1	  | 20  | 102    | GBX      | 5000  |
		| Barclays     | 01/01/2019  13:55:00 | 1	  | 20  | 101    | GBX      | 5000  |
		| Barclays     | 01/01/2019  13:56:00 | 1	  | 20  | 100    | GBX      | 5000  |
		| Barclays     | 01/01/2019  13:57:00 | 1	  | 20  | 99     | GBX      | 5000  |
		| Barclays     | 01/01/2019  13:58:00 | 1	  | 20  | 99     | GBX      | 5000  |
		| Barclays     | 01/01/2019  13:59:00 | 1	  | 20  | 100    | GBX      | 5000  |
		When I run the ramping rule
		Then I will have 0 ramping alerts


		# END ACCEPTANCE TEST SECTION 

