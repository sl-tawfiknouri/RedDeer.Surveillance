Feature: HighProfit Percentage Sensitive Parameters
	In order to meet MAR compliance requirements
	I need to be able to detect when traders are executing trades
	Which generate unusual levels of profits
	By measuring their security trade profits as a percentage return

Background:
	Given I have the high profit rule parameter values
	| WindowHours | HighProfitPercentage | HighProfitAbsolute | HighProfitCurrency | HighProfitUseCurrencyConversions |
	| 1           | 0.01                 |                    |                    |                                  |

@highprofit
@highprofitpercentage
@highprofitpercentagesensitive
Scenario: Empty Universe yields no alerts
	Given I have the orders for a universe from 01/01/2018 to 03/01/2018 :
	| SecurityName | OrderId | PlacedDate          | BookedDate | AmendedDate | RejectedDate | CancelledDate | FilledDate          | Type   | Direction | Currency | LimitPrice | AverageFillPrice | OrderedVolume | FilledVolume |
	When I run the high profit rule
	Then I will have 0 high profit alerts

