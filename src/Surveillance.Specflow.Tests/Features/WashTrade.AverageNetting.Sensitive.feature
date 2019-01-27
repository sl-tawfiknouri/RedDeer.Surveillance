Feature: WashTrade Average Netting Sensitive Parameters
	In order to meet MAR compliance requirements
	I need to be able to detect when traders are executing trades
	with no meaningful change of ownership

Background:
	Given I have the wash trade rule average netting parameter values:
	| window hours | minimum number of trades | maximum position value change | maximum absolute value change | maximum absolute value change currency |
	| 1            | 2                        | 0.01                         | 10000                         | GBX                                    |

@washtrade
Scenario: Empty Universe yields no alerts
	Given I have the empty universe
	When I run the wash trade rule
	Then I will have 0 wash trade alerts

@washtrade
Scenario: Two Trades In Wash Trade Universe yields one alert
	Given I have the one buy one sell universe
	When I run the wash trade rule
	Then I will have 1 wash trade alerts

@washtrade
Scenario: Three Trades In Wash Trade Universe yields no alerts
	Given I have the two buy one sell universe
	When I run the wash trade rule
	Then I will have 0 wash trade alerts