@cancelledorders
@cancelledorderspositionvolume
@cancelledorderpositionvolumesensitive
Feature: CancelledOrders Order Position Volume Sensitive Parameters
		In order to meet MAR compliance requirements
		I need to be able to detect when traders are placing orders
		which are then cancelled in an unusual pattern

Background:
			Given I have the cancelled orders rule parameter values
			| WindowHours | CancelledOrderPercentagePositionThreshold | CancelledOrderCountPercentageThreshold | MinimumNumberOfTradesToApplyRuleTo | MaximumNumberOfTradesToApplyRuleTo |
			| 1			  |	0.5										  |									       |        						 2 |									 |