﻿Feature: WashTrade Average Netting Sensitive Parameters
	In order to meet MAR compliance requirements
	I need to be able to detect when traders are executing trades
	with no meaningful change of ownership

Background:
	Given I have the wash trade rule parameter values
	| window hours | minimum number of trades | maximum position value change | maximum absolute value change | maximum absolute value change currency |
	| 1            | 2                        | 0.01m                         | 10000                         | GBX                                    |

@washtrade
Scenario: Empty Universe yields no alerts
	Given I have the empty universe
	When I run the wash trade rule
	Then I will have 0 alerts
