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
    [NUnit.Framework.DescriptionAttribute("HighProfit Absolute Non Sensitive Parameters")]
    public partial class HighProfitAbsoluteNonSensitiveParametersFeature
    {
        
        private TechTalk.SpecFlow.ITestRunner testRunner;
        
#line 1 "HighProfit.Absolute.NonSensitive.feature"
#line hidden
        
        [NUnit.Framework.OneTimeSetUpAttribute()]
        public virtual void FeatureSetup()
        {
            testRunner = TechTalk.SpecFlow.TestRunnerManager.GetTestRunner();
            TechTalk.SpecFlow.FeatureInfo featureInfo = new TechTalk.SpecFlow.FeatureInfo(new System.Globalization.CultureInfo("en-US"), "HighProfit Absolute Non Sensitive Parameters", "\tIn order to meet MAR compliance requirements\r\n\tI need to be able to detect when " +
                    "traders are executing trades\r\n\tWhich generate unusual levels of profits\r\n\tBy mea" +
                    "suring their security trade profits as an absolute return", ProgrammingLanguage.CSharp, ((string[])(null)));
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
                        "",
                        "100000",
                        "GBX",
                        "false"});
#line 11
 testRunner.Given("I have the high profit rule parameter values", ((string)(null)), table1, "Given ");
#line hidden
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Empty Universe yields no alerts")]
        [NUnit.Framework.CategoryAttribute("highprofit")]
        [NUnit.Framework.CategoryAttribute("highprofitabsolute")]
        [NUnit.Framework.CategoryAttribute("highprofitabsolutenonsensitive")]
        public virtual void EmptyUniverseYieldsNoAlerts()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Empty Universe yields no alerts", null, new string[] {
                        "highprofit",
                        "highprofitabsolute",
                        "highprofitabsolutenonsensitive"});
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
        [NUnit.Framework.CategoryAttribute("highprofitabsolute")]
        [NUnit.Framework.CategoryAttribute("highprofitabsolutenonsensitive")]
        public virtual void SingleOrderYieldsNoAlerts()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Single order yields no alerts", null, new string[] {
                        "highprofit",
                        "highprofitabsolute",
                        "highprofitabsolutenonsensitive"});
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
        [NUnit.Framework.DescriptionAttribute("Buy Sell orders within window yields two alerts")]
        [NUnit.Framework.CategoryAttribute("highprofit")]
        [NUnit.Framework.CategoryAttribute("highprofitabsolute")]
        [NUnit.Framework.CategoryAttribute("highprofitabsolutenonsensitive")]
        public virtual void BuySellOrdersWithinWindowYieldsTwoAlerts()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Buy Sell orders within window yields two alerts", null, new string[] {
                        "highprofit",
                        "highprofitabsolute",
                        "highprofitabsolutenonsensitive"});
#line 37
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
                        "100",
                        "100000",
                        "100000"});
            table4.AddRow(new string[] {
                        "Vodafone",
                        "1",
                        "01/01/2018 09:45:00",
                        "",
                        "",
                        "",
                        "",
                        "01/01/2018 09:45:00",
                        "MARKET",
                        "SELL",
                        "GBX",
                        "",
                        "110",
                        "100000",
                        "100000"});
#line 38
 testRunner.Given("I have the orders for a universe from 01/01/2018 to 03/01/2018 :", ((string)(null)), table4, "Given ");
