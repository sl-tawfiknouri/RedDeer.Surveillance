@fixedincomehighvolumeissuance
@fixedincomehighvolumeissuancepercentage
@fixedincomehighvolumeissuancepercentagenonsensitive
Feature: Fixed Income High Volume Issuance Percentage Non Sensitive
	In order to meet MAR compliance requirements
	I need to be able to detect when traders are executing trades
	Which have unusually high volume as a proportion
	of the percentage issuance

Background:
			Given I have the fixed income high volume issuance rule parameter values
			| WindowHours |  
			| 1           |

Scenario: Empty Universe yields no alerts
	Given I have the orders for a universe from 01/01/2018 to 03/01/2018 :
	| SecurityName | OrderId | PlacedDate          | BookedDate | AmendedDate | RejectedDate | CancelledDate | FilledDate          | Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
	When I run the fixed income high volume issuance rule
	Then I will have 0 fixed income high volume issuance alerts