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
    [NUnit.Framework.DescriptionAttribute("WashTrade Pairing Non Sensitive Parameters")]
    public partial class WashTradePairingNonSensitiveParametersFeature
    {
        
        private TechTalk.SpecFlow.ITestRunner testRunner;
        
#line 1 "WashTrade.Pairing.NonSensitive.feature"
#line hidden
        
        [NUnit.Framework.OneTimeSetUpAttribute()]
        public virtual void FeatureSetup()
        {
            testRunner = TechTalk.SpecFlow.TestRunnerManager.GetTestRunner();
            TechTalk.SpecFlow.FeatureInfo featureInfo = new TechTalk.SpecFlow.FeatureInfo(new System.Globalization.CultureInfo("en-US"), "WashTrade Pairing Non Sensitive Parameters", "\tIn order to meet MAR compliance requirements\r\n\tI need to be able to detect when " +
                    "traders are executing trades\r\n\twith no meaningful change of ownership\r\n\tBy pairi" +
                    "ng their trades for average value change being below\r\n\tthreshold parameters", ProgrammingLanguage.CSharp, ((string[])(null)));
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
            TechTalk.SpecFlow.Table table46 = new TechTalk.SpecFlow.Table(new string[] {
                        "WindowHours",
                        "PairingPositionMinimumNumberOfPairedTrades",
                        "PairingPositionPercentagePriceChangeThresholdPerPair",
                        "PairingPositionPercentageVolumeDifferenceThreshold",
                        "PairingPositionMaximumAbsoluteCurrencyAmount",
                        "PairingPositionMaximumAbsoluteCurrency",
                        "UsePairing"});
            table46.AddRow(new string[] {
                        "1",
                        "2",
                        "0.10",
                        "0.10",
                        "1000000",
                        "GBX",
                        "true"});
#line 9
 testRunner.Given("I have the wash trade rule parameter values", ((string)(null)), table46, "Given ");
#line hidden
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Empty Universe yields no alerts")]
        [NUnit.Framework.CategoryAttribute("washtrade")]
        [NUnit.Framework.CategoryAttribute("washtradepairing")]
        [NUnit.Framework.CategoryAttribute("washtradenonsensitive")]
        public virtual void EmptyUniverseYieldsNoAlerts()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Empty Universe yields no alerts", null, new string[] {
                        "washtrade",
                        "washtradepairing",
                        "washtradenonsensitive"});
#line 16
this.ScenarioInitialize(scenarioInfo);
            this.ScenarioStart();
#line 8
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
#line 17
 testRunner.Given("I have the orders for a universe from 01/01/2018 to 03/01/2018 :", ((string)(null)), table47, "Given ");
#line 19
 testRunner.When("I run the wash trade rule", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 20
 testRunner.Then("I will have 0 wash trade alerts", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("One Trade For Vodafone yields no alerts")]
        [NUnit.Framework.CategoryAttribute("washtrade")]
        [NUnit.Framework.CategoryAttribute("washtradepairing")]
        [NUnit.Framework.CategoryAttribute("washtradenonsensitive")]
        public virtual void OneTradeForVodafoneYieldsNoAlerts()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("One Trade For Vodafone yields no alerts", null, new string[] {
                        "washtrade",
                        "washtradepairing",
                        "washtradenonsensitive"});
#line 25
this.ScenarioInitialize(scenarioInfo);
            this.ScenarioStart();
#line 8
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
                        "100",
                        "1000",
                        "1000"});
#line 26
 testRunner.Given("I have the orders for a universe from 01/01/2018 to 03/01/2018 :", ((string)(null)), table48, "Given ");
#line 29
 testRunner.When("I run the wash trade rule", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 30
 testRunner.Then("I will have 0 wash trade alerts", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("One Trade For Barclays yields no alerts")]
        [NUnit.Framework.CategoryAttribute("washtrade")]
        [NUnit.Framework.CategoryAttribute("washtradepairing")]
        [NUnit.Framework.CategoryAttribute("washtradenonsensitive")]
        public virtual void OneTradeForBarclaysYieldsNoAlerts()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("One Trade For Barclays yields no alerts", null, new string[] {
                        "washtrade",
                        "washtradepairing",
                        "washtradenonsensitive"});
#line 35
this.ScenarioInitialize(scenarioInfo);
            this.ScenarioStart();
#line 8
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
                        "1000"});