#line 42
 testRunner.When("I run the high profit rule", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 43
 testRunner.Then("I will have 2 high profit alerts", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Buy Sell orders at exact window yields two alerts")]
        [NUnit.Framework.CategoryAttribute("highprofit")]
        [NUnit.Framework.CategoryAttribute("highprofitabsolute")]
        [NUnit.Framework.CategoryAttribute("highprofitabsolutenonsensitive")]
        public virtual void BuySellOrdersAtExactWindowYieldsTwoAlerts()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Buy Sell orders at exact window yields two alerts", null, new string[] {
                        "highprofit",
                        "highprofitabsolute",
                        "highprofitabsolutenonsensitive"});
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
                        "100000",
                        "100000"});
            table5.AddRow(new string[] {
                        "Vodafone",
                        "1",
                        "01/01/2018 10:30:00",
                        "",
                        "",
                        "",
                        "",
                        "01/01/2018 10:30:00",
                        "MARKET",
                        "SELL",
                        "GBX",
                        "",
                        "110",
                        "100000",
                        "100000"});
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
        [NUnit.Framework.DescriptionAttribute("Buy Sell orders but outside of window yields zero alerts")]
        [NUnit.Framework.CategoryAttribute("highprofit")]
        [NUnit.Framework.CategoryAttribute("highprofitabsolute")]
        [NUnit.Framework.CategoryAttribute("highprofitabsolutenonsensitive")]
        public virtual void BuySellOrdersButOutsideOfWindowYieldsZeroAlerts()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Buy Sell orders but outside of window yields zero alerts", null, new string[] {
                        "highprofit",
                        "highprofitabsolute",
                        "highprofitabsolutenonsensitive"});
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
                        "100",
                        "100000",
                        "100000"});
            table6.AddRow(new string[] {
                        "Vodafone",
                        "1",
                        "01/01/2018 10:35:00",
                        "",
                        "",
                        "",
                        "",
                        "01/01/2018 10:35:00",
                        "MARKET",
                        "SELL",
                        "GBX",
                        "",
                        "110",
                        "100000",
                        "100000"});
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
        [NUnit.Framework.DescriptionAttribute("Buy Sell orders yields two alerts")]
        [NUnit.Framework.CategoryAttribute("highprofit")]
        [NUnit.Framework.CategoryAttribute("highprofitabsolute")]
        [NUnit.Framework.CategoryAttribute("highprofitabsolutenonsensitive")]
        public virtual void BuySellOrdersYieldsTwoAlerts()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Buy Sell orders yields two alerts", null, new string[] {
                        "highprofit",
                        "highprofitabsolute",
                        "highprofitabsolutenonsensitive"});
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
                        "100000",
                        "100000"});
            table7.AddRow(new string[] {
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
                        "100000",
                        "100000"});
#line 73
 testRunner.Given("I have the orders for a universe from 01/01/2018 to 03/01/2018 :", ((string)(null)), table7, "Given ");
#line 77
 testRunner.When("I run the high profit rule", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 78
 testRunner.Then("I will have 2 high profit alerts", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Buy Sell orders at exact threshold yields two alerts")]
        [NUnit.Framework.CategoryAttribute("highprofit")]
        [NUnit.Framework.CategoryAttribute("highprofitabsolute")]
        [NUnit.Framework.CategoryAttribute("highprofitabsolutenonsensitive")]
        public virtual void BuySellOrdersAtExactThresholdYieldsTwoAlerts()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Buy Sell orders at exact threshold yields two alerts", null, new string[] {
                        "highprofit",
                        "highprofitabsolute",
                        "highprofitabsolutenonsensitive"});
#line 83
this.ScenarioInitialize(scenarioInfo);
            this.ScenarioStart();
#line 10
this.FeatureBackground();
#line hidden
            TechTalk.SpecFlow.Table table8 = new TechTalk.SpecFlow.Table(new string[] {
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
            table8.AddRow(new string[] {
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
                        "100000",
                        "100000"});
            table8.AddRow(new string[] {
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
                        "100000",
                        "100000"});
#line 84
 testRunner.Given("I have the orders for a universe from 01/01/2018 to 03/01/2018 :", ((string)(null)), table8, "Given ");
#line 88
 testRunner.When("I run the high profit rule", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 89
 testRunner.Then("I will have 2 high profit alerts", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Buy Sell orders at just below threshold yields zero alerts")]
        [NUnit.Framework.CategoryAttribute("highprofit")]
        [NUnit.Framework.CategoryAttribute("highprofitabsolute")]
        [NUnit.Framework.CategoryAttribute("highprofitabsolutenonsensitive")]
        public virtual void BuySellOrdersAtJustBelowThresholdYieldsZeroAlerts()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Buy Sell orders at just below threshold yields zero alerts", null, new string[] {
                        "highprofit",
                        "highprofitabsolute",
                        "highprofitabsolutenonsensitive"});
#line 94
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
                        "1000",
                        "99999",
                        "99999"});
            table9.AddRow(new string[] {
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
                        "99999",
                        "99999"});
