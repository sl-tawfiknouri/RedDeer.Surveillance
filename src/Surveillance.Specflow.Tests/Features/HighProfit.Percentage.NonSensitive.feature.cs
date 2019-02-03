// ------------------------------------------------------------------------------
//  <auto-generated>
//      This code was generated by SpecFlow (http://www.specflow.org/).
//      SpecFlow Version:3.0.0.0
//      SpecFlow Generator Version:3.0.0.0
// 
//      Changes to this file may cause incorrect behavior and will be lost if
//      the code is regenerated.
//  </auto-generated>
// ------------------------------------------------------------------------------
#region Designer generated code
#pragma warning disable
namespace Surveillance.Specflow.Tests.Features
{
    using TechTalk.SpecFlow;
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("TechTalk.SpecFlow", "3.0.0.0")]
    [System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [NUnit.Framework.TestFixtureAttribute()]
    [NUnit.Framework.DescriptionAttribute("HighProfit Percentage Non Sensitive Parameters")]
    public partial class HighProfitPercentageNonSensitiveParametersFeature
    {
        
        private TechTalk.SpecFlow.ITestRunner testRunner;
        
#line 1 "HighProfit.Percentage.NonSensitive.feature"
#line hidden
        
        [NUnit.Framework.OneTimeSetUpAttribute()]
        public virtual void FeatureSetup()
        {
            testRunner = TechTalk.SpecFlow.TestRunnerManager.GetTestRunner();
            TechTalk.SpecFlow.FeatureInfo featureInfo = new TechTalk.SpecFlow.FeatureInfo(new System.Globalization.CultureInfo("en-US"), "HighProfit Percentage Non Sensitive Parameters", "\tIn order to meet MAR compliance requirements\r\n\tI need to be able to detect when " +
                    "traders are executing trades\r\n\tWhich generate unusual levels of profits\r\n\tBy mea" +
                    "suring their security trade profits as a percentage return", ProgrammingLanguage.CSharp, ((string[])(null)));
            testRunner.OnFeatureStart(featureInfo);
        }
        
        [NUnit.Framework.OneTimeTearDownAttribute()]
        public virtual void FeatureTearDown()
        {
            testRunner.OnFeatureEnd();
            testRunner = null;
        }
        
        [NUnit.Framework.SetUpAttribute()]
        public virtual void TestInitialize()
        {
        }
        
        [NUnit.Framework.TearDownAttribute()]
        public virtual void ScenarioTearDown()
        {
            testRunner.OnScenarioEnd();
        }
        
        public virtual void ScenarioInitialize(TechTalk.SpecFlow.ScenarioInfo scenarioInfo)
        {
            testRunner.OnScenarioInitialize(scenarioInfo);
            testRunner.ScenarioContext.ScenarioContainer.RegisterInstanceAs<NUnit.Framework.TestContext>(NUnit.Framework.TestContext.CurrentContext);
        }
        
        public virtual void ScenarioStart()
        {
            testRunner.OnScenarioStart();
        }
        
        public virtual void ScenarioCleanup()
        {
            testRunner.CollectScenarioErrors();
        }
        
        public virtual void FeatureBackground()
        {
#line 10
#line hidden
            TechTalk.SpecFlow.Table table1 = new TechTalk.SpecFlow.Table(new string[] {
                        "WindowHours",
                        "HighProfitPercentage",
                        "HighProfitAbsolute",
                        "HighProfitCurrency",
                        "HighProfitUseCurrencyConversions"});
            table1.AddRow(new string[] {
                        "1",
                        "0.1",
                        "",
                        "",
                        ""});
#line 11
 testRunner.Given("I have the high profit rule parameter values", ((string)(null)), table1, "Given ");
#line hidden
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Empty Universe yields no alerts")]
        [NUnit.Framework.CategoryAttribute("highprofit")]
        [NUnit.Framework.CategoryAttribute("highprofitpercentage")]
        [NUnit.Framework.CategoryAttribute("highprofitpercentagesensitive")]
        public virtual void EmptyUniverseYieldsNoAlerts()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Empty Universe yields no alerts", null, new string[] {
                        "highprofit",
                        "highprofitpercentage",
                        "highprofitpercentagesensitive"});
#line 18
this.ScenarioInitialize(scenarioInfo);
            this.ScenarioStart();
#line 10
this.FeatureBackground();
#line hidden
            TechTalk.SpecFlow.Table table2 = new TechTalk.SpecFlow.Table(new string[] {
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
                        "FilledVolume"});
#line 19
 testRunner.Given("I have the orders for a universe from 01/01/2018 to 03/01/2018 :", ((string)(null)), table2, "Given ");
#line 21
 testRunner.When("I run the high profit rule", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 22
 testRunner.Then("I will have 0 high profit alerts", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Single order yields no alerts")]
        [NUnit.Framework.CategoryAttribute("highprofit")]
        [NUnit.Framework.CategoryAttribute("highprofitpercentage")]
        [NUnit.Framework.CategoryAttribute("highprofitpercentagesensitive")]
        public virtual void SingleOrderYieldsNoAlerts()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Single order yields no alerts", null, new string[] {
                        "highprofit",
                        "highprofitpercentage",
                        "highprofitpercentagesensitive"});
#line 27
this.ScenarioInitialize(scenarioInfo);
            this.ScenarioStart();
#line 10
this.FeatureBackground();
#line hidden
            TechTalk.SpecFlow.Table table3 = new TechTalk.SpecFlow.Table(new string[] {
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
                        "FilledVolume"});
            table3.AddRow(new string[] {
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
                        "10",
                        "100",
                        "100"});
#line 28
 testRunner.Given("I have the orders for a universe from 01/01/2018 to 03/01/2018 :", ((string)(null)), table3, "Given ");
#line 31
 testRunner.When("I run the high profit rule", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 32
 testRunner.Then("I will have 0 high profit alerts", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Buy Sell orders yields two alerts")]
        [NUnit.Framework.CategoryAttribute("highprofit")]
        [NUnit.Framework.CategoryAttribute("highprofitpercentage")]
        [NUnit.Framework.CategoryAttribute("highprofitpercentagesensitive")]
        public virtual void BuySellOrdersYieldsTwoAlerts()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Buy Sell orders yields two alerts", null, new string[] {
                        "highprofit",
                        "highprofitpercentage",
                        "highprofitpercentagesensitive"});
#line 38
this.ScenarioInitialize(scenarioInfo);
            this.ScenarioStart();
#line 10
this.FeatureBackground();
#line hidden
            TechTalk.SpecFlow.Table table4 = new TechTalk.SpecFlow.Table(new string[] {
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
                        "FilledVolume"});
            table4.AddRow(new string[] {
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
                        "10",
                        "100",
                        "100"});
            table4.AddRow(new string[] {
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
                        "12",
                        "100",
                        "100"});
#line 39
 testRunner.Given("I have the orders for a universe from 01/01/2018 to 03/01/2018 :", ((string)(null)), table4, "Given ");
#line 43
 testRunner.When("I run the high profit rule", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 44
 testRunner.Then("I will have 2 high profit alerts", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Buy Sell orders at exact percentage yields two alerts")]
        [NUnit.Framework.CategoryAttribute("highprofit")]
        [NUnit.Framework.CategoryAttribute("highprofitpercentage")]
        [NUnit.Framework.CategoryAttribute("highprofitpercentagesensitive")]
        public virtual void BuySellOrdersAtExactPercentageYieldsTwoAlerts()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Buy Sell orders at exact percentage yields two alerts", null, new string[] {
                        "highprofit",
                        "highprofitpercentage",
                        "highprofitpercentagesensitive"});
#line 49
this.ScenarioInitialize(scenarioInfo);
            this.ScenarioStart();
#line 10
this.FeatureBackground();
#line hidden
            TechTalk.SpecFlow.Table table5 = new TechTalk.SpecFlow.Table(new string[] {
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
                        "FilledVolume"});
            table5.AddRow(new string[] {
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
                        "100",
                        "100"});
            table5.AddRow(new string[] {
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
                        "110",
                        "100",
                        "100"});
#line 50
 testRunner.Given("I have the orders for a universe from 01/01/2018 to 03/01/2018 :", ((string)(null)), table5, "Given ");
#line 54
 testRunner.When("I run the high profit rule", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 55
 testRunner.Then("I will have 2 high profit alerts", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Buy Sell orders at just below percentage yields zero alerts")]
        [NUnit.Framework.CategoryAttribute("highprofit")]
        [NUnit.Framework.CategoryAttribute("highprofitpercentage")]
        [NUnit.Framework.CategoryAttribute("highprofitpercentagesensitive")]
        public virtual void BuySellOrdersAtJustBelowPercentageYieldsZeroAlerts()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Buy Sell orders at just below percentage yields zero alerts", null, new string[] {
                        "highprofit",
                        "highprofitpercentage",
                        "highprofitpercentagesensitive"});
#line 60
this.ScenarioInitialize(scenarioInfo);
            this.ScenarioStart();
#line 10
this.FeatureBackground();
#line hidden
            TechTalk.SpecFlow.Table table6 = new TechTalk.SpecFlow.Table(new string[] {
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
                        "FilledVolume"});
            table6.AddRow(new string[] {
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
                        "1000",
                        "100",
                        "100"});
            table6.AddRow(new string[] {
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
                        "1001",
                        "100",
                        "100"});
#line 61
 testRunner.Given("I have the orders for a universe from 01/01/2018 to 03/01/2018 :", ((string)(null)), table6, "Given ");
#line 65
 testRunner.When("I run the high profit rule", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 66
 testRunner.Then("I will have 0 high profit alerts", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Buy order with increase in market price (bmll) yields one alert")]
        [NUnit.Framework.CategoryAttribute("highprofit")]
        [NUnit.Framework.CategoryAttribute("highprofitpercentage")]
        [NUnit.Framework.CategoryAttribute("highprofitpercentagesensitive")]
        public virtual void BuyOrderWithIncreaseInMarketPriceBmllYieldsOneAlert()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Buy order with increase in market price (bmll) yields one alert", null, new string[] {
                        "highprofit",
                        "highprofitpercentage",
                        "highprofitpercentagesensitive"});
#line 72
this.ScenarioInitialize(scenarioInfo);
            this.ScenarioStart();
#line 10
this.FeatureBackground();
#line hidden
            TechTalk.SpecFlow.Table table7 = new TechTalk.SpecFlow.Table(new string[] {
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
                        "FilledVolume"});
            table7.AddRow(new string[] {
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
                        "100",
                        "100"});
#line 73
 testRunner.Given("I have the orders for a universe from 01/01/2018 to 03/01/2018 :", ((string)(null)), table7, "Given ");
#line hidden
            TechTalk.SpecFlow.Table table8 = new TechTalk.SpecFlow.Table(new string[] {
                        "SecurityName",
                        "Epoch",
                        "Bid",
                        "Ask",
                        "Price",
                        "Currency",
                        "Volume"});
            table8.AddRow(new string[] {
                        "Vodafone",
                        "01/01/2018 09:30:00",
                        "101",
                        "101",
                        "110",
                        "GBX",
                        "10000"});
#line 76
 testRunner.And("With the intraday market data :", ((string)(null)), table8, "And ");
#line 79
 testRunner.When("I run the high profit rule", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 80
 testRunner.Then("I will have 1 high profit alerts", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Buy order with increase in market price to exact percentage (bmll) yields one ale" +
            "rt")]
        [NUnit.Framework.CategoryAttribute("highprofit")]
        [NUnit.Framework.CategoryAttribute("highprofitpercentage")]
        [NUnit.Framework.CategoryAttribute("highprofitpercentagesensitive")]
        public virtual void BuyOrderWithIncreaseInMarketPriceToExactPercentageBmllYieldsOneAlert()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Buy order with increase in market price to exact percentage (bmll) yields one ale" +
                    "rt", null, new string[] {
                        "highprofit",
                        "highprofitpercentage",
                        "highprofitpercentagesensitive"});