#line 36
 testRunner.Given("I have the orders for a universe from 01/01/2018 to 03/01/2018 :", ((string)(null)), table49, "Given ");
#line 39
 testRunner.When("I run the wash trade rule", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 40
 testRunner.Then("I will have 0 wash trade alerts", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Two Trades In Wash Trade For Different Securities yields no alert")]
        [NUnit.Framework.CategoryAttribute("washtrade")]
        [NUnit.Framework.CategoryAttribute("washtradepairing")]
        [NUnit.Framework.CategoryAttribute("washtradenonsensitive")]
        public virtual void TwoTradesInWashTradeForDifferentSecuritiesYieldsNoAlert()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Two Trades In Wash Trade For Different Securities yields no alert", null, new string[] {
                        "washtrade",
                        "washtradepairing",
                        "washtradenonsensitive"});
#line 45
this.ScenarioInitialize(scenarioInfo);
            this.ScenarioStart();
#line 8
this.FeatureBackground();
#line hidden
            TechTalk.SpecFlow.Table table50 = new TechTalk.SpecFlow.Table(new string[] {
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
            table50.AddRow(new string[] {
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
                        "1000"});
            table50.AddRow(new string[] {
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
                        "1000"});
#line 46
 testRunner.Given("I have the orders for a universe from 01/01/2018 to 03/01/2018 :", ((string)(null)), table50, "Given ");
#line 50
 testRunner.When("I run the wash trade rule", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 51
 testRunner.Then("I will have 0 wash trade alerts", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Three Trades at same price point In Wash Trade yields no alerts")]
        [NUnit.Framework.CategoryAttribute("washtrade")]
        [NUnit.Framework.CategoryAttribute("washtradepairing")]
        [NUnit.Framework.CategoryAttribute("washtradenonsensitive")]
        public virtual void ThreeTradesAtSamePricePointInWashTradeYieldsNoAlerts()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Three Trades at same price point In Wash Trade yields no alerts", null, new string[] {
                        "washtrade",
                        "washtradepairing",
                        "washtradenonsensitive"});
#line 56
this.ScenarioInitialize(scenarioInfo);
            this.ScenarioStart();
#line 8
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
                        "1000",
                        "1000"});
            table51.AddRow(new string[] {
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
                        "1000"});
            table51.AddRow(new string[] {
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
                        "1000"});
#line 57
 testRunner.Given("I have the orders for a universe from 01/01/2018 to 03/01/2018 :", ((string)(null)), table51, "Given ");
#line 62
 testRunner.When("I run the wash trade rule", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 63
 testRunner.Then("I will have 0 wash trade alerts", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Two Trades In Wash Trade yields one alert")]
        [NUnit.Framework.CategoryAttribute("washtrade")]
        [NUnit.Framework.CategoryAttribute("washtradepairing")]
        [NUnit.Framework.CategoryAttribute("washtradenonsensitive")]
        public virtual void TwoTradesInWashTradeYieldsOneAlert()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Two Trades In Wash Trade yields one alert", null, new string[] {
                        "washtrade",
                        "washtradepairing",
                        "washtradenonsensitive"});
#line 68
this.ScenarioInitialize(scenarioInfo);
            this.ScenarioStart();
#line 8
this.FeatureBackground();
#line hidden
            TechTalk.SpecFlow.Table table52 = new TechTalk.SpecFlow.Table(new string[] {
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
            table52.AddRow(new string[] {
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
                        "1000"});
            table52.AddRow(new string[] {
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
                        "1000"});
#line 69
 testRunner.Given("I have the orders for a universe from 01/01/2018 to 03/01/2018 :", ((string)(null)), table52, "Given ");
#line 73
 testRunner.When("I run the wash trade rule", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 74
 testRunner.Then("I will have 1 wash trade alerts", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Two Trade For Micron yields one alerts when within 1 hour")]
        [NUnit.Framework.CategoryAttribute("washtrade")]
        [NUnit.Framework.CategoryAttribute("washtradepairing")]
        [NUnit.Framework.CategoryAttribute("washtradenonsensitive")]
        [NUnit.Framework.CategoryAttribute("timewindow")]
        public virtual void TwoTradeForMicronYieldsOneAlertsWhenWithin1Hour()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Two Trade For Micron yields one alerts when within 1 hour", null, new string[] {
                        "washtrade",
                        "washtradepairing",
                        "washtradenonsensitive",
                        "timewindow"});
#line 81
this.ScenarioInitialize(scenarioInfo);
            this.ScenarioStart();
#line 8
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
                        "Micron",
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
                        "1000"});
            table53.AddRow(new string[] {
                        "Micron",
                        "1",
                        "01/01/2018 10:00:00",
                        "",
                        "",
                        "",
                        "",
                        "01/01/2018 10:00:00",
                        "MARKET",
                        "SELL",
                        "GBX",
                        "",
                        "100",
                        "1000",
                        "1000"});
