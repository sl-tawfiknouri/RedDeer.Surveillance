@spoofing
@spoofingsensitive
Feature: Spoofing Sensitive Parameters
	In order to meet MAR compliance requirements
	I need to be able to detect when traders are misleading markets
	And then capitalising on their market impact by trading differently

Background: 
	Given I have the spoofing rule parameter values
	| WindowHours |
	| 1           |

Scenario: Empty Universe yields no alerts
		 Given I have the orders for a universe from 01/01/2018 to 01/05/2018 :
         | SecurityName | OrderId | PlacedDate | FilledDate | Type | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
         When I run the spoofing rule
		 Then I will have 0 spoofing alerts