#line 86
this.ScenarioInitialize(scenarioInfo);
            this.ScenarioStart();
#line 10
this.FeatureBackground();
#line hidden
            TechTalk.SpecFlow.Table table9 = new TechTalk.SpecFlow.Table(new string[] {
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
                        "FilledVolume"});
            table9.AddRow(new string[] {
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
                        "100",
                        "100"});
#line 87
 testRunner.Given("I have the orders for a universe from 01/01/2018 to 03/01/2018 :", ((string)(null)), table9, "Given ");
#line hidden
            TechTalk.SpecFlow.Table table10 = new TechTalk.SpecFlow.Table(new string[] {
                        "SecurityName",
                        "Epoch",
                        "Bid",
                        "Ask",
                        "Price",
                        "Currency",
                        "Volume"});
            table10.AddRow(new string[] {
                        "Vodafone",
                        "01/01/2018 09:30:00",
                        "101",
                        "101",
                        "110",
                        "GBX",
                        "10000"});
#line 90
 testRunner.And("With the intraday market data :", ((string)(null)), table10, "And ");
#line 93
 testRunner.When("I run the high profit rule", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 94
 testRunner.Then("I will have 1 high profit alerts", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Buy order with substantial increase in market price (bmll) yields one alert")]
        [NUnit.Framework.CategoryAttribute("highprofit")]
        [NUnit.Framework.CategoryAttribute("highprofitpercentage")]
        [NUnit.Framework.CategoryAttribute("highprofitpercentagesensitive")]
        public virtual void BuyOrderWithSubstantialIncreaseInMarketPriceBmllYieldsOneAlert()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Buy order with substantial increase in market price (bmll) yields one alert", null, new string[] {
                        "highprofit",
                        "highprofitpercentage",
                        "highprofitpercentagesensitive"});
