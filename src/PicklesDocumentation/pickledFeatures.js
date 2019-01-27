jsonPWrapper ({
  "Features": [
    {
      "RelativeFolder": "WashTrade.AverageNetting.Sensitive.feature",
      "Feature": {
        "Name": "WashTrade Average Netting Sensitive Parameters",
        "Description": "In order to meet MAR compliance requirements\r\nI need to be able to detect when traders are executing trades\r\nwith no meaningful change of ownership",
        "FeatureElements": [
          {
            "Name": "Empty Universe yields no alerts",
            "Slug": "empty-universe-yields-no-alerts",
            "Description": "",
            "Steps": [
              {
                "Keyword": "Given",
                "NativeKeyword": "Given ",
                "Name": "I have the empty universe",
                "StepComments": [],
                "AfterLastStepComments": []
              },
              {
                "Keyword": "When",
                "NativeKeyword": "When ",
                "Name": "I run the wash trade rule",
                "StepComments": [],
                "AfterLastStepComments": []
              },
              {
                "Keyword": "Then",
                "NativeKeyword": "Then ",
                "Name": "I will have 0 wash trade alerts",
                "StepComments": [],
                "AfterLastStepComments": []
              }
            ],
            "Tags": [
              "@washtrade"
            ],
            "Result": {
              "WasExecuted": false,
              "WasSuccessful": false,
              "WasProvided": false
            }
          },
          {
            "Name": "Two Trades In Wash Trade Universe yields one alert",
            "Slug": "two-trades-in-wash-trade-universe-yields-one-alert",
            "Description": "",
            "Steps": [
              {
                "Keyword": "Given",
                "NativeKeyword": "Given ",
                "Name": "I have the buy sell universe",
                "StepComments": [],
                "AfterLastStepComments": []
              },
              {
                "Keyword": "When",
                "NativeKeyword": "When ",
                "Name": "I run the wash trade rule",
                "StepComments": [],
                "AfterLastStepComments": []
              },
              {
                "Keyword": "Then",
                "NativeKeyword": "Then ",
                "Name": "I will have 1 wash trade alerts",
                "StepComments": [],
                "AfterLastStepComments": []
              }
            ],
            "Tags": [
              "@washtrade"
            ],
            "Result": {
              "WasExecuted": false,
              "WasSuccessful": false,
              "WasProvided": false
            }
          },
          {
            "Name": "Three Trades In Wash Trade Universe yields no alerts",
            "Slug": "three-trades-in-wash-trade-universe-yields-no-alerts",
            "Description": "",
            "Steps": [
              {
                "Keyword": "Given",
                "NativeKeyword": "Given ",
                "Name": "I have the buy buy sell universe",
                "StepComments": [],
                "AfterLastStepComments": []
              },
              {
                "Keyword": "When",
                "NativeKeyword": "When ",
                "Name": "I run the wash trade rule",
                "StepComments": [],
                "AfterLastStepComments": []
              },
              {
                "Keyword": "Then",
                "NativeKeyword": "Then ",
                "Name": "I will have 0 wash trade alerts",
                "StepComments": [],
                "AfterLastStepComments": []
              }
            ],
            "Tags": [
              "@washtrade"
            ],
            "Result": {
              "WasExecuted": false,
              "WasSuccessful": false,
              "WasProvided": false
            }
          },
          {
            "Name": "Buy1 Sell1 at Price1 and Buy2 Sell2 and Price2 yields two alerts",
            "Slug": "buy1-sell1-at-price1-and-buy2-sell2-and-price2-yields-two-alerts",
            "Description": "",
            "Steps": [
              {
                "Keyword": "Given",
                "NativeKeyword": "Given ",
                "Name": "I have the buy sell at p1 buy sell at p2 universe",
                "StepComments": [],
                "AfterLastStepComments": []
              },
              {
                "Keyword": "When",
                "NativeKeyword": "When ",
                "Name": "I run the wash trade rule",
                "StepComments": [],
                "AfterLastStepComments": []
              },
              {
                "Keyword": "Then",
                "NativeKeyword": "Then ",
                "Name": "I will have 2 wash trade alerts",
                "StepComments": [],
                "AfterLastStepComments": []
              }
            ],
            "Tags": [
              "@washtrade"
            ],
            "Result": {
              "WasExecuted": false,
              "WasSuccessful": false,
              "WasProvided": false
            }
          },
          {
            "Name": "Buy1 Sell1 at Price1 and Buy2 Sell2 and Price2 and Buy3 but no sell yields two alerts",
            "Slug": "buy1-sell1-at-price1-and-buy2-sell2-and-price2-and-buy3-but-no-sell-yields-two-alerts",
            "Description": "",
            "Steps": [
              {
                "Keyword": "Given",
                "NativeKeyword": "Given ",
                "Name": "I have the buy sell at p1 buy sell at p2 buy at p3 universe",
                "StepComments": [],
                "AfterLastStepComments": []
              },
              {
                "Keyword": "When",
                "NativeKeyword": "When ",
                "Name": "I run the wash trade rule",
                "StepComments": [],
                "AfterLastStepComments": []
              },
              {
                "Keyword": "Then",
                "NativeKeyword": "Then ",
                "Name": "I will have 2 wash trade alerts",
                "StepComments": [],
                "AfterLastStepComments": []
              }
            ],
            "Tags": [
              "@washtrade"
            ],
            "Result": {
              "WasExecuted": false,
              "WasSuccessful": false,
              "WasProvided": false
            }
          }
        ],
        "Background": {
          "Name": "",
          "Description": "",
          "Steps": [
            {
              "Keyword": "Given",
              "NativeKeyword": "Given ",
              "Name": "I have the wash trade rule average netting parameter values:",
              "TableArgument": {
                "HeaderRow": [
                  "window hours",
                  "minimum number of trades",
                  "maximum position value change",
                  "maximum absolute value change",
                  "maximum absolute value change currency"
                ],
                "DataRows": [
                  [
                    "1",
                    "2",
                    "0.01",
                    "10000",
                    "GBP"
                  ]
                ]
              },
              "StepComments": [],
              "AfterLastStepComments": []
            }
          ],
          "Tags": [],
          "Result": {
            "WasExecuted": false,
            "WasSuccessful": false,
            "WasProvided": false
          }
        },
        "Result": {
          "WasExecuted": false,
          "WasSuccessful": false,
          "WasProvided": false
        },
        "Tags": []
      },
      "Result": {
        "WasExecuted": false,
        "WasSuccessful": false,
        "WasProvided": false
      }
    }
  ],
  "Summary": {
    "Tags": [
      {
        "Tag": "@washtrade",
        "Total": 5,
        "Passing": 0,
        "Failing": 0,
        "Inconclusive": 5
      }
    ],
    "Folders": [
      {
        "Folder": "WashTrade.AverageNetting.Sensitive.feature",
        "Total": 5,
        "Passing": 0,
        "Failing": 0,
        "Inconclusive": 5
      }
    ],
    "NotTestedFolders": [
      {
        "Folder": "WashTrade.AverageNetting.Sensitive.feature",
        "Total": 0,
        "Passing": 0,
        "Failing": 0,
        "Inconclusive": 0
      }
    ],
    "Scenarios": {
      "Total": 5,
      "Passing": 0,
      "Failing": 0,
      "Inconclusive": 5
    },
    "Features": {
      "Total": 1,
      "Passing": 0,
      "Failing": 0,
      "Inconclusive": 1
    }
  },
  "Configuration": {
    "GeneratedOn": "27 January 2019 16:53:22"
  }
});