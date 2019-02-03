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
    [NUnit.Framework.DescriptionAttribute("HighProfit Percentage Sensitive Parameters")]
    public partial class HighProfitPercentageSensitiveParametersFeature
    {
        
        private TechTalk.SpecFlow.ITestRunner testRunner;
        
#line 1 "HighProfit.Percentage.Sensitive.feature"
#line hidden
        
        [NUnit.Framework.OneTimeSetUpAttribute()]
        public virtual void FeatureSetup()
        {
            testRunner = TechTalk.SpecFlow.TestRunnerManager.GetTestRunner();
            TechTalk.SpecFlow.FeatureInfo featureInfo = new TechTalk.SpecFlow.FeatureInfo(new System.Globalization.CultureInfo("en-US"), "HighProfit Percentage Sensitive Parameters", "\tIn order to meet MAR compliance requirements\r\n\tI need to be able to detect when " +
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
            TechTalk.SpecFlow.Table table43 = new TechTalk.SpecFlow.Table(new string[] {
                        "WindowHours",
                        "HighProfitPercentage",
                        "HighProfitAbsolute",
                        "HighProfitCurrency",
                        "HighProfitUseCurrencyConversions"});
            table43.AddRow(new string[] {
                        "1",
                        "0.01",
                        "",
                        "",
                        ""});
#line 11
 testRunner.Given("I have the high profit rule parameter values", ((string)(null)), table43, "Given ");
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
            TechTalk.SpecFlow.Table table44 = new TechTalk.SpecFlow.Table(new string[] {
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
 testRunner.Given("I have the orders for a universe from 01/01/2018 to 03/01/2018 :", ((string)(null)), table44, "Given ");
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
#line 28
this.ScenarioInitialize(scenarioInfo);
            this.ScenarioStart();
#line 10
this.FeatureBackground();
#line hidden
            TechTalk.SpecFlow.Table table45 = new TechTalk.SpecFlow.Table(new string[] {
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
            table45.AddRow(new string[] {
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
#line 29
 testRunner.Given("I have the orders for a universe from 01/01/2018 to 03/01/2018 :", ((string)(null)), table45, "Given ");
#line 32
 testRunner.When("I run the high profit rule", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 33
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
#line 39
this.ScenarioInitialize(scenarioInfo);
            this.ScenarioStart();
#line 10
this.FeatureBackground();
#line hidden
            TechTalk.SpecFlow.Table table46 = new TechTalk.SpecFlow.Table(new string[] {
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
            table46.AddRow(new string[] {
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
            table46.AddRow(new string[] {
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
#line 40
 testRunner.Given("I have the orders for a universe from 01/01/2018 to 03/01/2018 :", ((string)(null)), table46, "Given ");
#line 44
 testRunner.When("I run the high profit rule", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 45
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
#line 50
this.ScenarioInitialize(scenarioInfo);
            this.ScenarioStart();
#line 10
this.FeatureBackground();
#line hidden
            TechTalk.SpecFlow.Table table47 = new TechTalk.SpecFlow.Table(new string[] {
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
            table47.AddRow(new string[] {
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
            table47.AddRow(new string[] {
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
                        "101",
                        "100",
                        "100"});
#line 51
 testRunner.Given("I have the orders for a universe from 01/01/2018 to 03/01/2018 :", ((string)(null)), table47, "Given ");
#line 55
 testRunner.When("I run the high profit rule", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 56
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
#line 61
this.ScenarioInitialize(scenarioInfo);
            this.ScenarioStart();
#line 10
this.FeatureBackground();
#line hidden
            TechTalk.SpecFlow.Table table48 = new TechTalk.SpecFlow.Table(new string[] {
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
            table48.AddRow(new string[] {
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
            table48.AddRow(new string[] {
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
#line 62
 testRunner.Given("I have the orders for a universe from 01/01/2018 to 03/01/2018 :", ((string)(null)), table48, "Given ");
#line 66
 testRunner.When("I run the high profit rule", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 67
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
#line 73
this.ScenarioInitialize(scenarioInfo);
            this.ScenarioStart();
#line 10
this.FeatureBackground();
#line hidden
            TechTalk.SpecFlow.Table table49 = new TechTalk.SpecFlow.Table(new string[] {
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
            table49.AddRow(new string[] {
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
#line 74
 testRunner.Given("I have the orders for a universe from 01/01/2018 to 03/01/2018 :", ((string)(null)), table49, "Given ");
#line hidden
            TechTalk.SpecFlow.Table table50 = new TechTalk.SpecFlow.Table(new string[] {
                        "SecurityName",
                        "Epoch",
                        "Bid",
                        "Ask",
                        "Price",
                        "Currency",
                        "Volume"});
            table50.AddRow(new string[] {
                        "Vodafone",
                        "01/01/2018 09:30:00",
                        "101",
                        "101",
                        "101",
                        "GBX",
                        "10000"});
#line 77
 testRunner.And("With the intraday market data :", ((string)(null)), table50, "And ");
#line 80
 testRunner.When("I run the high profit rule", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 81
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
#line 87
this.ScenarioInitialize(scenarioInfo);
            this.ScenarioStart();
#line 10
this.FeatureBackground();
#line hidden
            TechTalk.SpecFlow.Table table51 = new TechTalk.SpecFlow.Table(new string[] {
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
            table51.AddRow(new string[] {
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
#line 88
 testRunner.Given("I have the orders for a universe from 01/01/2018 to 03/01/2018 :", ((string)(null)), table51, "Given ");
#line hidden
            TechTalk.SpecFlow.Table table52 = new TechTalk.SpecFlow.Table(new string[] {
                        "SecurityName",
                        "Epoch",
                        "Bid",
                        "Ask",
                        "Price",
                        "Currency",
                        "Volume"});
            table52.AddRow(new string[] {
                        "Vodafone",
                        "01/01/2018 09:30:00",
                        "101",
                        "101",
                        "101",
                        "GBX",
                        "10000"});
#line 91
 testRunner.And("With the intraday market data :", ((string)(null)), table52, "And ");
#line 94
 testRunner.When("I run the high profit rule", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 95
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
#line 100
this.ScenarioInitialize(scenarioInfo);
            this.ScenarioStart();
#line 10
this.FeatureBackground();
#line hidden
            TechTalk.SpecFlow.Table table53 = new TechTalk.SpecFlow.Table(new string[] {
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
            table53.AddRow(new string[] {
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
#line 101
 testRunner.Given("I have the orders for a universe from 01/01/2018 to 03/01/2018 :", ((string)(null)), table53, "Given ");
#line hidden
            TechTalk.SpecFlow.Table table54 = new TechTalk.SpecFlow.Table(new string[] {
                        "SecurityName",
                        "Epoch",
                        "Bid",
                        "Ask",
                        "Price",
                        "Currency",
                        "Volume"});
            table54.AddRow(new string[] {
                        "Vodafone",
                        "01/01/2018 09:30:00",
                        "101",
                        "101",
                        "110",
                        "GBX",
                        "10000"});
#line 104
 testRunner.And("With the intraday market data :", ((string)(null)), table54, "And ");
#line 107
 testRunner.When("I run the high profit rule", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 108
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
#line 113
this.ScenarioInitialize(scenarioInfo);
            this.ScenarioStart();
#line 10
this.FeatureBackground();
#line hidden
            TechTalk.SpecFlow.Table table55 = new TechTalk.SpecFlow.Table(new string[] {
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
            table55.AddRow(new string[] {
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
#line 114
 testRunner.Given("I have the orders for a universe from 01/01/2018 to 03/01/2018 :", ((string)(null)), table55, "Given ");
#line hidden
            TechTalk.SpecFlow.Table table56 = new TechTalk.SpecFlow.Table(new string[] {
                        "SecurityName",
                        "Epoch",
                        "Bid",
                        "Ask",
                        "Price",
                        "Currency",
                        "Volume"});
            table56.AddRow(new string[] {
                        "Vodafone",
                        "01/01/2018 09:30:00",
                        "101",
                        "101",
                        "98",
                        "GBX",
                        "10000"});
#line 117
 testRunner.And("With the intraday market data :", ((string)(null)), table56, "And ");
#line 120
 testRunner.When("I run the high profit rule", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 121
 testRunner.Then("I will have 0 high profit alerts", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            this.ScenarioCleanup();
        }
    }
}
#pragma warning restore
#endregion
