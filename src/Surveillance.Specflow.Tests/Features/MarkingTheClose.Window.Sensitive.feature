@markingtheclose
@markingtheclosewindow
@markingtheclosewindowsensitive
Feature: MarkingTheClose Window Sensitive Parameters
	In order to meet MAR compliance requirements
	I need to be able to detect when traders are executing trades
	towards the market closure time at an unusually
	high volume in order to extract supernormal profits

Background:
			Given I have the marking the close rule parameter values
			| WindowHours | PercentageThresholdDailyVolume | PercentageThresholdWindowVolume |
			| 1           |                                | 0.1							 |


