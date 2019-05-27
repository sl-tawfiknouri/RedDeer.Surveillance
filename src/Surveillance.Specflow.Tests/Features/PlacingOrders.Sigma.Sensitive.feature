@placingorders
@placingorderssigma
@placingorderssigmasensitive
Feature: PlacingOrders Sigma Sensitive Parameters
		In order to meet MAR compliance requirements
		I need to be able to detect when traders are placing orders
		which were never likely to be executed

Background:
			Given I have the placing orders rule parameter values
			| WindowHours | Sigma | 
			| 1           | 1     | 
		
Scenario: Empty Universe yields no alerts
		 Given I have the orders for a universe from 01/01/2018 to 01/05/2018 :
         | SecurityName | OrderId | PlacedDate | FilledDate | Type | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
         When I run the placing orders rule
		 Then I will have 0 placing orders alerts





