@markingtheclose
@markingtheclosedaily
@markingtheclosedailysensitive
Feature: MarkingTheClose Daily Sensitive Parameters
	In order to meet MAR compliance requirements
	I need to be able to detect when traders are executing trades
	towards the market closure time at an unusually
	high volume in order to extract supernormal profits

	Background:
			Given I have the marking the close rule parameter values
			| WindowHours | PercentageThresholdDailyVolume | PercentageThresholdWindowVolume |
			| 1			  |	0.1							   |						         |


Scenario: Empty Universe yields no alerts
		 Given I have the orders for a universe from 01/01/2018 to 01/05/2018 :
         | SecurityName | OrderId | PlacedDate | FilledDate | Type | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
         When I run the marking the close rule
		 Then I will have 0 marking the close alerts
