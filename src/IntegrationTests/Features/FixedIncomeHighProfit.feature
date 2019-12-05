Feature: FixedIncomeHighProfit
  Further description

  Scenario: Basic fixed income high profit rule run
    Given the fixed income high profit core settings
      | ProfitThresholdPercent | PriceType | BackwardWindow | ForwardWindow |
      | 20                     | Close     | 1 day          | 1 day         |
    And the orders
      | OrderId | _Date               | _FixedIncomeSecurity | OrderDirection | OrderAverageFillPrice | _Volume | OrderCleanDirty |
      | 0       | 2018-01-01T15:00:00 | BOND123              | BUY            | 100                   | 100     | CLEAN           |
    And the fixed income close prices
      | Date       | _FixedIncomeSecurity | ClosePrice |
      | 2018-01-01 | BOND123              | 130        |
    When the rule is run between "2018-01-01" and "2018-01-01"
    Then there should be a breach with order ids "0"

  Scenario: Fixed income high profit rule run without a breach
    Given the fixed income high profit core settings
      | ProfitThresholdPercent | PriceType | BackwardWindow | ForwardWindow |
      | 20                     | Close     | 1 day          | 1 day         |
    And the orders
      | OrderId | _Date               | _FixedIncomeSecurity | OrderDirection | OrderAverageFillPrice | _Volume | OrderCleanDirty |
      | 0       | 2018-01-01T15:00:00 | BOND123              | BUY            | 100                   | 100     | CLEAN           |
    And the fixed income close prices
      | Date       | _FixedIncomeSecurity | ClosePrice |
      | 2018-01-01 | BOND123              | 119        |
    When the rule is run between "2018-01-01" and "2018-01-01"
    Then there should be no breaches

	