#line 99
this.ScenarioInitialize(scenarioInfo);
            this.ScenarioStart();
#line 10
this.FeatureBackground();
#line hidden
            TechTalk.SpecFlow.Table table11 = new TechTalk.SpecFlow.Table(new string[] {
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
                        "FilledVolume"});
            table11.AddRow(new string[] {
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
                        "100",
                        "100"});
#line 100
 testRunner.Given("I have the orders for a universe from 01/01/2018 to 03/01/2018 :", ((string)(null)), table11, "Given ");
#line hidden
            TechTalk.SpecFlow.Table table12 = new TechTalk.SpecFlow.Table(new string[] {
                        "SecurityName",
                        "Epoch",
                        "Bid",
                        "Ask",
                        "Price",
                        "Currency",
                        "Volume"});
            table12.AddRow(new string[] {
                        "Vodafone",
                        "01/01/2018 09:30:00",
                        "101",
                        "101",
                        "110",
                        "GBX",
                        "10000"});
#line 103
 testRunner.And("With the intraday market data :", ((string)(null)), table12, "And ");
#line 106
 testRunner.When("I run the high profit rule", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 107
 testRunner.Then("I will have 1 high profit alerts", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Buy order with decrease in market price (bmll) yields zero alerts")]
        [NUnit.Framework.CategoryAttribute("highprofit")]
        [NUnit.Framework.CategoryAttribute("highprofitpercentage")]
        [NUnit.Framework.CategoryAttribute("highprofitpercentagesensitive")]
        public virtual void BuyOrderWithDecreaseInMarketPriceBmllYieldsZeroAlerts()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Buy order with decrease in market price (bmll) yields zero alerts", null, new string[] {
                        "highprofit",
                        "highprofitpercentage",
                        "highprofitpercentagesensitive"});
