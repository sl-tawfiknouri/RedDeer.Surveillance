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
    [NUnit.Framework.DescriptionAttribute("HighVolume Market Capitalisation Sensitive Parameters")]
    public partial class HighVolumeMarketCapitalisationSensitiveParametersFeature
    {
        
        private TechTalk.SpecFlow.ITestRunner testRunner;
        
#line 1 "HighVolume.MarketCapitalisation.Sensitive.feature"
#line hidden
        
        [NUnit.Framework.OneTimeSetUpAttribute()]
        public virtual void FeatureSetup()
        {
            testRunner = TechTalk.SpecFlow.TestRunnerManager.GetTestRunner();
            TechTalk.SpecFlow.FeatureInfo featureInfo = new TechTalk.SpecFlow.FeatureInfo(new System.Globalization.CultureInfo("en-US"), "HighVolume Market Capitalisation Sensitive Parameters", @"	In order to meet MAR compliance requirements
	I need to be able to detect when traders are executing trades
	At a volume where they are able to exert market manipulating pressure
	on the prices the market is trading at
	By measuring their security trades relative to the market cap of the underlying company", ProgrammingLanguage.CSharp, ((string[])(null)));
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
            TechTalk.SpecFlow.Table table127 = new TechTalk.SpecFlow.Table(new string[] {
                        "WindowHours",
                        "HighVolumePercentageDaily",
                        "HighVolumePercentageWindow",
                        "HighVolumePercentageMarketCap"});
            table127.AddRow(new string[] {
                        "1",
                        "",
                        "",
                        "0.01"});
#line 9
 testRunner.Given("I have the high volume rule parameter values", ((string)(null)), table127, "Given ");
#line hidden
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Empty Universe yields no alerts")]
        [NUnit.Framework.CategoryAttribute("highvolume")]
        [NUnit.Framework.CategoryAttribute("highvolumemarketcap")]
        [NUnit.Framework.CategoryAttribute("highvolumemarketcapsensitive")]
        public virtual void EmptyUniverseYieldsNoAlerts()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Empty Universe yields no alerts", null, new string[] {
                        "highvolume",
                        "highvolumemarketcap",
                        "highvolumemarketcapsensitive"});
#line 16
this.ScenarioInitialize(scenarioInfo);
            this.ScenarioStart();
#line 8
this.FeatureBackground();
#line hidden
            TechTalk.SpecFlow.Table table128 = new TechTalk.SpecFlow.Table(new string[] {
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
 testRunner.Given("I have the orders for a universe from 01/01/2018 to 03/01/2018 :", ((string)(null)), table128, "Given ");
#line 19
 testRunner.When("I run the high volume rule", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 20
 testRunner.Then("I will have 0 high volume alerts", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("One order at market cap yields one alert")]
        [NUnit.Framework.CategoryAttribute("highvolume")]
        [NUnit.Framework.CategoryAttribute("highvolumemarketcap")]
        [NUnit.Framework.CategoryAttribute("highvolumemarketcapsensitive")]
        public virtual void OneOrderAtMarketCapYieldsOneAlert()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("One order at market cap yields one alert", null, new string[] {
                        "highvolume",
                        "highvolumemarketcap",
                        "highvolumemarketcapsensitive"});
#line 25
this.ScenarioInitialize(scenarioInfo);
            this.ScenarioStart();
#line 8
this.FeatureBackground();
#line hidden
            TechTalk.SpecFlow.Table table129 = new TechTalk.SpecFlow.Table(new string[] {
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
            table129.AddRow(new string[] {
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
                        "1000",
                        "1000"});
#line 26
 testRunner.Given("I have the orders for a universe from 01/01/2018 to 03/01/2018 :", ((string)(null)), table129, "Given ");
#line hidden
            TechTalk.SpecFlow.Table table130 = new TechTalk.SpecFlow.Table(new string[] {
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
            table130.AddRow(new string[] {
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
 testRunner.And("With the interday market data :", ((string)(null)), table130, "And ");
#line 32
 testRunner.When("I run the high volume rule", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 33
 testRunner.Then("I will have 1 high volume alerts", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("One order below market cap yields zero alerts")]
        [NUnit.Framework.CategoryAttribute("highvolume")]
        [NUnit.Framework.CategoryAttribute("highvolumemarketcap")]
        [NUnit.Framework.CategoryAttribute("highvolumemarketcapsensitive")]
        public virtual void OneOrderBelowMarketCapYieldsZeroAlerts()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("One order below market cap yields zero alerts", null, new string[] {
                        "highvolume",
                        "highvolumemarketcap",
                        "highvolumemarketcapsensitive"});
#line 38
this.ScenarioInitialize(scenarioInfo);
            this.ScenarioStart();
#line 8
this.FeatureBackground();
#line hidden
            TechTalk.SpecFlow.Table table131 = new TechTalk.SpecFlow.Table(new string[] {
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
            table131.AddRow(new string[] {
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
                        "999",
                        "999"});
#line 39
 testRunner.Given("I have the orders for a universe from 01/01/2018 to 03/01/2018 :", ((string)(null)), table131, "Given ");
#line hidden
            TechTalk.SpecFlow.Table table132 = new TechTalk.SpecFlow.Table(new string[] {
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
            table132.AddRow(new string[] {
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
#line 42
 testRunner.And("With the interday market data :", ((string)(null)), table132, "And ");
#line 45
 testRunner.When("I run the high volume rule", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 46
 testRunner.Then("I will have 0 high volume alerts", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("One order above market cap yields one alerts")]
        [NUnit.Framework.CategoryAttribute("highvolume")]
        [NUnit.Framework.CategoryAttribute("highvolumemarketcap")]
        [NUnit.Framework.CategoryAttribute("highvolumemarketcapsensitive")]
        public virtual void OneOrderAboveMarketCapYieldsOneAlerts()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("One order above market cap yields one alerts", null, new string[] {
                        "highvolume",
                        "highvolumemarketcap",
                        "highvolumemarketcapsensitive"});
#line 52
this.ScenarioInitialize(scenarioInfo);
            this.ScenarioStart();
#line 8
this.FeatureBackground();
#line hidden
            TechTalk.SpecFlow.Table table133 = new TechTalk.SpecFlow.Table(new string[] {
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
            table133.AddRow(new string[] {
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
                        "1001",
                        "1000"});
#line 53
 testRunner.Given("I have the orders for a universe from 01/01/2018 to 03/01/2018 :", ((string)(null)), table133, "Given ");
#line hidden
            TechTalk.SpecFlow.Table table134 = new TechTalk.SpecFlow.Table(new string[] {
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
            table134.AddRow(new string[] {
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
#line 56
 testRunner.And("With the interday market data :", ((string)(null)), table134, "And ");
#line 59
 testRunner.When("I run the high volume rule", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 60
 testRunner.Then("I will have 1 high volume alerts", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Two order at market cap at window yields one alert")]
        [NUnit.Framework.CategoryAttribute("highvolume")]
        [NUnit.Framework.CategoryAttribute("highvolumemarketcap")]
        [NUnit.Framework.CategoryAttribute("highvolumemarketcapsensitive")]
        public virtual void TwoOrderAtMarketCapAtWindowYieldsOneAlert()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Two order at market cap at window yields one alert", null, new string[] {
                        "highvolume",
                        "highvolumemarketcap",
                        "highvolumemarketcapsensitive"});
#line 66
this.ScenarioInitialize(scenarioInfo);
            this.ScenarioStart();
#line 8
this.FeatureBackground();
#line hidden
            TechTalk.SpecFlow.Table table135 = new TechTalk.SpecFlow.Table(new string[] {
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
            table135.AddRow(new string[] {
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
                        "500",
                        "500"});
            table135.AddRow(new string[] {
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
                        "500",
                        "500"});
#line 67
 testRunner.Given("I have the orders for a universe from 01/01/2018 to 03/01/2018 :", ((string)(null)), table135, "Given ");
#line hidden
            TechTalk.SpecFlow.Table table136 = new TechTalk.SpecFlow.Table(new string[] {
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
            table136.AddRow(new string[] {
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
#line 71
 testRunner.And("With the interday market data :", ((string)(null)), table136, "And ");
#line 74
 testRunner.When("I run the high volume rule", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 75
 testRunner.Then("I will have 1 high volume alerts", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Two order at market cap but just outside window yields zero alerts")]
        [NUnit.Framework.CategoryAttribute("highvolume")]
        [NUnit.Framework.CategoryAttribute("highvolumemarketcap")]
        [NUnit.Framework.CategoryAttribute("highvolumemarketcapsensitive")]
        public virtual void TwoOrderAtMarketCapButJustOutsideWindowYieldsZeroAlerts()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Two order at market cap but just outside window yields zero alerts", null, new string[] {
                        "highvolume",
                        "highvolumemarketcap",
                        "highvolumemarketcapsensitive"});
#line 80
this.ScenarioInitialize(scenarioInfo);
            this.ScenarioStart();
#line 8
this.FeatureBackground();
#line hidden
            TechTalk.SpecFlow.Table table137 = new TechTalk.SpecFlow.Table(new string[] {
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
            table137.AddRow(new string[] {
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
                        "500",
                        "500"});
            table137.AddRow(new string[] {
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
                        "500",
                        "500"});
#line 81
 testRunner.Given("I have the orders for a universe from 01/01/2018 to 03/01/2018 :", ((string)(null)), table137, "Given ");
#line hidden
            TechTalk.SpecFlow.Table table138 = new TechTalk.SpecFlow.Table(new string[] {
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
            table138.AddRow(new string[] {
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
#line 85
 testRunner.And("With the interday market data :", ((string)(null)), table138, "And ");
#line 88
 testRunner.When("I run the high volume rule", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 89
 testRunner.Then("I will have 0 high volume alerts", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Two order at market cap and inside window yields one alert")]
        [NUnit.Framework.CategoryAttribute("highvolume")]
        [NUnit.Framework.CategoryAttribute("highvolumemarketcap")]
        [NUnit.Framework.CategoryAttribute("highvolumemarketcapsensitive")]
        public virtual void TwoOrderAtMarketCapAndInsideWindowYieldsOneAlert()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Two order at market cap and inside window yields one alert", null, new string[] {
                        "highvolume",
                        "highvolumemarketcap",
                        "highvolumemarketcapsensitive"});
#line 94
this.ScenarioInitialize(scenarioInfo);
            this.ScenarioStart();
#line 8
this.FeatureBackground();
#line hidden
            TechTalk.SpecFlow.Table table139 = new TechTalk.SpecFlow.Table(new string[] {
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
            table139.AddRow(new string[] {
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
                        "500",
                        "500"});
            table139.AddRow(new string[] {
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
                        "500",
                        "500"});
#line 95
 testRunner.Given("I have the orders for a universe from 01/01/2018 to 03/01/2018 :", ((string)(null)), table139, "Given ");
#line hidden
            TechTalk.SpecFlow.Table table140 = new TechTalk.SpecFlow.Table(new string[] {
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
            table140.AddRow(new string[] {
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
#line 99
 testRunner.And("With the interday market data :", ((string)(null)), table140, "And ");
#line 102
 testRunner.When("I run the high volume rule", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 103
 testRunner.Then("I will have 1 high volume alerts", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            this.ScenarioCleanup();
        }
    }
}
#pragma warning restore
#endregion
