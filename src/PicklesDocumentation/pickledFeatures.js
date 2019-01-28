jsonPWrapper ({
  "Features": [
    {
      "RelativeFolder": "WashTrade.AverageNetting.NonSensitive.feature",
      "Feature": {
        "Name": "WashTrade Average Netting Non-Sensitive Parameters",
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
              "@washtradenonsensitive"
            ],
            "Result": {
              "WasExecuted": false,
              "WasSuccessful": false,
              "WasProvided": false
            }
          },
          {
            "Name": "One Trade For Vodafone yields no alerts",
            "Slug": "one-trade-for-vodafone-yields-no-alerts",
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
                      "100",
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
              "@washtradenonsensitive"
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
              "Name": "I have the wash trade rule parameter values",
              "TableArgument": {
                "HeaderRow": [
                  "WindowHours",
                  "MinimumNumberOfTrades",
                  "MaximumPositionChangeValue",
                  "MaximumAbsoluteValueChange",
                  "MaximumAbsoluteValueChangeCurrency",
                  "UseAverageNetting"
                ],
                "DataRows": [
                  [
                    "1",
                    "2",
                    "0.10",
                    "1000000",
                    "GBX",
                    "true"
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
    },
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
            "Name": "One Trade For Vodafone yields no alerts",
            "Slug": "one-trade-for-vodafone-yields-no-alerts",
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
                      "100",
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
            "Name": "One Trade For Barclays yields no alerts",
            "Slug": "one-trade-for-barclays-yields-no-alerts",
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
                      "Barclays",
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
                      "100",
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
            "Name": "Two Trades In Wash Trade yields one alert",
            "Slug": "two-trades-in-wash-trade-yields-one-alert",
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
                      "100",
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
                      "100",
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
            "Name": "Two Trades In Wash Trade For Different Securities yields one alert",
            "Slug": "two-trades-in-wash-trade-for-different-securities-yields-one-alert",
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
                      "100",
                      "1000",
                      "1000"
                    ],
                    [
                      "Barclays",
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
                      "100",
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
            "Name": "Three Trades at same price point In Wash Trade yields no alerts",
            "Slug": "three-trades-at-same-price-point-in-wash-trade-yields-no-alerts",
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
                      "100",
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
                      "100",
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
                      "100",
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
                      "100",
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
                      "100",
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
                      "200",
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
                      "200",
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
                      "100",
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
                      "100",
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
                      "200",
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
                      "200",
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
                      "300",
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
              "Name": "I have the wash trade rule parameter values",
              "TableArgument": {
                "HeaderRow": [
                  "WindowHours",
                  "MinimumNumberOfTrades",
                  "MaximumPositionChangeValue",
                  "MaximumAbsoluteValueChange",
                  "MaximumAbsoluteValueChangeCurrency",
                  "UseAverageNetting"
                ],
                "DataRows": [
                  [
                    "1",
                    "2",
                    "0.01",
                    "10000",
                    "GBX",
                    "true"
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
    },
    {
      "RelativeFolder": "WashTrade.Clustering.NonSensitive.feature",
      "Feature": {
        "Name": "WashTrade Clustering Non Sensitive Parameters",
        "Description": "In order to meet MAR compliance requirements\r\nI need to be able to detect when traders are executing trades\r\nwith no meaningful change of ownership\r\nBy clustering their trades for average value change being below\r\nthreshold parameters",
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
              "@washtradeclustering",
              "@washtradenonsensitive"
            ],
            "Result": {
              "WasExecuted": false,
              "WasSuccessful": false,
              "WasProvided": false
            }
          },
          {
            "Name": "One Trade For Vodafone yields no alerts",
            "Slug": "one-trade-for-vodafone-yields-no-alerts",
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
                      "100",
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
              "@washtradeclustering",
              "@washtradenonsensitive"
            ],
            "Result": {
              "WasExecuted": false,
              "WasSuccessful": false,
              "WasProvided": false
            }
          },
          {
            "Name": "One Trade For Barclays yields no alerts",
            "Slug": "one-trade-for-barclays-yields-no-alerts",
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
                      "Barclays",
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
                      "100",
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
              "@washtradeclustering",
              "@washtradenonsensitive"
            ],
            "Result": {
              "WasExecuted": false,
              "WasSuccessful": false,
              "WasProvided": false
            }
          },
          {
            "Name": "Two Trades In Wash Trade For Different Securities yields one alert",
            "Slug": "two-trades-in-wash-trade-for-different-securities-yields-one-alert",
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
                      "100",
                      "1000",
                      "1000"
                    ],
                    [
                      "Barclays",
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
                      "100",
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
              "@washtradeclustering",
              "@washtradenonsensitive"
            ],
            "Result": {
              "WasExecuted": false,
              "WasSuccessful": false,
              "WasProvided": false
            }
          },
          {
            "Name": "Three Trades at same price point In Wash Trade yields no alerts",
            "Slug": "three-trades-at-same-price-point-in-wash-trade-yields-no-alerts",
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
                      "100",
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
                      "100",
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
                      "100",
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
              "@washtradeclustering",
              "@washtradenonsensitive"
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
              "Name": "I have the wash trade rule parameter values",
              "TableArgument": {
                "HeaderRow": [
                  "WindowHours",
                  "ClusteringPositionMinimumNumberOfTrades",
                  "ClusteringPercentageValueDifferenceThreshold",
                  "UseClustering"
                ],
                "DataRows": [
                  [
                    "1",
                    "2",
                    "0.10",
                    "true"
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
    },
    {
      "RelativeFolder": "WashTrade.Clustering.Sensitive.feature",
      "Feature": {
        "Name": "WashTrade Clustering Sensitive Parameters",
        "Description": "In order to meet MAR compliance requirements\r\nI need to be able to detect when traders are executing trades\r\nwith no meaningful change of ownership\r\nBy clustering their trades for average value change being below\r\nthreshold parameters",
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
              "@washtradeclustering",
              "@washtradesensitive"
            ],
            "Result": {
              "WasExecuted": false,
              "WasSuccessful": false,
              "WasProvided": false
            }
          },
          {
            "Name": "One Trade For Vodafone yields no alerts",
            "Slug": "one-trade-for-vodafone-yields-no-alerts",
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
                      "100",
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
              "@washtradeclustering",
              "@washtradesensitive"
            ],
            "Result": {
              "WasExecuted": false,
              "WasSuccessful": false,
              "WasProvided": false
            }
          },
          {
            "Name": "One Trade For Barclays yields no alerts",
            "Slug": "one-trade-for-barclays-yields-no-alerts",
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
                      "Barclays",
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
                      "100",
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
              "@washtradeclustering",
              "@washtradesensitive"
            ],
            "Result": {
              "WasExecuted": false,
              "WasSuccessful": false,
              "WasProvided": false
            }
          },
          {
            "Name": "Two Trades In Wash Trade For Different Securities yields one alert",
            "Slug": "two-trades-in-wash-trade-for-different-securities-yields-one-alert",
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
                      "100",
                      "1000",
                      "1000"
                    ],
                    [
                      "Barclays",
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
                      "100",
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
              "@washtradeclustering",
              "@washtradesensitive"
            ],
            "Result": {
              "WasExecuted": false,
              "WasSuccessful": false,
              "WasProvided": false
            }
          },
          {
            "Name": "Three Trades at same price point In Wash Trade yields no alerts",
            "Slug": "three-trades-at-same-price-point-in-wash-trade-yields-no-alerts",
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
                      "100",
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
                      "100",
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
                      "100",
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
              "@washtradeclustering",
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
              "Name": "I have the wash trade rule parameter values",
              "TableArgument": {
                "HeaderRow": [
                  "WindowHours",
                  "ClusteringPositionMinimumNumberOfTrades",
                  "ClusteringPercentageValueDifferenceThreshold",
                  "UseClustering"
                ],
                "DataRows": [
                  [
                    "1",
                    "2",
                    "0.03",
                    "true"
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
    },
    {
      "RelativeFolder": "WashTrade.Pairing.NonSensitive.feature",
      "Feature": {
        "Name": "WashTrade Pairing Non Sensitive Parameters",
        "Description": "In order to meet MAR compliance requirements\r\nI need to be able to detect when traders are executing trades\r\nwith no meaningful change of ownership\r\nBy pairing their trades for average value change being below\r\nthreshold parameters",
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
              "@washtradepairing",
              "@washtradenonsensitive"
            ],
            "Result": {
              "WasExecuted": false,
              "WasSuccessful": false,
              "WasProvided": false
            }
          },
          {
            "Name": "One Trade For Vodafone yields no alerts",
            "Slug": "one-trade-for-vodafone-yields-no-alerts",
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
                      "100",
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
              "@washtradepairing",
              "@washtradenonsensitive"
            ],
            "Result": {
              "WasExecuted": false,
              "WasSuccessful": false,
              "WasProvided": false
            }
          },
          {
            "Name": "One Trade For Barclays yields no alerts",
            "Slug": "one-trade-for-barclays-yields-no-alerts",
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
                      "Barclays",
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
                      "100",
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
              "@washtradepairing",
              "@washtradenonsensitive"
            ],
            "Result": {
              "WasExecuted": false,
              "WasSuccessful": false,
              "WasProvided": false
            }
          },
          {
            "Name": "Two Trades In Wash Trade For Different Securities yields no alert",
            "Slug": "two-trades-in-wash-trade-for-different-securities-yields-no-alert",
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
                      "100",
                      "1000",
                      "1000"
                    ],
                    [
                      "Barclays",
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
                      "100",
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
              "@washtradepairing",
              "@washtradenonsensitive"
            ],
            "Result": {
              "WasExecuted": false,
              "WasSuccessful": false,
              "WasProvided": false
            }
          },
          {
            "Name": "Three Trades at same price point In Wash Trade yields no alerts",
            "Slug": "three-trades-at-same-price-point-in-wash-trade-yields-no-alerts",
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
                      "100",
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
                      "100",
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
                      "100",
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
              "@washtradepairing",
              "@washtradenonsensitive"
            ],
            "Result": {
              "WasExecuted": false,
              "WasSuccessful": false,
              "WasProvided": false
            }
          },
          {
            "Name": "Two Trades In Wash Trade yields one alert",
            "Slug": "two-trades-in-wash-trade-yields-one-alert",
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
                      "100",
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
                      "100",
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
              "@washtradepairing",
              "@washtradenonsensitive"
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
              "Name": "I have the wash trade rule parameter values",
              "TableArgument": {
                "HeaderRow": [
                  "WindowHours",
                  "PairingPositionMinimumNumberOfPairedTrades",
                  "PairingPositionPercentagePriceChangeThresholdPerPair",
                  "PairingPositionPercentageVolumeDifferenceThreshold",
                  "PairingPositionMaximumAbsoluteCurrencyAmount",
                  "PairingPositionMaximumAbsoluteCurrency",
                  "UsePairing"
                ],
                "DataRows": [
                  [
                    "1",
                    "2",
                    "0.10",
                    "0.10",
                    "1000000",
                    "GBX",
                    "true"
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
    },
    {
      "RelativeFolder": "WashTrade.Pairing.Sensitive.feature",
      "Feature": {
        "Name": "WashTrade Pairing Sensitive Parameters",
        "Description": "In order to meet MAR compliance requirements\r\nI need to be able to detect when traders are executing trades\r\nwith no meaningful change of ownership\r\nBy pairing their trades for average value change being below\r\nthreshold parameters",
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
              "@washtradepairing",
              "@washtradesensitive"
            ],
            "Result": {
              "WasExecuted": false,
              "WasSuccessful": false,
              "WasProvided": false
            }
          },
          {
            "Name": "One Trade For Vodafone yields no alerts",
            "Slug": "one-trade-for-vodafone-yields-no-alerts",
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
                      "100",
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
              "@washtradepairing",
              "@washtradesensitive"
            ],
            "Result": {
              "WasExecuted": false,
              "WasSuccessful": false,
              "WasProvided": false
            }
          },
          {
            "Name": "One Trade For Barclays yields no alerts",
            "Slug": "one-trade-for-barclays-yields-no-alerts",
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
                      "Barclays",
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
                      "100",
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
              "@washtradepairing",
              "@washtradesensitive"
            ],
            "Result": {
              "WasExecuted": false,
              "WasSuccessful": false,
              "WasProvided": false
            }
          },
          {
            "Name": "Two Trades In Wash Trade For Different Securities yields one alert",
            "Slug": "two-trades-in-wash-trade-for-different-securities-yields-one-alert",
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
                      "100",
                      "1000",
                      "1000"
                    ],
                    [
                      "Barclays",
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
                      "100",
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
              "@washtradepairing",
              "@washtradesensitive"
            ],
            "Result": {
              "WasExecuted": false,
              "WasSuccessful": false,
              "WasProvided": false
            }
          },
          {
            "Name": "Three Trades at same price point In Wash Trade yields no alerts",
            "Slug": "three-trades-at-same-price-point-in-wash-trade-yields-no-alerts",
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
                      "100",
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
                      "100",
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
                      "100",
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
              "@washtradepairing",
              "@washtradesensitive"
            ],
            "Result": {
              "WasExecuted": false,
              "WasSuccessful": false,
              "WasProvided": false
            }
          },
          {
            "Name": "Two Trades In Wash Trade yields one alert",
            "Slug": "two-trades-in-wash-trade-yields-one-alert",
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
                      "100",
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
                      "100",
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
              "@washtradepairing",
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
              "Name": "I have the wash trade rule parameter values",
              "TableArgument": {
                "HeaderRow": [
                  "WindowHours",
                  "PairingPositionMinimumNumberOfPairedTrades",
                  "PairingPositionPercentagePriceChangeThresholdPerPair",
                  "PairingPositionPercentageVolumeDifferenceThreshold",
                  "PairingPositionMaximumAbsoluteCurrencyAmount",
                  "PairingPositionMaximumAbsoluteCurrency",
                  "UsePairing"
                ],
                "DataRows": [
                  [
                    "1",
                    "2",
                    "0.03",
                    "0.03",
                    "10000",
                    "GBX",
                    "true"
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
        "Total": 32,
        "Passing": 0,
        "Failing": 0,
        "Inconclusive": 32
      },
      {
        "Tag": "@washtradenonsensitive",
        "Total": 13,
        "Passing": 0,
        "Failing": 0,
        "Inconclusive": 13
      },
      {
        "Tag": "@washtradesensitive",
        "Total": 19,
        "Passing": 0,
        "Failing": 0,
        "Inconclusive": 19
      },
      {
        "Tag": "@washtradeclustering",
        "Total": 10,
        "Passing": 0,
        "Failing": 0,
        "Inconclusive": 10
      },
      {
        "Tag": "@washtradepairing",
        "Total": 12,
        "Passing": 0,
        "Failing": 0,
        "Inconclusive": 12
      }
    ],
    "Folders": [
      {
        "Folder": "WashTrade.AverageNetting.NonSensitive.feature",
        "Total": 2,
        "Passing": 0,
        "Failing": 0,
        "Inconclusive": 2
      },
      {
        "Folder": "WashTrade.AverageNetting.Sensitive.feature",
        "Total": 8,
        "Passing": 0,
        "Failing": 0,
        "Inconclusive": 8
      },
      {
        "Folder": "WashTrade.Clustering.NonSensitive.feature",
        "Total": 5,
        "Passing": 0,
        "Failing": 0,
        "Inconclusive": 5
      },
      {
        "Folder": "WashTrade.Clustering.Sensitive.feature",
        "Total": 5,
        "Passing": 0,
        "Failing": 0,
        "Inconclusive": 5
      },
      {
        "Folder": "WashTrade.Pairing.NonSensitive.feature",
        "Total": 6,
        "Passing": 0,
        "Failing": 0,
        "Inconclusive": 6
      },
      {
        "Folder": "WashTrade.Pairing.Sensitive.feature",
        "Total": 6,
        "Passing": 0,
        "Failing": 0,
        "Inconclusive": 6
      }
    ],
    "NotTestedFolders": [
      {
        "Folder": "WashTrade.AverageNetting.NonSensitive.feature",
        "Total": 0,
        "Passing": 0,
        "Failing": 0,
        "Inconclusive": 0
      },
      {
        "Folder": "WashTrade.AverageNetting.Sensitive.feature",
        "Total": 0,
        "Passing": 0,
        "Failing": 0,
        "Inconclusive": 0
      },
      {
        "Folder": "WashTrade.Clustering.NonSensitive.feature",
        "Total": 0,
        "Passing": 0,
        "Failing": 0,
        "Inconclusive": 0
      },
      {
        "Folder": "WashTrade.Clustering.Sensitive.feature",
        "Total": 0,
        "Passing": 0,
        "Failing": 0,
        "Inconclusive": 0
      },
      {
        "Folder": "WashTrade.Pairing.NonSensitive.feature",
        "Total": 0,
        "Passing": 0,
        "Failing": 0,
        "Inconclusive": 0
      },
      {
        "Folder": "WashTrade.Pairing.Sensitive.feature",
        "Total": 0,
        "Passing": 0,
        "Failing": 0,
        "Inconclusive": 0
      }
    ],
    "Scenarios": {
      "Total": 32,
      "Passing": 0,
      "Failing": 0,
      "Inconclusive": 32
    },
    "Features": {
      "Total": 6,
      "Passing": 0,
      "Failing": 0,
      "Inconclusive": 6
    }
  },
  "Configuration": {
    "GeneratedOn": "28 January 2019 16:25:44"
  }
});