#line 112
this.ScenarioInitialize(scenarioInfo);
            this.ScenarioStart();
#line 10
this.FeatureBackground();
#line hidden
            TechTalk.SpecFlow.Table table13 = new TechTalk.SpecFlow.Table(new string[] {
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
                        "FilledVolume"});
            table13.AddRow(new string[] {
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
                        "100",
                        "100"});
#line 113
 testRunner.Given("I have the orders for a universe from 01/01/2018 to 03/01/2018 :", ((string)(null)), table13, "Given ");
#line hidden
            TechTalk.SpecFlow.Table table14 = new TechTalk.SpecFlow.Table(new string[] {
                        "SecurityName",
                        "Epoch",
                        "Bid",
                        "Ask",
                        "Price",
                        "Currency",
                        "Volume"});
            table14.AddRow(new string[] {
                        "Vodafone",
                        "01/01/2018 09:30:00",
                        "101",
                        "101",
                        "98",
                        "GBX",
                        "10000"});
#line 116
 testRunner.And("With the intraday market data :", ((string)(null)), table14, "And ");
#line 119
 testRunner.When("I run the high profit rule", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 120
 testRunner.Then("I will have 0 high profit alerts", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            this.ScenarioCleanup();
        }
    }
}
#pragma warning restore
#endregion
