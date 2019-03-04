﻿@fixedincomewashtrade
@fixedincomewashtradeaveragenetting
@fixedincomewashtradeaveragenettingnonsensitive
Feature: Fixed Income Wash Trade Average Netting Non Sensitive
	In order to meet MAR compliance requirements
	I need to be able to detect when traders are executing trades
	Which net out to near zero net gain whilst meeting
	A given threshold of volume

Background:
			Given I have the fixed income wash trade rule parameter values
			| WindowHours |  
			| 1           |

Scenario: Empty Universe yields no alerts
	Given I have the orders for a universe from 01/01/2018 to 03/01/2018 :
	| SecurityName | OrderId | PlacedDate          | BookedDate | AmendedDate | RejectedDate | CancelledDate | FilledDate          | Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
	When I run the fixed income wash trade rule
	Then I will have 0 fixed income wash trade alerts