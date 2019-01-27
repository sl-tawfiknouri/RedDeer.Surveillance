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
	Given I have the buy sell universe
	When I run the wash trade rule
	Then I will have 1 wash trade alerts

@washtrade
Scenario: Three Trades In Wash Trade Universe yields no alerts
	Given I have the buy buy sell universe
	When I run the wash trade rule
	Then I will have 0 wash trade alerts

@washtrade
Scenario: Buy1 Sell1 at Price1 and Buy2 Sell2 and Price2 yields two alerts
	Given I have the buy sell at p1 buy sell at p2 universe
	When I run the wash trade rule
	Then I will have 2 wash trade alerts