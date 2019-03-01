@fixedincomehighprofit
@fixedincomehighprofitpercentage
@fixedincomehighprofitpercentagenonsensitive
Feature: Fixed Income High Profit Percentage Non Sensitive
	In order to meet MAR compliance requirements
	I need to be able to detect when traders are executing trades
	Which generate unusual levels of profits
	By measuring their security trade profits as a percentage return

Background:
			Given I have the fixed income high profit rule parameter values
			| WindowHours | HighProfitPercentage |   
			| 1           | 0.1                  |

Scenario: Empty Universe yields no alerts
	Given I have the orders for a universe from 01/01/2018 to 03/01/2018 :
	| SecurityName | OrderId | PlacedDate          | BookedDate | AmendedDate | RejectedDate | CancelledDate | FilledDate          | Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
	When I run the fixed income high profit rule
	Then I will have 0 fixed income high profit alerts

			