#line 82
 testRunner.Given("I have the orders for a universe from 01/01/2018 to 03/01/2018 :", ((string)(null)), table53, "Given ");
#line 86
 testRunner.When("I run the wash trade rule", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 87
 testRunner.Then("I will have 1 wash trade alerts", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Two Trade For Micron yields no alerts when 2 hours apart")]
        [NUnit.Framework.CategoryAttribute("washtrade")]
        [NUnit.Framework.CategoryAttribute("washtradepairing")]
        [NUnit.Framework.CategoryAttribute("washtradenonsensitive")]
        [NUnit.Framework.CategoryAttribute("timewindow")]
        public virtual void TwoTradeForMicronYieldsNoAlertsWhen2HoursApart()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Two Trade For Micron yields no alerts when 2 hours apart", null, new string[] {
                        "washtrade",
                        "washtradepairing",
                        "washtradenonsensitive",
                        "timewindow"});
#line 93
this.ScenarioInitialize(scenarioInfo);
            this.ScenarioStart();
#line 8
this.FeatureBackground();
#line hidden
            TechTalk.SpecFlow.Table table54 = new TechTalk.SpecFlow.Table(new string[] {
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
            table54.AddRow(new string[] {
                        "Micron",
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
                        "1000"});
            table54.AddRow(new string[] {
                        "Micron",
                        "1",
                        "01/01/2018 11:30:00",
                        "",
                        "",
                        "",
                        "",
                        "01/01/2018 11:30:00",
                        "MARKET",
                        "SELL",
                        "GBX",
                        "",
                        "100",
                        "1000",
                        "1000"});
#line 94
 testRunner.Given("I have the orders for a universe from 01/01/2018 to 03/01/2018 :", ((string)(null)), table54, "Given ");
#line 98
 testRunner.When("I run the wash trade rule", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 99
 testRunner.Then("I will have 0 wash trade alerts", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Two Trade For Micron yields one alerts when exactly 1 hour apart")]
        [NUnit.Framework.CategoryAttribute("washtrade")]
        [NUnit.Framework.CategoryAttribute("washtradepairing")]
        [NUnit.Framework.CategoryAttribute("washtradenonsensitive")]
        [NUnit.Framework.CategoryAttribute("timewindow")]
        public virtual void TwoTradeForMicronYieldsOneAlertsWhenExactly1HourApart()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Two Trade For Micron yields one alerts when exactly 1 hour apart", null, new string[] {
                        "washtrade",
                        "washtradepairing",
                        "washtradenonsensitive",
                        "timewindow"});
#line 105
this.ScenarioInitialize(scenarioInfo);
            this.ScenarioStart();
#line 8
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
                        "Micron",
                        "0",
                        "01/01/2018 09:33:00",
                        "",
                        "",
                        "",
                        "",
                        "01/01/2018 09:33:00",
                        "MARKET",
                        "BUY",
                        "GBX",
                        "",
                        "100",
                        "1000",
                        "1000"});
            table55.AddRow(new string[] {
                        "Micron",
                        "1",
                        "01/01/2018 10:33:00",
                        "",
                        "",
                        "",
                        "",
                        "01/01/2018 10:33:00",
                        "MARKET",
                        "SELL",
                        "GBX",
                        "",
                        "100",
                        "1000",
                        "1000"});
#line 106
 testRunner.Given("I have the orders for a universe from 01/01/2018 to 03/01/2018 :", ((string)(null)), table55, "Given ");
#line 110
 testRunner.When("I run the wash trade rule", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 111
 testRunner.Then("I will have 1 wash trade alerts", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            this.ScenarioCleanup();
        }
    }
}
#pragma warning restore
#endregion
