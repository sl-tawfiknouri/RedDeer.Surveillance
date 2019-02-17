@cancelledorders
@cancelledorderscount
@cancelledordercountsensitive
Feature: CancelledOrders Order Count Sensitive Parameters
		In order to meet MAR compliance requirements
		I need to be able to detect when traders are placing orders
		which are then cancelled in an unusual pattern

Background:
			Given I have the cancelled orders rule parameter values
			| WindowHours | CancelledOrderPercentagePositionThreshold | CancelledOrderCountPercentageThreshold | MinimumNumberOfTradesToApplyRuleTo | MaximumNumberOfTradesToApplyRuleTo |
			| 1			  |											  |									  0.1 |									 2 |									 |
		
Scenario: Empty Universe yields no alerts
		 Given I have the orders for a universe from 01/01/2018 to 01/05/2018 :
         | SecurityName | OrderId | PlacedDate | FilledDate | Type | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
         When I run the cancelled orders rule
		 Then I will have 0 cancelled orders alerts