#line 95
 testRunner.Given("I have the orders for a universe from 01/01/2018 to 03/01/2018 :", ((string)(null)), table9, "Given ");
#line 99
 testRunner.When("I run the high profit rule", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 100
 testRunner.Then("I will have 0 high profit alerts", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Buy order with increase in market price (bmll) yields one alert")]
        [NUnit.Framework.CategoryAttribute("highprofit")]
        [NUnit.Framework.CategoryAttribute("highprofitabsolute")]
        [NUnit.Framework.CategoryAttribute("highprofitabsolutenonsensitive")]
        public virtual void BuyOrderWithIncreaseInMarketPriceBmllYieldsOneAlert()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Buy order with increase in market price (bmll) yields one alert", null, new string[] {
                        "highprofit",
                        "highprofitabsolute",
                        "highprofitabsolutenonsensitive"});
#line 105
this.ScenarioInitialize(scenarioInfo);
            this.ScenarioStart();
#line 10
this.FeatureBackground();
#line hidden
            TechTalk.SpecFlow.Table table10 = new TechTalk.SpecFlow.Table(new string[] {
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
            table10.AddRow(new string[] {
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
                        "100000000",
                        "100000000"});
#line 106
 testRunner.Given("I have the orders for a universe from 01/01/2018 to 03/01/2018 :", ((string)(null)), table10, "Given ");
#line hidden
            TechTalk.SpecFlow.Table table11 = new TechTalk.SpecFlow.Table(new string[] {
                        "SecurityName",
                        "Epoch",
                        "Bid",
                        "Ask",
                        "Price",
                        "Currency",
                        "Volume"});
            table11.AddRow(new string[] {
                        "Vodafone",
                        "01/01/2018 09:30:00",
                        "101",
                        "101",
                        "110",
                        "GBX",
                        "10000"});
#line 109
 testRunner.And("With the intraday market data :", ((string)(null)), table11, "And ");
#line 112
 testRunner.When("I run the high profit rule", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 113
 testRunner.Then("I will have 1 high profit alerts", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Buy order with increase in market price to exact threshold (bmll) yields one aler" +
            "t")]
        [NUnit.Framework.CategoryAttribute("highprofit")]
        [NUnit.Framework.CategoryAttribute("highprofitabsolute")]
        [NUnit.Framework.CategoryAttribute("highprofitabsolutenonsensitive")]
        public virtual void BuyOrderWithIncreaseInMarketPriceToExactThresholdBmllYieldsOneAlert()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Buy order with increase in market price to exact threshold (bmll) yields one aler" +
                    "t", null, new string[] {
                        "highprofit",
                        "highprofitabsolute",
                        "highprofitabsolutenonsensitive"});
#line 119
this.ScenarioInitialize(scenarioInfo);
            this.ScenarioStart();
#line 10
this.FeatureBackground();
#line hidden
            TechTalk.SpecFlow.Table table12 = new TechTalk.SpecFlow.Table(new string[] {
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
            table12.AddRow(new string[] {
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
                        "100000",
                        "100000"});
#line 120
 testRunner.Given("I have the orders for a universe from 01/01/2018 to 03/01/2018 :", ((string)(null)), table12, "Given ");
#line hidden
            TechTalk.SpecFlow.Table table13 = new TechTalk.SpecFlow.Table(new string[] {
                        "SecurityName",
                        "Epoch",
                        "Bid",
                        "Ask",
                        "Price",
                        "Currency",
                        "Volume"});
            table13.AddRow(new string[] {
                        "Vodafone",
                        "01/01/2018 09:30:00",
                        "101",
                        "101",
                        "101",
                        "GBX",
                        "10000"});
#line 123
 testRunner.And("With the intraday market data :", ((string)(null)), table13, "And ");
#line 126
 testRunner.When("I run the high profit rule", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 127
 testRunner.Then("I will have 1 high profit alerts", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Buy order with substantial increase in market price (bmll) yields one alert")]
        [NUnit.Framework.CategoryAttribute("highprofit")]
        [NUnit.Framework.CategoryAttribute("highprofitabsolute")]
        [NUnit.Framework.CategoryAttribute("highprofitabsolutenonsensitive")]
        public virtual void BuyOrderWithSubstantialIncreaseInMarketPriceBmllYieldsOneAlert()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Buy order with substantial increase in market price (bmll) yields one alert", null, new string[] {
                        "highprofit",
                        "highprofitabsolute",
                        "highprofitabsolutenonsensitive"});
#line 132
this.ScenarioInitialize(scenarioInfo);
            this.ScenarioStart();
#line 10
this.FeatureBackground();
#line hidden
            TechTalk.SpecFlow.Table table14 = new TechTalk.SpecFlow.Table(new string[] {
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
            table14.AddRow(new string[] {
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
                        "100000",
                        "100000"});
#line 133
 testRunner.Given("I have the orders for a universe from 01/01/2018 to 03/01/2018 :", ((string)(null)), table14, "Given ");
#line hidden
            TechTalk.SpecFlow.Table table15 = new TechTalk.SpecFlow.Table(new string[] {
                        "SecurityName",
                        "Epoch",
                        "Bid",
                        "Ask",
                        "Price",
                        "Currency",
                        "Volume"});
            table15.AddRow(new string[] {
                        "Vodafone",
                        "01/01/2018 09:30:00",
                        "101",
                        "101",
                        "110",
                        "GBX",
                        "10000"});
#line 136
 testRunner.And("With the intraday market data :", ((string)(null)), table15, "And ");
#line 139
 testRunner.When("I run the high profit rule", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 140
 testRunner.Then("I will have 1 high profit alerts", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Buy order with decrease in market price (bmll) yields zero alerts")]
        [NUnit.Framework.CategoryAttribute("highprofit")]
        [NUnit.Framework.CategoryAttribute("highprofitabsolute")]
        [NUnit.Framework.CategoryAttribute("highprofitabsolutenonsensitive")]
        public virtual void BuyOrderWithDecreaseInMarketPriceBmllYieldsZeroAlerts()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Buy order with decrease in market price (bmll) yields zero alerts", null, new string[] {
                        "highprofit",
                        "highprofitabsolute",
                        "highprofitabsolutenonsensitive"});
#line 145
this.ScenarioInitialize(scenarioInfo);
            this.ScenarioStart();
#line 10
this.FeatureBackground();
#line hidden
            TechTalk.SpecFlow.Table table16 = new TechTalk.SpecFlow.Table(new string[] {
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
            table16.AddRow(new string[] {
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
#line 146
 testRunner.Given("I have the orders for a universe from 01/01/2018 to 03/01/2018 :", ((string)(null)), table16, "Given ");
#line hidden
            TechTalk.SpecFlow.Table table17 = new TechTalk.SpecFlow.Table(new string[] {
                        "SecurityName",
                        "Epoch",
                        "Bid",
                        "Ask",
                        "Price",
                        "Currency",
                        "Volume"});
            table17.AddRow(new string[] {
                        "Vodafone",
                        "01/01/2018 09:30:00",
                        "101",
                        "101",
                        "98",
                        "GBX",
                        "10000"});
#line 149
 testRunner.And("With the intraday market data :", ((string)(null)), table17, "And ");
#line 152
 testRunner.When("I run the high profit rule", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 153
 testRunner.Then("I will have 0 high profit alerts", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            this.ScenarioCleanup();
        }
    }
}
#pragma warning restore
#endregion
