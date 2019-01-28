jsonPWrapper ({
  "Features": [
    {
      "RelativeFolder": "WashTrade.AverageNetting.Sensitive.feature",
      "Feature": {
        "Name": "WashTrade Average Netting Sensitive Parameters",
        "Description": "In order to meet MAR compliance requirements\r\nI need to be able to detect when traders are executing trades\r\nwith no meaningful change of ownership\r\nBy netting their trades for average value change being below\r\nthreshold parameters",
        "FeatureElements": [
          {
            "Name": "Empty Universe yields no alerts",
            "Slug": "empty-universe-yields-no-alerts",
            "Description": "",
            "Steps": [
              {
                "Keyword": "Given",
                "NativeKeyword": "Given ",
                "Name": "I have the orders for a universe from 01/01/2018 to 03/01/2018 :",
                "TableArgument": {
                  "HeaderRow": [
                    "SecurityName",
                    "OrderId",
                    "PlacedDate",
                    "BookedDate",
                    "AmendedDate",
                    "RejectedDate",
                    "CancelledDate",
                    "FilledDate",
                    "Type",
                    "Direction",
                    "Currency",
                    "LimitPrice",
                    "AverageFillPrice",
                    "OrderedVolume",
                    "FilledVolume"
                  ],
                  "DataRows": []
                },
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
              "@washtrade",
              "@washtradesensitive"
            ],
            "Result": {
              "WasExecuted": false,
              "WasSuccessful": false,
              "WasProvided": false
            }
          },
          {
            "Name": "One Trade Universe yields no alerts",
            "Slug": "one-trade-universe-yields-no-alerts",
            "Description": "",
            "Steps": [
              {
                "Keyword": "Given",
                "NativeKeyword": "Given ",
                "Name": "I have the orders for a universe from 01/01/2018 to 03/01/2018 :",
                "TableArgument": {
                  "HeaderRow": [
                    "SecurityName",
                    "OrderId",
                    "PlacedDate",
                    "BookedDate",
                    "AmendedDate",
                    "RejectedDate",
                    "CancelledDate",
                    "FilledDate",
                    "Type",
                    "Direction",
                    "Currency",
                    "LimitPrice",
                    "AverageFillPrice",
                    "OrderedVolume",
                    "FilledVolume"
                  ],
                  "DataRows": [
                    [
                      "Vodafone",
                      "0",
                      "01/01/2018 09:30:00",
                      "",
                      "",
                      "",
                      "",
                      "01/01/2018 09:30:00",
                      "MARKET",
                      "BUY",
                      "GBX",
                      "",
                      "10.01",
                      "1000",
                      "1000"
                    ]
                  ]
                },
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
              "@washtrade",
              "@washtradesensitive"
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
                "Name": "I have the orders for a universe from 01/01/2018 to 03/01/2018 :",
                "TableArgument": {
                  "HeaderRow": [
                    "SecurityName",
                    "OrderId",
                    "PlacedDate",
                    "BookedDate",
                    "AmendedDate",
                    "RejectedDate",
                    "CancelledDate",
                    "FilledDate",
                    "Type",
                    "Direction",
                    "Currency",
                    "LimitPrice",
                    "AverageFillPrice",
                    "OrderedVolume",
                    "FilledVolume"
                  ],
                  "DataRows": [
                    [
                      "Vodafone",
                      "0",
                      "01/01/2018 09:30:00",
                      "",
                      "",
                      "",
                      "",
                      "01/01/2018 09:30:00",
                      "MARKET",
                      "BUY",
                      "GBX",
                      "",
                      "10.01",
                      "1000",
                      "1000"
                    ],
                    [
                      "Vodafone",
                      "1",
                      "01/01/2018 09:30:00",
                      "",
                      "",
                      "",
                      "",
                      "01/01/2018 09:30:00",
                      "MARKET",
                      "SELL",
                      "GBX",
                      "",
                      "10.01",
                      "1000",
                      "1000"
                    ]
                  ]
                },
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
              "@washtrade",
              "@washtradesensitive"
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
                "Name": "I have the orders for a universe from 01/01/2018 to 03/01/2018 :",
                "TableArgument": {
                  "HeaderRow": [
                    "SecurityName",
                    "OrderId",
                    "PlacedDate",
                    "BookedDate",
                    "AmendedDate",
                    "RejectedDate",
                    "CancelledDate",
                    "FilledDate",
                    "Type",
                    "Direction",
                    "Currency",
                    "LimitPrice",
                    "AverageFillPrice",
                    "OrderedVolume",
                    "FilledVolume"
                  ],
                  "DataRows": [
                    [
                      "Vodafone",
                      "0",
                      "01/01/2018 09:30:00",
                      "",
                      "",
                      "",
                      "",
                      "01/01/2018 09:30:00",
                      "MARKET",
                      "BUY",
                      "GBX",
                      "",
                      "10.01",
                      "1000",
                      "1000"
                    ],
                    [
                      "Vodafone",
                      "1",
                      "01/01/2018 09:30:00",
                      "",
                      "",
                      "",
                      "",
                      "01/01/2018 09:30:00",
                      "MARKET",
                      "BUY",
                      "GBX",
                      "",
                      "10.01",
                      "1000",
                      "1000"
                    ],
                    [
                      "Vodafone",
                      "2",
                      "01/01/2018 09:30:00",
                      "",
                      "",
                      "",
                      "",
                      "01/01/2018 09:30:00",
                      "MARKET",
                      "SELL",
                      "GBX",
                      "",
                      "10.01",
                      "1000",
                      "1000"
                    ]
                  ]
                },
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
              "@washtrade",
              "@washtradesensitive"
            ],
            "Result": {
              "WasExecuted": false,
              "WasSuccessful": false,
              "WasProvided": false
            }
          },
          {
            "Name": "Four trades at two price points yields two alerts",
            "Slug": "four-trades-at-two-price-points-yields-two-alerts",
            "Description": "",
            "Steps": [
              {
                "Keyword": "Given",
                "NativeKeyword": "Given ",
                "Name": "I have the orders for a universe from 01/01/2018 to 03/01/2018 :",
                "TableArgument": {
                  "HeaderRow": [
                    "SecurityName",
                    "OrderId",
                    "PlacedDate",
                    "BookedDate",
                    "AmendedDate",
                    "RejectedDate",
                    "CancelledDate",
                    "FilledDate",
                    "Type",
                    "Direction",
                    "Currency",
                    "LimitPrice",
                    "AverageFillPrice",
                    "OrderedVolume",
                    "FilledVolume"
                  ],
                  "DataRows": [
                    [
                      "Vodafone",
                      "0",
                      "01/01/2018 09:30:00",
                      "",
                      "",
                      "",
                      "",
                      "01/01/2018 09:30:00",
                      "MARKET",
                      "BUY",
                      "GBX",
                      "",
                      "10.01",
                      "1000",
                      "1000"
                    ],
                    [
                      "Vodafone",
                      "1",
                      "01/01/2018 09:30:00",
                      "",
                      "",
                      "",
                      "",
                      "01/01/2018 09:30:00",
                      "MARKET",
                      "SELL",
                      "GBX",
                      "",
                      "10.01",
                      "1000",
                      "1000"
                    ],
                    [
                      "Vodafone",
                      "2",
                      "01/01/2018 09:35:00",
                      "",
                      "",
                      "",
                      "",
                      "01/01/2018 09:35:00",
                      "MARKET",
                      "BUY",
                      "GBX",
                      "",
                      "20.01",
                      "1000",
                      "1000"
                    ],
                    [
                      "Vodafone",
                      "3",
                      "01/01/2018 09:35:00",
                      "",
                      "",
                      "",
                      "",
                      "01/01/2018 09:35:00",
                      "MARKET",
                      "SELL",
                      "GBX",
                      "",
                      "20.01",
                      "1000",
                      "1000"
                    ]
                  ]
                },
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
              "@washtrade",
              "@washtradesensitive"
            ],
            "Result": {
              "WasExecuted": false,
              "WasSuccessful": false,
              "WasProvided": false
            }
          },
          {
            "Name": "Five trades in two pairs with a single trade per three price points yields two alerts",
            "Slug": "five-trades-in-two-pairs-with-a-single-trade-per-three-price-points-yields-two-alerts",
            "Description": "",
            "Steps": [
              {
                "Keyword": "Given",
                "NativeKeyword": "Given ",
                "Name": "I have the orders for a universe from 01/01/2018 to 03/01/2018 :",
                "TableArgument": {
                  "HeaderRow": [
                    "SecurityName",
                    "OrderId",
                    "PlacedDate",
                    "BookedDate",
                    "AmendedDate",
                    "RejectedDate",
                    "CancelledDate",
                    "FilledDate",
                    "Type",
                    "Direction",
                    "Currency",
                    "LimitPrice",
                    "AverageFillPrice",
                    "OrderedVolume",
                    "FilledVolume"
                  ],
                  "DataRows": [
                    [
                      "Vodafone",
                      "0",
                      "01/01/2018 09:30:00",
                      "",
                      "",
                      "",
                      "",
                      "01/01/2018 09:30:00",
                      "MARKET",
                      "BUY",
                      "GBX",
                      "",
                      "10.01",
                      "1000",
                      "1000"
                    ],
                    [
                      "Vodafone",
                      "1",
                      "01/01/2018 09:30:00",
                      "",
                      "",
                      "",
                      "",
                      "01/01/2018 09:30:00",
                      "MARKET",
                      "SELL",
                      "GBX",
                      "",
                      "10.01",
                      "1000",
                      "1000"
                    ],
                    [
                      "Vodafone",
                      "2",
                      "01/01/2018 09:35:00",
                      "",
                      "",
                      "",
                      "",
                      "01/01/2018 09:35:00",
                      "MARKET",
                      "BUY",
                      "GBX",
                      "",
                      "20.01",
                      "1000",
                      "1000"
                    ],
                    [
                      "Vodafone",
                      "3",
                      "01/01/2018 09:35:00",
                      "",
                      "",
                      "",
                      "",
                      "01/01/2018 09:35:00",
                      "MARKET",
                      "SELL",
                      "GBX",
                      "",
                      "20.01",
                      "1000",
                      "1000"
                    ],
                    [
                      "Vodafone",
                      "4",
                      "01/01/2018 09:40:00",
                      "",
                      "",
                      "",
                      "",
                      "01/01/2018 09:40:00",
                      "MARKET",
                      "BUY",
                      "GBX",
                      "",
                      "30.01",
                      "1000",
                      "1000"
                    ]
                  ]
                },
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
              "@washtrade",
              "@washtradesensitive"
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
              "Name": "I have the wash trade rule average netting parameter values",
              "TableArgument": {
                "HeaderRow": [
                  "WindowHours",
                  "MinimumNumberOfTrades",
                  "MaximumPositionChangeValue",
                  "MaximumAbsoluteValueChange",
                  "MaximumAbsoluteValueChangeCurrency"
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
        "Total": 6,
        "Passing": 0,
        "Failing": 0,
        "Inconclusive": 6
      },
      {
        "Tag": "@washtradesensitive",
        "Total": 6,
        "Passing": 0,
        "Failing": 0,
        "Inconclusive": 6
      }
    ],
    "Folders": [
      {
        "Folder": "WashTrade.AverageNetting.Sensitive.feature",
        "Total": 6,
        "Passing": 0,
        "Failing": 0,
        "Inconclusive": 6
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
      "Total": 6,
      "Passing": 0,
      "Failing": 0,
      "Inconclusive": 6
    },
    "Features": {
      "Total": 1,
      "Passing": 0,
      "Failing": 0,
      "Inconclusive": 1
    }
  },
  "Configuration": {
    "GeneratedOn": "28 January 2019 13:25:26"
  }
});