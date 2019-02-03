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
    [NUnit.Framework.DescriptionAttribute("HighVolume Window Volume Non Sensitive Parameters")]
    public partial class HighVolumeWindowVolumeNonSensitiveParametersFeature
    {
        
        private TechTalk.SpecFlow.ITestRunner testRunner;
        
#line 1 "HighVolume.WindowVolume.NonSensitive.feature"
#line hidden
        
        [NUnit.Framework.OneTimeSetUpAttribute()]
        public virtual void FeatureSetup()
        {
            testRunner = TechTalk.SpecFlow.TestRunnerManager.GetTestRunner();
            TechTalk.SpecFlow.FeatureInfo featureInfo = new TechTalk.SpecFlow.FeatureInfo(new System.Globalization.CultureInfo("en-US"), "HighVolume Window Volume Non Sensitive Parameters", @"	In order to meet MAR compliance requirements
	I need to be able to detect when traders are executing trades
	At a volume where they are able to exert market manipulating pressure
	on the prices the market is trading at
	By measuring their security trades relative to the window volume traded of the company", ProgrammingLanguage.CSharp, ((string[])(null)));
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
            TechTalk.SpecFlow.Table table149 = new TechTalk.SpecFlow.Table(new string[] {
                        "WindowHours",
                        "HighVolumePercentageDaily",
                        "HighVolumePercentageWindow",
                        "HighVolumePercentageMarketCap"});
            table149.AddRow(new string[] {
                        "1",
                        "",
                        "0.1",
                        ""});
#line 9
 testRunner.Given("I have the high volume rule parameter values", ((string)(null)), table149, "Given ");
#line hidden
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Empty Universe yields no alerts")]
        [NUnit.Framework.CategoryAttribute("highvolume")]
        [NUnit.Framework.CategoryAttribute("highvolumewindow")]
        [NUnit.Framework.CategoryAttribute("highvolumewindownonsensitive")]
        public virtual void EmptyUniverseYieldsNoAlerts()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Empty Universe yields no alerts", null, new string[] {
                        "highvolume",
                        "highvolumewindow",
                        "highvolumewindownonsensitive"});
#line 16
this.ScenarioInitialize(scenarioInfo);
            this.ScenarioStart();
#line 8
this.FeatureBackground();
#line hidden
            TechTalk.SpecFlow.Table table150 = new TechTalk.SpecFlow.Table(new string[] {
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
 testRunner.Given("I have the orders for a universe from 01/01/2018 to 03/01/2018 :", ((string)(null)), table150, "Given ");
#line 19
 testRunner.When("I run the high volume rule", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 20
 testRunner.Then("I will have 0 high volume alerts", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("One order at window volume yields one alert")]
        [NUnit.Framework.CategoryAttribute("highvolume")]
        [NUnit.Framework.CategoryAttribute("highvolumewindow")]
        [NUnit.Framework.CategoryAttribute("highvolumewindownonsensitive")]
        public virtual void OneOrderAtWindowVolumeYieldsOneAlert()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("One order at window volume yields one alert", null, new string[] {
                        "highvolume",
                        "highvolumewindow",
                        "highvolumewindownonsensitive"});
#line 25
this.ScenarioInitialize(scenarioInfo);
            this.ScenarioStart();
#line 8
this.FeatureBackground();
#line hidden
            TechTalk.SpecFlow.Table table151 = new TechTalk.SpecFlow.Table(new string[] {
                        "SecurityName",
                        "OrderId",
                        "PlacedDate",
                        "FilledDate",
                        "Type",
                        "Direction",
                        "Currency",
                        "LimitPrice",
                        "AverageFillPrice",
                        "OrderedVolume",
                        "FilledVolume"});
            table151.AddRow(new string[] {
                        "Vodafone",
                        "0",
                        "01/01/2018 09:30:00",
                        "01/01/2018 09:30:00",
                        "MARKET",
                        "BUY",
                        "GBX",
                        "",
                        "10",
                        "1000",
                        "1000"});
#line 26
 testRunner.Given("I have the orders for a universe from 01/01/2018 to 03/01/2018 :", ((string)(null)), table151, "Given ");
#line hidden
            TechTalk.SpecFlow.Table table152 = new TechTalk.SpecFlow.Table(new string[] {
                        "SecurityName",
                        "Epoch",
                        "Bid",
                        "Ask",
                        "Price",
                        "Currency",
                        "Volume"});
            table152.AddRow(new string[] {
                        "Vodafone",
                        "01/01/2018  09:30:00",
                        "1",
                        "20",
                        "10",
                        "GBX",
                        "5000"});
            table152.AddRow(new string[] {
                        "Vodafone",
                        "01/01/2018  09:29:00",
                        "1",
                        "20",
                        "10",
                        "GBX",
                        "5000"});
#line 29
 testRunner.And("With the intraday market data :", ((string)(null)), table152, "And ");
#line 33
 testRunner.When("I run the high volume rule", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 34
 testRunner.Then("I will have 1 high volume alerts", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("One order in different currency and exchange at window volume yields one alert")]
        [NUnit.Framework.CategoryAttribute("highvolume")]
        [NUnit.Framework.CategoryAttribute("highvolumewindow")]
        [NUnit.Framework.CategoryAttribute("highvolumewindownonsensitive")]
        public virtual void OneOrderInDifferentCurrencyAndExchangeAtWindowVolumeYieldsOneAlert()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("One order in different currency and exchange at window volume yields one alert", null, new string[] {
                        "highvolume",
                        "highvolumewindow",
                        "highvolumewindownonsensitive"});
#line 39
this.ScenarioInitialize(scenarioInfo);
            this.ScenarioStart();
#line 8
this.FeatureBackground();
#line hidden
            TechTalk.SpecFlow.Table table153 = new TechTalk.SpecFlow.Table(new string[] {
                        "SecurityName",
                        "OrderId",
                        "PlacedDate",
                        "FilledDate",
                        "Type",
                        "Direction",
                        "Currency",
                        "LimitPrice",
                        "AverageFillPrice",
                        "OrderedVolume",
                        "FilledVolume"});
            table153.AddRow(new string[] {
                        "Nvidia",
                        "0",
                        "01/01/2018 17:30:00",
                        "01/01/2018 17:30:00",
                        "MARKET",
                        "BUY",
                        "USD",
                        "",
                        "10",
                        "1000",
                        "1000"});
#line 40
 testRunner.Given("I have the orders for a universe from 01/01/2018 to 03/01/2018 :", ((string)(null)), table153, "Given ");
#line hidden
            TechTalk.SpecFlow.Table table154 = new TechTalk.SpecFlow.Table(new string[] {
                        "SecurityName",
                        "Epoch",
                        "Bid",
                        "Ask",
                        "Price",
                        "Currency",
                        "Volume"});
            table154.AddRow(new string[] {
                        "Nvidia",
                        "01/01/2018  17:30:00",
                        "1",
                        "20",
                        "10",
                        "USD",
                        "5000"});
            table154.AddRow(new string[] {
                        "Nvidia",
                        "01/01/2018  17:29:00",
                        "1",
                        "20",
                        "10",
                        "USD",
                        "5000"});
#line 43
 testRunner.And("With the intraday market data :", ((string)(null)), table154, "And ");
#line 47
 testRunner.When("I run the high volume rule", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 48
 testRunner.Then("I will have 1 high volume alerts", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("One order below window volume yields zero alerts")]
        [NUnit.Framework.CategoryAttribute("highvolume")]
        [NUnit.Framework.CategoryAttribute("highvolumewindow")]
        [NUnit.Framework.CategoryAttribute("highvolumewindownonsensitive")]
        public virtual void OneOrderBelowWindowVolumeYieldsZeroAlerts()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("One order below window volume yields zero alerts", null, new string[] {
                        "highvolume",
                        "highvolumewindow",
                        "highvolumewindownonsensitive"});
#line 53
this.ScenarioInitialize(scenarioInfo);
            this.ScenarioStart();
#line 8
this.FeatureBackground();
#line hidden
            TechTalk.SpecFlow.Table table155 = new TechTalk.SpecFlow.Table(new string[] {
                        "SecurityName",
                        "OrderId",
                        "PlacedDate",
                        "FilledDate",
                        "Type",
                        "Direction",
                        "Currency",
                        "LimitPrice",
                        "AverageFillPrice",
                        "OrderedVolume",
                        "FilledVolume"});
            table155.AddRow(new string[] {
                        "Vodafone",
                        "0",
                        "01/01/2018 09:30:00",
                        "01/01/2018 09:30:00",
                        "MARKET",
                        "BUY",
                        "GBX",
                        "",
                        "10",
                        "999",
                        "999"});
#line 54
 testRunner.Given("I have the orders for a universe from 01/01/2018 to 03/01/2018 :", ((string)(null)), table155, "Given ");
#line hidden
            TechTalk.SpecFlow.Table table156 = new TechTalk.SpecFlow.Table(new string[] {
                        "SecurityName",
                        "Epoch",
                        "Bid",
                        "Ask",
                        "Price",
                        "Currency",
                        "Volume"});
            table156.AddRow(new string[] {
                        "Vodafone",
                        "01/01/2018 09:30:00",
                        "1",
                        "20",
                        "10",
                        "GBX",
                        "10000"});
#line 57
 testRunner.And("With the intraday market data :", ((string)(null)), table156, "And ");
#line 60
 testRunner.When("I run the high volume rule", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 61
 testRunner.Then("I will have 0 high volume alerts", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("One order above window volume yields one alerts")]
        [NUnit.Framework.CategoryAttribute("highvolume")]
        [NUnit.Framework.CategoryAttribute("highvolumewindow")]
        [NUnit.Framework.CategoryAttribute("highvolumewindownonsensitive")]
        public virtual void OneOrderAboveWindowVolumeYieldsOneAlerts()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("One order above window volume yields one alerts", null, new string[] {
                        "highvolume",
                        "highvolumewindow",
                        "highvolumewindownonsensitive"});
#line 66
this.ScenarioInitialize(scenarioInfo);
            this.ScenarioStart();
#line 8
this.FeatureBackground();
#line hidden
            TechTalk.SpecFlow.Table table157 = new TechTalk.SpecFlow.Table(new string[] {
                        "SecurityName",
                        "OrderId",
                        "PlacedDate",
                        "FilledDate",
                        "Type",
                        "Direction",
                        "Currency",
                        "LimitPrice",
                        "AverageFillPrice",
                        "OrderedVolume",
                        "FilledVolume"});
            table157.AddRow(new string[] {
                        "Vodafone",
                        "0",
                        "01/01/2018 09:30:00",
                        "01/01/2018 09:30:00",
                        "MARKET",
                        "BUY",
                        "GBX",
                        "",
                        "10",
                        "1001",
                        "1001"});
#line 67
 testRunner.Given("I have the orders for a universe from 01/01/2018 to 03/01/2018 :", ((string)(null)), table157, "Given ");
#line hidden
            TechTalk.SpecFlow.Table table158 = new TechTalk.SpecFlow.Table(new string[] {
                        "SecurityName",
                        "Epoch",
                        "Bid",
                        "Ask",
                        "Price",
                        "Currency",
                        "Volume"});
            table158.AddRow(new string[] {
                        "Vodafone",
                        "01/01/2018 09:30:00",
                        "1",
                        "20",
                        "10",
                        "GBX",
                        "10000"});
#line 70
 testRunner.And("With the intraday market data :", ((string)(null)), table158, "And ");
#line 73
 testRunner.When("I run the high volume rule", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 74
 testRunner.Then("I will have 1 high volume alerts", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Two orders split just outside of window at window volume yields zero alert")]
        [NUnit.Framework.CategoryAttribute("highvolume")]
        [NUnit.Framework.CategoryAttribute("highvolumewindow")]
        [NUnit.Framework.CategoryAttribute("highvolumewindownonsensitive")]
        public virtual void TwoOrdersSplitJustOutsideOfWindowAtWindowVolumeYieldsZeroAlert()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Two orders split just outside of window at window volume yields zero alert", null, new string[] {
                        "highvolume",
                        "highvolumewindow",
                        "highvolumewindownonsensitive"});
#line 79
this.ScenarioInitialize(scenarioInfo);
            this.ScenarioStart();
#line 8
this.FeatureBackground();
#line hidden
            TechTalk.SpecFlow.Table table159 = new TechTalk.SpecFlow.Table(new string[] {
                        "SecurityName",
                        "OrderId",
                        "PlacedDate",
                        "FilledDate",
                        "Type",
                        "Direction",
                        "Currency",
                        "LimitPrice",
                        "AverageFillPrice",
                        "OrderedVolume",
                        "FilledVolume"});
            table159.AddRow(new string[] {
                        "Vodafone",
                        "0",
                        "01/01/2018 09:30:00",
                        "01/01/2018 09:30:00",
                        "MARKET",
                        "BUY",
                        "GBX",
                        "",
                        "10",
                        "500",
                        "500"});
            table159.AddRow(new string[] {
                        "Vodafone",
                        "0",
                        "01/01/2018 10:35:00",
                        "01/01/2018 10:35:00",
                        "MARKET",
                        "BUY",
                        "GBX",
                        "",
                        "10",
                        "500",
                        "500"});
#line 80
 testRunner.Given("I have the orders for a universe from 01/01/2018 to 03/01/2018 :", ((string)(null)), table159, "Given ");
#line hidden
            TechTalk.SpecFlow.Table table160 = new TechTalk.SpecFlow.Table(new string[] {
                        "SecurityName",
                        "Epoch",
                        "Bid",
                        "Ask",
                        "Price",
                        "Currency",
                        "Volume"});
            table160.AddRow(new string[] {
                        "Vodafone",
                        "01/01/2018  09:30:00",
                        "1",
                        "20",
                        "10",
                        "GBX",
                        "5000"});
            table160.AddRow(new string[] {
                        "Vodafone",
                        "01/01/2018  09:29:00",
                        "1",
                        "20",
                        "10",
                        "GBX",
                        "5000"});
#line 84
 testRunner.And("With the intraday market data :", ((string)(null)), table160, "And ");
#line 88
 testRunner.When("I run the high volume rule", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 89
 testRunner.Then("I will have 0 high volume alerts", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Two orders split just on window at window volume yields one alert")]
        [NUnit.Framework.CategoryAttribute("highvolume")]
        [NUnit.Framework.CategoryAttribute("highvolumewindow")]
        [NUnit.Framework.CategoryAttribute("highvolumewindownonsensitive")]
        public virtual void TwoOrdersSplitJustOnWindowAtWindowVolumeYieldsOneAlert()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Two orders split just on window at window volume yields one alert", null, new string[] {
                        "highvolume",
                        "highvolumewindow",
                        "highvolumewindownonsensitive"});
#line 94
this.ScenarioInitialize(scenarioInfo);
            this.ScenarioStart();
#line 8
this.FeatureBackground();
#line hidden
            TechTalk.SpecFlow.Table table161 = new TechTalk.SpecFlow.Table(new string[] {
                        "SecurityName",
                        "OrderId",
                        "PlacedDate",
                        "FilledDate",
                        "Type",
                        "Direction",
                        "Currency",
                        "LimitPrice",
                        "AverageFillPrice",
                        "OrderedVolume",
                        "FilledVolume"});
            table161.AddRow(new string[] {
                        "Vodafone",
                        "0",
                        "01/01/2018 09:30:00",
                        "01/01/2018 09:30:00",
                        "MARKET",
                        "BUY",
                        "GBX",
                        "",
                        "10",
                        "500",
                        "500"});
            table161.AddRow(new string[] {
                        "Vodafone",
                        "0",
                        "01/01/2018 10:30:00",
                        "01/01/2018 10:30:00",
                        "MARKET",
                        "BUY",
                        "GBX",
                        "",
                        "10",
                        "500",
                        "500"});
#line 95
 testRunner.Given("I have the orders for a universe from 01/01/2018 to 03/01/2018 :", ((string)(null)), table161, "Given ");
#line hidden
            TechTalk.SpecFlow.Table table162 = new TechTalk.SpecFlow.Table(new string[] {
                        "SecurityName",
                        "Epoch",
                        "Bid",
                        "Ask",
                        "Price",
                        "Currency",
                        "Volume"});
            table162.AddRow(new string[] {
                        "Vodafone",
                        "01/01/2018  09:30:00",
                        "1",
                        "20",
                        "10",
                        "GBX",
                        "5000"});
            table162.AddRow(new string[] {
                        "Vodafone",
                        "01/01/2018  09:29:00",
                        "1",
                        "20",
                        "10",
                        "GBX",
                        "5000"});
#line 99
 testRunner.And("With the intraday market data :", ((string)(null)), table162, "And ");
#line 103
 testRunner.When("I run the high volume rule", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 104
 testRunner.Then("I will have 1 high volume alerts", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Two orders split just inside window at window volume yields one alert")]
        [NUnit.Framework.CategoryAttribute("highvolume")]
        [NUnit.Framework.CategoryAttribute("highvolumewindow")]
        [NUnit.Framework.CategoryAttribute("highvolumewindownonsensitive")]
        public virtual void TwoOrdersSplitJustInsideWindowAtWindowVolumeYieldsOneAlert()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Two orders split just inside window at window volume yields one alert", null, new string[] {
                        "highvolume",
                        "highvolumewindow",
                        "highvolumewindownonsensitive"});
#line 109
this.ScenarioInitialize(scenarioInfo);
            this.ScenarioStart();
#line 8
this.FeatureBackground();
#line hidden
            TechTalk.SpecFlow.Table table163 = new TechTalk.SpecFlow.Table(new string[] {
                        "SecurityName",
                        "OrderId",
                        "PlacedDate",
                        "FilledDate",
                        "Type",
                        "Direction",
                        "Currency",
                        "LimitPrice",
                        "AverageFillPrice",
                        "OrderedVolume",
                        "FilledVolume"});
            table163.AddRow(new string[] {
                        "Vodafone",
                        "0",
                        "01/01/2018 09:30:00",
                        "01/01/2018 09:30:00",
                        "MARKET",
                        "BUY",
                        "GBX",
                        "",
                        "10",
                        "500",
                        "500"});
            table163.AddRow(new string[] {
                        "Vodafone",
                        "0",
                        "01/01/2018 10:25:00",
                        "01/01/2018 10:25:00",
                        "MARKET",
                        "BUY",
                        "GBX",
                        "",
                        "10",
                        "500",
                        "500"});
#line 110
 testRunner.Given("I have the orders for a universe from 01/01/2018 to 03/01/2018 :", ((string)(null)), table163, "Given ");
#line hidden
            TechTalk.SpecFlow.Table table164 = new TechTalk.SpecFlow.Table(new string[] {
                        "SecurityName",
                        "Epoch",
                        "Bid",
                        "Ask",
                        "Price",
                        "Currency",
                        "Volume"});
            table164.AddRow(new string[] {
                        "Vodafone",
                        "01/01/2018  09:30:00",
                        "1",
                        "20",
                        "10",
                        "GBX",
                        "5000"});
            table164.AddRow(new string[] {
                        "Vodafone",
                        "01/01/2018  09:29:00",
                        "1",
                        "20",
                        "10",
                        "GBX",
                        "5000"});
#line 114
 testRunner.And("With the intraday market data :", ((string)(null)), table164, "And ");
#line 118
 testRunner.When("I run the high volume rule", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 119
 testRunner.Then("I will have 1 high volume alerts", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            this.ScenarioCleanup();
        }
    }
}
#pragma warning restore
#endregion
