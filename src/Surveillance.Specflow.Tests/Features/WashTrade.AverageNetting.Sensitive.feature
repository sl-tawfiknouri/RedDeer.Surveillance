Feature: WashTrade Average Netting Sensitive Parameters
	In order to meet MAR compliance requirements
	I need to be able to detect when traders are executing trades
	with no meaningful change of ownership


@washtrade
Scenario: Empty Universe yields no alerts
	Given I have the empty universe
	When I run the wash trade rule
	Then I will have 0 alerts
