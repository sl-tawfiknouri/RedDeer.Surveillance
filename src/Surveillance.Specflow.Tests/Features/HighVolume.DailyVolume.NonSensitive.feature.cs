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
    [NUnit.Framework.DescriptionAttribute("HighVolume Daily Volume Non Sensitive Parameters")]
    public partial class HighVolumeDailyVolumeNonSensitiveParametersFeature
    {
        
        private TechTalk.SpecFlow.ITestRunner testRunner;
        
#line 1 "HighVolume.DailyVolume.NonSensitive.feature"
#line hidden
        
        [NUnit.Framework.OneTimeSetUpAttribute()]
        public virtual void FeatureSetup()
        {
            testRunner = TechTalk.SpecFlow.TestRunnerManager.GetTestRunner();
            TechTalk.SpecFlow.FeatureInfo featureInfo = new TechTalk.SpecFlow.FeatureInfo(new System.Globalization.CultureInfo("en-US"), "HighVolume Daily Volume Non Sensitive Parameters", @"	In order to meet MAR compliance requirements
	I need to be able to detect when traders are executing trades
	At a volume where they are able to exert market manipulating pressure
	on the prices the market is trading at
	By measuring their security trades relative to the daily volume traded of the company", ProgrammingLanguage.CSharp, ((string[])(null)));
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
#line 8
#line hidden
            TechTalk.SpecFlow.Table table85 = new TechTalk.SpecFlow.Table(new string[] {
                        "WindowHours",
                        "HighVolumePercentageDaily",
                        "HighVolumePercentageWindow",
                        "HighVolumePercentageMarketCap"});
            table85.AddRow(new string[] {
                        "1",
                        "0.1",
                        "",
                        ""});
#line 9
 testRunner.Given("I have the high volume rule parameter values", ((string)(null)), table85, "Given ");
#line hidden
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Empty Universe yields no alerts")]
        [NUnit.Framework.CategoryAttribute("highvolume")]
        [NUnit.Framework.CategoryAttribute("highvolumeDaily")]
        [NUnit.Framework.CategoryAttribute("highvolumedailynonsensitive")]
        public virtual void EmptyUniverseYieldsNoAlerts()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Empty Universe yields no alerts", null, new string[] {
                        "highvolume",
                        "highvolumeDaily",
                        "highvolumedailynonsensitive"});
#line 16
this.ScenarioInitialize(scenarioInfo);
            this.ScenarioStart();
#line 8
this.FeatureBackground();
#line hidden
            TechTalk.SpecFlow.Table table86 = new TechTalk.SpecFlow.Table(new string[] {
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
#line 17
 testRunner.Given("I have the orders for a universe from 01/01/2018 to 03/01/2018 :", ((string)(null)), table86, "Given ");
#line 19
 testRunner.When("I run the high volume rule", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 20
 testRunner.Then("I will have 0 high volume alerts", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("One order at daily volume yields one alert")]
        [NUnit.Framework.CategoryAttribute("highvolume")]
        [NUnit.Framework.CategoryAttribute("highvolumeDaily")]
        [NUnit.Framework.CategoryAttribute("highvolumedailynonsensitive")]
        public virtual void OneOrderAtDailyVolumeYieldsOneAlert()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("One order at daily volume yields one alert", null, new string[] {
                        "highvolume",
                        "highvolumeDaily",
                        "highvolumedailynonsensitive"});
#line 25
this.ScenarioInitialize(scenarioInfo);
            this.ScenarioStart();
#line 8
this.FeatureBackground();
#line hidden
            TechTalk.SpecFlow.Table table87 = new TechTalk.SpecFlow.Table(new string[] {
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
            table87.AddRow(new string[] {
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
#line 26
 testRunner.Given("I have the orders for a universe from 01/01/2018 to 03/01/2018 :", ((string)(null)), table87, "Given ");
#line hidden
            TechTalk.SpecFlow.Table table88 = new TechTalk.SpecFlow.Table(new string[] {
                        "SecurityName",
                        "Epoch",
                        "OpenPrice",
                        "ClosePrice",
                        "HighIntradayPrice",
                        "LowIntradayPrice",
                        "ListedSecurities",
                        "MarketCap",
                        "DailyVolume",
                        "Currency"});
            table88.AddRow(new string[] {
                        "Vodafone",
                        "01/01/2018",
                        "10",
                        "11",
                        "11.5",
                        "10",
                        "10",
                        "1000000",
                        "1000",
                        "GBX"});
#line 29
 testRunner.And("With the interday market data :", ((string)(null)), table88, "And ");
#line 32
 testRunner.When("I run the high volume rule", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 33
 testRunner.Then("I will have 1 high volume alerts", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("One order in different exchange and currency at daily volume yields one alert")]
        [NUnit.Framework.CategoryAttribute("highvolume")]
        [NUnit.Framework.CategoryAttribute("highvolumeDaily")]
        [NUnit.Framework.CategoryAttribute("highvolumedailynonsensitive")]
        public virtual void OneOrderInDifferentExchangeAndCurrencyAtDailyVolumeYieldsOneAlert()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("One order in different exchange and currency at daily volume yields one alert", null, new string[] {
                        "highvolume",
                        "highvolumeDaily",
                        "highvolumedailynonsensitive"});
#line 38
this.ScenarioInitialize(scenarioInfo);
            this.ScenarioStart();
#line 8
this.FeatureBackground();
#line hidden
            TechTalk.SpecFlow.Table table89 = new TechTalk.SpecFlow.Table(new string[] {
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
            table89.AddRow(new string[] {
                        "Nvidia",
                        "0",
                        "01/01/2018 09:30:00",
                        "",
                        "",
                        "",
                        "",
                        "01/01/2018 09:30:00",
                        "MARKET",
                        "BUY",
                        "USD",
                        "",
                        "10",
                        "100",
                        "100"});
#line 39
 testRunner.Given("I have the orders for a universe from 01/01/2018 to 03/01/2018 :", ((string)(null)), table89, "Given ");
#line hidden
            TechTalk.SpecFlow.Table table90 = new TechTalk.SpecFlow.Table(new string[] {
                        "SecurityName",
                        "Epoch",
                        "OpenPrice",
                        "ClosePrice",
                        "HighIntradayPrice",
                        "LowIntradayPrice",
                        "ListedSecurities",
                        "MarketCap",
                        "DailyVolume",
                        "Currency"});
            table90.AddRow(new string[] {
                        "Nvidia",
                        "01/01/2018",
                        "10",
                        "11",
                        "11.5",
                        "10",
                        "10",
                        "1000000",
                        "1000",
                        "USD"});
#line 42
 testRunner.And("With the interday market data :", ((string)(null)), table90, "And ");
#line 45
 testRunner.When("I run the high volume rule", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 46
 testRunner.Then("I will have 1 high volume alerts", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("One order below daily volume yields zero alerts")]
        [NUnit.Framework.CategoryAttribute("highvolume")]
        [NUnit.Framework.CategoryAttribute("highvolumeDaily")]
        [NUnit.Framework.CategoryAttribute("highvolumedailynonsensitive")]
        public virtual void OneOrderBelowDailyVolumeYieldsZeroAlerts()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("One order below daily volume yields zero alerts", null, new string[] {
                        "highvolume",
                        "highvolumeDaily",
                        "highvolumedailynonsensitive"});
#line 51
this.ScenarioInitialize(scenarioInfo);
            this.ScenarioStart();
#line 8
this.FeatureBackground();
#line hidden
            TechTalk.SpecFlow.Table table91 = new TechTalk.SpecFlow.Table(new string[] {
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
            table91.AddRow(new string[] {
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
                        "99",
                        "99"});
#line 52
 testRunner.Given("I have the orders for a universe from 01/01/2018 to 03/01/2018 :", ((string)(null)), table91, "Given ");
#line hidden
            TechTalk.SpecFlow.Table table92 = new TechTalk.SpecFlow.Table(new string[] {
                        "SecurityName",
                        "Epoch",
                        "OpenPrice",
                        "ClosePrice",
                        "HighIntradayPrice",
                        "LowIntradayPrice",
                        "ListedSecurities",
                        "MarketCap",
                        "DailyVolume",
                        "Currency"});
            table92.AddRow(new string[] {
                        "Vodafone",
                        "01/01/2018",
                        "10",
                        "11",
                        "11.5",
                        "10",
                        "10",
                        "1000000",
                        "1000",
                        "GBX"});
#line 55
 testRunner.And("With the interday market data :", ((string)(null)), table92, "And ");
#line 58
 testRunner.When("I run the high volume rule", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 59
 testRunner.Then("I will have 0 high volume alerts", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("One order above daily volume yields one alerts")]
        [NUnit.Framework.CategoryAttribute("highvolume")]
        [NUnit.Framework.CategoryAttribute("highvolumeDaily")]
        [NUnit.Framework.CategoryAttribute("highvolumedailynonsensitive")]
        public virtual void OneOrderAboveDailyVolumeYieldsOneAlerts()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("One order above daily volume yields one alerts", null, new string[] {
                        "highvolume",
                        "highvolumeDaily",
                        "highvolumedailynonsensitive"});
#line 64
this.ScenarioInitialize(scenarioInfo);
            this.ScenarioStart();
#line 8
this.FeatureBackground();
#line hidden
            TechTalk.SpecFlow.Table table93 = new TechTalk.SpecFlow.Table(new string[] {
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
            table93.AddRow(new string[] {
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
                        "101",
                        "101"});
#line 65
 testRunner.Given("I have the orders for a universe from 01/01/2018 to 03/01/2018 :", ((string)(null)), table93, "Given ");
#line hidden
            TechTalk.SpecFlow.Table table94 = new TechTalk.SpecFlow.Table(new string[] {
                        "SecurityName",
                        "Epoch",
                        "OpenPrice",
                        "ClosePrice",
                        "HighIntradayPrice",
                        "LowIntradayPrice",
                        "ListedSecurities",
                        "MarketCap",
                        "DailyVolume",
                        "Currency"});
            table94.AddRow(new string[] {
                        "Vodafone",
                        "01/01/2018",
                        "10",
                        "11",
                        "11.5",
                        "10",
                        "10",
                        "1000000",
                        "1000",
                        "GBX"});
#line 68
 testRunner.And("With the interday market data :", ((string)(null)), table94, "And ");
#line 71
 testRunner.When("I run the high volume rule", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 72
 testRunner.Then("I will have 1 high volume alerts", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Two order at daily volume at exact window yields one alert")]
        [NUnit.Framework.CategoryAttribute("highvolume")]
        [NUnit.Framework.CategoryAttribute("highvolumeDaily")]
        [NUnit.Framework.CategoryAttribute("highvolumedailynonsensitive")]
        public virtual void TwoOrderAtDailyVolumeAtExactWindowYieldsOneAlert()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Two order at daily volume at exact window yields one alert", null, new string[] {
                        "highvolume",
                        "highvolumeDaily",
                        "highvolumedailynonsensitive"});
#line 78
this.ScenarioInitialize(scenarioInfo);
            this.ScenarioStart();
#line 8
this.FeatureBackground();
#line hidden
            TechTalk.SpecFlow.Table table95 = new TechTalk.SpecFlow.Table(new string[] {
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
            table95.AddRow(new string[] {
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
                        "50",
                        "50"});
            table95.AddRow(new string[] {
                        "Vodafone",
                        "0",
                        "01/01/2018 10:30:00",
                        "",
                        "",
                        "",
                        "",
                        "01/01/2018 10:30:00",
                        "MARKET",
                        "BUY",
                        "GBX",
                        "",
                        "10",
                        "50",
                        "50"});
#line 79
 testRunner.Given("I have the orders for a universe from 01/01/2018 to 03/01/2018 :", ((string)(null)), table95, "Given ");
#line hidden
            TechTalk.SpecFlow.Table table96 = new TechTalk.SpecFlow.Table(new string[] {
                        "SecurityName",
                        "Epoch",
                        "OpenPrice",
                        "ClosePrice",
                        "HighIntradayPrice",
                        "LowIntradayPrice",
                        "ListedSecurities",
                        "MarketCap",
                        "DailyVolume",
                        "Currency"});
            table96.AddRow(new string[] {
                        "Vodafone",
                        "01/01/2018",
                        "10",
                        "11",
                        "11.5",
                        "10",
                        "10",
                        "1000000",
                        "1000",
                        "GBX"});
#line 83
 testRunner.And("With the interday market data :", ((string)(null)), table96, "And ");
#line 86
 testRunner.When("I run the high volume rule", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 87
 testRunner.Then("I will have 1 high volume alerts", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Two order at daily volume but outside window yields zero alert")]
        [NUnit.Framework.CategoryAttribute("highvolume")]
        [NUnit.Framework.CategoryAttribute("highvolumeDaily")]
        [NUnit.Framework.CategoryAttribute("highvolumedailynonsensitive")]
        public virtual void TwoOrderAtDailyVolumeButOutsideWindowYieldsZeroAlert()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Two order at daily volume but outside window yields zero alert", null, new string[] {
                        "highvolume",
                        "highvolumeDaily",
                        "highvolumedailynonsensitive"});
#line 93
this.ScenarioInitialize(scenarioInfo);
            this.ScenarioStart();
#line 8
this.FeatureBackground();
#line hidden
            TechTalk.SpecFlow.Table table97 = new TechTalk.SpecFlow.Table(new string[] {
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
            table97.AddRow(new string[] {
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
                        "50",
                        "50"});
            table97.AddRow(new string[] {
                        "Vodafone",
                        "0",
                        "01/01/2018 10:31:00",
                        "",
                        "",
                        "",
                        "",
                        "01/01/2018 10:31:00",
                        "MARKET",
                        "BUY",
                        "GBX",
                        "",
                        "10",
                        "50",
                        "50"});
#line 94
 testRunner.Given("I have the orders for a universe from 01/01/2018 to 03/01/2018 :", ((string)(null)), table97, "Given ");
#line hidden
            TechTalk.SpecFlow.Table table98 = new TechTalk.SpecFlow.Table(new string[] {
                        "SecurityName",
                        "Epoch",
                        "OpenPrice",
                        "ClosePrice",
                        "HighIntradayPrice",
                        "LowIntradayPrice",
                        "ListedSecurities",
                        "MarketCap",
                        "DailyVolume",
                        "Currency"});
            table98.AddRow(new string[] {
                        "Vodafone",
                        "01/01/2018",
                        "10",
                        "11",
                        "11.5",
                        "10",
                        "10",
                        "1000000",
                        "1000",
                        "GBX"});
#line 98
 testRunner.And("With the interday market data :", ((string)(null)), table98, "And ");
#line 101
 testRunner.When("I run the high volume rule", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 102
 testRunner.Then("I will have 0 high volume alerts", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Two order at daily volume and inside yields one alert")]
        [NUnit.Framework.CategoryAttribute("highvolume")]
        [NUnit.Framework.CategoryAttribute("highvolumeDaily")]
        [NUnit.Framework.CategoryAttribute("highvolumedailynonsensitive")]
        public virtual void TwoOrderAtDailyVolumeAndInsideYieldsOneAlert()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Two order at daily volume and inside yields one alert", null, new string[] {
                        "highvolume",
                        "highvolumeDaily",
                        "highvolumedailynonsensitive"});
#line 108
this.ScenarioInitialize(scenarioInfo);
            this.ScenarioStart();
#line 8
this.FeatureBackground();
#line hidden
            TechTalk.SpecFlow.Table table99 = new TechTalk.SpecFlow.Table(new string[] {
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
            table99.AddRow(new string[] {
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
                        "50",
                        "50"});
            table99.AddRow(new string[] {
                        "Vodafone",
                        "0",
                        "01/01/2018 10:25:00",
                        "",
                        "",
                        "",
                        "",
                        "01/01/2018 10:25:00",
                        "MARKET",
                        "BUY",
                        "GBX",
                        "",
                        "10",
                        "50",
                        "50"});
#line 109
 testRunner.Given("I have the orders for a universe from 01/01/2018 to 03/01/2018 :", ((string)(null)), table99, "Given ");
#line hidden
            TechTalk.SpecFlow.Table table100 = new TechTalk.SpecFlow.Table(new string[] {
                        "SecurityName",
                        "Epoch",
                        "OpenPrice",
                        "ClosePrice",
                        "HighIntradayPrice",
                        "LowIntradayPrice",
                        "ListedSecurities",
                        "MarketCap",
                        "DailyVolume",
                        "Currency"});
            table100.AddRow(new string[] {
                        "Vodafone",
                        "01/01/2018",
                        "10",
                        "11",
                        "11.5",
                        "10",
                        "10",
                        "1000000",
                        "1000",
                        "GBX"});
#line 113
 testRunner.And("With the interday market data :", ((string)(null)), table100, "And ");
#line 116
 testRunner.When("I run the high volume rule", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 117
 testRunner.Then("I will have 1 high volume alerts", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            this.ScenarioCleanup();
        }
    }
}
#pragma warning restore
#endregion
