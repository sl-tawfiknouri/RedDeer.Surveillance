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
    [NUnit.Framework.DescriptionAttribute("HighVolume Daily Volume Sensitive Parameters")]
    public partial class HighVolumeDailyVolumeSensitiveParametersFeature
    {
        
        private TechTalk.SpecFlow.ITestRunner testRunner;
        
#line 1 "HighVolume.DailyVolume.Sensitive.feature"
#line hidden
        
        [NUnit.Framework.OneTimeSetUpAttribute()]
        public virtual void FeatureSetup()
        {
            testRunner = TechTalk.SpecFlow.TestRunnerManager.GetTestRunner();
            TechTalk.SpecFlow.FeatureInfo featureInfo = new TechTalk.SpecFlow.FeatureInfo(new System.Globalization.CultureInfo("en-US"), "HighVolume Daily Volume Sensitive Parameters", @"	In order to meet MAR compliance requirements
	I need to be able to detect when traders are executing trades
	At a volume where they are able to exert market manipulating pressure
	on the prices the market is trading at
	By measuring their security trades relative to the daily volume traded of the underlying company", ProgrammingLanguage.CSharp, ((string[])(null)));
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
            TechTalk.SpecFlow.Table table23 = new TechTalk.SpecFlow.Table(new string[] {
                        "WindowHours",
                        "HighVolumePercentageDaily",
                        "HighVolumePercentageWindow",
                        "HighVolumePercentageMarketCap"});
            table23.AddRow(new string[] {
                        "1",
                        "0.01",
                        "",
                        ""});
#line 9
 testRunner.Given("I have the high volume rule parameter values", ((string)(null)), table23, "Given ");
#line hidden
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Empty Universe yields no alerts")]
        [NUnit.Framework.CategoryAttribute("highvolume")]
        [NUnit.Framework.CategoryAttribute("highvolumeDaily")]
        [NUnit.Framework.CategoryAttribute("highvolumeDailySensitive")]
        public virtual void EmptyUniverseYieldsNoAlerts()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Empty Universe yields no alerts", null, new string[] {
                        "highvolume",
                        "highvolumeDaily",
                        "highvolumeDailySensitive"});
#line 16
this.ScenarioInitialize(scenarioInfo);
            this.ScenarioStart();
#line 8
this.FeatureBackground();
#line hidden
            TechTalk.SpecFlow.Table table24 = new TechTalk.SpecFlow.Table(new string[] {
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
 testRunner.Given("I have the orders for a universe from 01/01/2018 to 03/01/2018 :", ((string)(null)), table24, "Given ");
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
        [NUnit.Framework.CategoryAttribute("highvolumeDailySensitive")]
        public virtual void OneOrderAtDailyVolumeYieldsOneAlert()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("One order at daily volume yields one alert", null, new string[] {
                        "highvolume",
                        "highvolumeDaily",
                        "highvolumeDailySensitive"});
#line 25
this.ScenarioInitialize(scenarioInfo);
            this.ScenarioStart();
#line 8
this.FeatureBackground();
#line hidden
            TechTalk.SpecFlow.Table table25 = new TechTalk.SpecFlow.Table(new string[] {
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
            table25.AddRow(new string[] {
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
                        "10",
                        "10"});
#line 26
 testRunner.Given("I have the orders for a universe from 01/01/2018 to 03/01/2018 :", ((string)(null)), table25, "Given ");
#line hidden
            TechTalk.SpecFlow.Table table26 = new TechTalk.SpecFlow.Table(new string[] {
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
            table26.AddRow(new string[] {
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
 testRunner.And("With the interday market data :", ((string)(null)), table26, "And ");
#line 32
 testRunner.When("I run the high volume rule", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 33
 testRunner.Then("I will have 1 high volume alerts", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("One order below daily volume yields zero alerts")]
        [NUnit.Framework.CategoryAttribute("highvolume")]
        [NUnit.Framework.CategoryAttribute("highvolumeDaily")]
        [NUnit.Framework.CategoryAttribute("highvolumeDailySensitive")]
        public virtual void OneOrderBelowDailyVolumeYieldsZeroAlerts()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("One order below daily volume yields zero alerts", null, new string[] {
                        "highvolume",
                        "highvolumeDaily",
                        "highvolumeDailySensitive"});
#line 38
this.ScenarioInitialize(scenarioInfo);
            this.ScenarioStart();
#line 8
this.FeatureBackground();
#line hidden
            TechTalk.SpecFlow.Table table27 = new TechTalk.SpecFlow.Table(new string[] {
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
            table27.AddRow(new string[] {
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
                        "9",
                        "9"});
#line 39
 testRunner.Given("I have the orders for a universe from 01/01/2018 to 03/01/2018 :", ((string)(null)), table27, "Given ");
#line hidden
            TechTalk.SpecFlow.Table table28 = new TechTalk.SpecFlow.Table(new string[] {
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
            table28.AddRow(new string[] {
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
 testRunner.And("With the interday market data :", ((string)(null)), table28, "And ");
#line 45
 testRunner.When("I run the high volume rule", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 46
 testRunner.Then("I will have 0 high volume alerts", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("One order above daily volume yields one alerts")]
        [NUnit.Framework.CategoryAttribute("highvolume")]
        [NUnit.Framework.CategoryAttribute("highvolumeDaily")]
        [NUnit.Framework.CategoryAttribute("highvolumeDailySensitive")]
        public virtual void OneOrderAboveDailyVolumeYieldsOneAlerts()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("One order above daily volume yields one alerts", null, new string[] {
                        "highvolume",
                        "highvolumeDaily",
                        "highvolumeDailySensitive"});
#line 51
this.ScenarioInitialize(scenarioInfo);
            this.ScenarioStart();
#line 8
this.FeatureBackground();
#line hidden
            TechTalk.SpecFlow.Table table29 = new TechTalk.SpecFlow.Table(new string[] {
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
            table29.AddRow(new string[] {
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
                        "11",
                        "11"});
#line 52
 testRunner.Given("I have the orders for a universe from 01/01/2018 to 03/01/2018 :", ((string)(null)), table29, "Given ");
#line hidden
            TechTalk.SpecFlow.Table table30 = new TechTalk.SpecFlow.Table(new string[] {
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
            table30.AddRow(new string[] {
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
 testRunner.And("With the interday market data :", ((string)(null)), table30, "And ");
#line 58
 testRunner.When("I run the high volume rule", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 59
 testRunner.Then("I will have 1 high volume alerts", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            this.ScenarioCleanup();
        }
    }
}
#pragma warning restore
#endregion
