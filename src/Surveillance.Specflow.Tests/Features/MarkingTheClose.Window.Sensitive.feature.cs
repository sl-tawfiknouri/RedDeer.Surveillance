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
    [NUnit.Framework.DescriptionAttribute("MarkingTheClose Window Sensitive Parameters")]
    [NUnit.Framework.CategoryAttribute("markingtheclose")]
    [NUnit.Framework.CategoryAttribute("markingtheclosewindow")]
    [NUnit.Framework.CategoryAttribute("markingtheclosewindowsensitive")]
    public partial class MarkingTheCloseWindowSensitiveParametersFeature
    {
        
        private TechTalk.SpecFlow.ITestRunner testRunner;
        
#line 1 "MarkingTheClose.Window.Sensitive.feature"
#line hidden
        
        [NUnit.Framework.OneTimeSetUpAttribute()]
        public virtual void FeatureSetup()
        {
            testRunner = TechTalk.SpecFlow.TestRunnerManager.GetTestRunner();
            TechTalk.SpecFlow.FeatureInfo featureInfo = new TechTalk.SpecFlow.FeatureInfo(new System.Globalization.CultureInfo("en-US"), "MarkingTheClose Window Sensitive Parameters", "\tIn order to meet MAR compliance requirements\r\n\tI need to be able to detect when " +
                    "traders are executing trades\r\n\ttowards the market closure time at an unusually\r\n" +
                    "\thigh volume in order to extract supernormal profits", ProgrammingLanguage.CSharp, new string[] {
                        "markingtheclose",
                        "markingtheclosewindow",
                        "markingtheclosewindowsensitive"});
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
            TechTalk.SpecFlow.Table table515 = new TechTalk.SpecFlow.Table(new string[] {
                        "WindowHours",
                        "PercentageThresholdDailyVolume",
                        "PercentageThresholdWindowVolume"});
            table515.AddRow(new string[] {
                        "1",
                        "",
                        "0.1"});
#line 11
   testRunner.Given("I have the marking the close rule parameter values", ((string)(null)), table515, "Given ");
#line hidden
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Empty Universe yields no alerts")]
        public virtual void EmptyUniverseYieldsNoAlerts()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Empty Universe yields no alerts", null, ((string[])(null)));
#line 15
this.ScenarioInitialize(scenarioInfo);
            this.ScenarioStart();
#line 10
this.FeatureBackground();
#line hidden
            TechTalk.SpecFlow.Table table516 = new TechTalk.SpecFlow.Table(new string[] {
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
#line 16
   testRunner.Given("I have the orders for a universe from 01/01/2018 to 01/05/2018 :", ((string)(null)), table516, "Given ");
#line 18
         testRunner.When("I run the marking the close rule", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 19
   testRunner.Then("I will have 0 marking the close alerts", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Marking the close just out of window raises 0 alerts")]
        public virtual void MarkingTheCloseJustOutOfWindowRaises0Alerts()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Marking the close just out of window raises 0 alerts", null, ((string[])(null)));
#line 22
this.ScenarioInitialize(scenarioInfo);
            this.ScenarioStart();
#line 10
this.FeatureBackground();
#line hidden
            TechTalk.SpecFlow.Table table517 = new TechTalk.SpecFlow.Table(new string[] {
                        "SecurityName",
                        "OrderId",
                        "PlacedDate",
                        "CancelledDate",
                        "Type",
                        "Direction",
                        "Currency",
                        "LimitPrice",
                        "AverageFillPrice",
                        "OrderedVolume",
                        "FilledVolume"});
            table517.AddRow(new string[] {
                        "Barclays",
                        "1",
                        "01/01/2019 14:59:00",
                        "",
                        "Market",
                        "Buy",
                        "GBX",
                        "",
                        "",
                        "500",
                        "500"});
            table517.AddRow(new string[] {
                        "Barclays",
                        "2",
                        "01/01/2019 14:59:00",
                        "",
                        "Market",
                        "Buy",
                        "GBX",
                        "",
                        "",
                        "500",
                        "500"});
#line 23
  testRunner.Given("I have the orders for a universe from 01/01/2019 to 01/01/2019 :", ((string)(null)), table517, "Given ");
#line hidden
            TechTalk.SpecFlow.Table table518 = new TechTalk.SpecFlow.Table(new string[] {
                        "SecurityName",
                        "Epoch",
                        "Bid",
                        "Ask",
                        "Price",
                        "Currency",
                        "Volume"});
            table518.AddRow(new string[] {
                        "Barclays",
                        "01/01/2019  16:00:00",
                        "1",
                        "20",
                        "10",
                        "GBX",
                        "5000"});
            table518.AddRow(new string[] {
                        "Barclays",
                        "01/01/2019  15:55:00",
                        "1",
                        "20",
                        "10",
                        "GBX",
                        "5000"});
#line 27
    testRunner.And("With the intraday market data :", ((string)(null)), table518, "And ");
#line 31
   testRunner.When("I run the marking the close rule", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 32
   testRunner.Then("I will have 0 marking the close alerts", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Marking the close on the window raises 0 alerts")]
        public virtual void MarkingTheCloseOnTheWindowRaises0Alerts()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Marking the close on the window raises 0 alerts", null, ((string[])(null)));
#line 34
this.ScenarioInitialize(scenarioInfo);
            this.ScenarioStart();
#line 10
this.FeatureBackground();
#line hidden
            TechTalk.SpecFlow.Table table519 = new TechTalk.SpecFlow.Table(new string[] {
                        "SecurityName",
                        "OrderId",
                        "PlacedDate",
                        "CancelledDate",
                        "Type",
                        "Direction",
                        "Currency",
                        "LimitPrice",
                        "AverageFillPrice",
                        "OrderedVolume",
                        "FilledVolume"});
            table519.AddRow(new string[] {
                        "Barclays",
                        "1",
                        "01/01/2019 15:00:00",
                        "",
                        "Market",
                        "Buy",
                        "GBX",
                        "",
                        "",
                        "500",
                        "500"});
            table519.AddRow(new string[] {
                        "Barclays",
                        "2",
                        "01/01/2019 15:00:00",
                        "",
                        "Market",
                        "Buy",
                        "GBX",
                        "",
                        "",
                        "500",
                        "500"});
#line 35
  testRunner.Given("I have the orders for a universe from 01/01/2019 to 01/01/2019 :", ((string)(null)), table519, "Given ");
#line hidden
            TechTalk.SpecFlow.Table table520 = new TechTalk.SpecFlow.Table(new string[] {
                        "SecurityName",
                        "Epoch",
                        "Bid",
                        "Ask",
                        "Price",
                        "Currency",
                        "Volume"});
            table520.AddRow(new string[] {
                        "Barclays",
                        "01/01/2019  16:00:00",
                        "1",
                        "20",
                        "10",
                        "GBX",
                        "5000"});
            table520.AddRow(new string[] {
                        "Barclays",
                        "01/01/2019  15:55:00",
                        "1",
                        "20",
                        "10",
                        "GBX",
                        "5000"});
#line 39
    testRunner.And("With the intraday market data :", ((string)(null)), table520, "And ");
#line 43
   testRunner.When("I run the marking the close rule", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 44
   testRunner.Then("I will have 1 marking the close alerts", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Marking the close raises 1 alerts")]
        public virtual void MarkingTheCloseRaises1Alerts()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Marking the close raises 1 alerts", null, ((string[])(null)));
#line 46
this.ScenarioInitialize(scenarioInfo);
            this.ScenarioStart();
#line 10
this.FeatureBackground();
#line hidden
            TechTalk.SpecFlow.Table table521 = new TechTalk.SpecFlow.Table(new string[] {
                        "SecurityName",
                        "OrderId",
                        "PlacedDate",
                        "CancelledDate",
                        "Type",
                        "Direction",
                        "Currency",
                        "LimitPrice",
                        "AverageFillPrice",
                        "OrderedVolume",
                        "FilledVolume"});
            table521.AddRow(new string[] {
                        "Barclays",
                        "1",
                        "01/01/2019 15:35:00",
                        "",
                        "Market",
                        "Buy",
                        "GBX",
                        "",
                        "",
                        "500",
                        "500"});
            table521.AddRow(new string[] {
                        "Barclays",
                        "2",
                        "01/01/2019 15:35:00",
                        "",
                        "Market",
                        "Buy",
                        "GBX",
                        "",
                        "",
                        "500",
                        "500"});
#line 47
  testRunner.Given("I have the orders for a universe from 01/01/2019 to 01/01/2019 :", ((string)(null)), table521, "Given ");
#line hidden
            TechTalk.SpecFlow.Table table522 = new TechTalk.SpecFlow.Table(new string[] {
                        "SecurityName",
                        "Epoch",
                        "Bid",
                        "Ask",
                        "Price",
                        "Currency",
                        "Volume"});
            table522.AddRow(new string[] {
                        "Barclays",
                        "01/01/2019  16:00:00",
                        "1",
                        "20",
                        "10",
                        "GBX",
                        "5000"});
            table522.AddRow(new string[] {
                        "Barclays",
                        "01/01/2019  15:55:00",
                        "1",
                        "20",
                        "10",
                        "GBX",
                        "5000"});
#line 51
    testRunner.And("With the intraday market data :", ((string)(null)), table522, "And ");
#line 55
   testRunner.When("I run the marking the close rule", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 56
   testRunner.Then("I will have 1 marking the close alerts", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Marking the close one second before the close raises 1 alerts")]
        public virtual void MarkingTheCloseOneSecondBeforeTheCloseRaises1Alerts()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Marking the close one second before the close raises 1 alerts", null, ((string[])(null)));
#line 58
this.ScenarioInitialize(scenarioInfo);
            this.ScenarioStart();
#line 10
this.FeatureBackground();
#line hidden
            TechTalk.SpecFlow.Table table523 = new TechTalk.SpecFlow.Table(new string[] {
                        "SecurityName",
                        "OrderId",
                        "PlacedDate",
                        "CancelledDate",
                        "Type",
                        "Direction",
                        "Currency",
                        "LimitPrice",
                        "AverageFillPrice",
                        "OrderedVolume",
                        "FilledVolume"});
            table523.AddRow(new string[] {
                        "Barclays",
                        "1",
                        "01/01/2019 15:59:59",
                        "",
                        "Market",
                        "Buy",
                        "GBX",
                        "",
                        "",
                        "500",
                        "500"});
#line 59
  testRunner.Given("I have the orders for a universe from 01/01/2019 to 01/01/2019 :", ((string)(null)), table523, "Given ");
#line hidden
            TechTalk.SpecFlow.Table table524 = new TechTalk.SpecFlow.Table(new string[] {
                        "SecurityName",
                        "Epoch",
                        "Bid",
                        "Ask",
                        "Price",
                        "Currency",
                        "Volume"});
            table524.AddRow(new string[] {
                        "Barclays",
                        "01/01/2019  16:00:00",
                        "1",
                        "20",
                        "10",
                        "GBX",
                        "5000"});
#line 62
    testRunner.And("With the intraday market data :", ((string)(null)), table524, "And ");
#line 65
   testRunner.When("I run the marking the close rule", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 66
   testRunner.Then("I will have 1 marking the close alerts", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Marking the close raises 2 alerts for differnet days")]
        public virtual void MarkingTheCloseRaises2AlertsForDiffernetDays()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Marking the close raises 2 alerts for differnet days", null, ((string[])(null)));
#line 68
this.ScenarioInitialize(scenarioInfo);
            this.ScenarioStart();
#line 10
this.FeatureBackground();
#line hidden
            TechTalk.SpecFlow.Table table525 = new TechTalk.SpecFlow.Table(new string[] {
                        "SecurityName",
                        "OrderId",
                        "PlacedDate",
                        "CancelledDate",
                        "Type",
                        "Direction",
                        "Currency",
                        "LimitPrice",
                        "AverageFillPrice",
                        "OrderedVolume",
                        "FilledVolume"});
            table525.AddRow(new string[] {
                        "Barclays",
                        "1",
                        "01/01/2019 15:35:00",
                        "",
                        "Market",
                        "Buy",
                        "GBX",
                        "",
                        "",
                        "500",
                        "500"});
            table525.AddRow(new string[] {
                        "Barclays",
                        "2",
                        "01/01/2019 15:35:00",
                        "",
                        "Market",
                        "Buy",
                        "GBX",
                        "",
                        "",
                        "500",
                        "500"});
            table525.AddRow(new string[] {
                        "Barclays",
                        "3",
                        "01/02/2019 15:35:00",
                        "",
                        "Market",
                        "Buy",
                        "GBX",
                        "",
                        "",
                        "500",
                        "500"});
            table525.AddRow(new string[] {
                        "Barclays",
                        "4",
                        "01/02/2019 15:35:00",
                        "",
                        "Market",
                        "Buy",
                        "GBX",
                        "",
                        "",
                        "500",
                        "500"});
#line 69
  testRunner.Given("I have the orders for a universe from 01/01/2019 to 01/02/2019 :", ((string)(null)), table525, "Given ");
#line hidden
            TechTalk.SpecFlow.Table table526 = new TechTalk.SpecFlow.Table(new string[] {
                        "SecurityName",
                        "Epoch",
                        "Bid",
                        "Ask",
                        "Price",
                        "Currency",
                        "Volume"});
            table526.AddRow(new string[] {
                        "Barclays",
                        "01/01/2019  16:00:00",
                        "1",
                        "20",
                        "10",
                        "GBX",
                        "5000"});
            table526.AddRow(new string[] {
                        "Barclays",
                        "01/01/2019  15:55:00",
                        "1",
                        "20",
                        "10",
                        "GBX",
                        "5000"});
            table526.AddRow(new string[] {
                        "Barclays",
                        "01/02/2019  16:00:00",
                        "1",
                        "20",
                        "10",
                        "GBX",
                        "5000"});
            table526.AddRow(new string[] {
                        "Barclays",
                        "01/02/2019  15:55:00",
                        "1",
                        "20",
                        "10",
                        "GBX",
                        "5000"});
#line 75
    testRunner.And("With the intraday market data :", ((string)(null)), table526, "And ");
#line 81
   testRunner.When("I run the marking the close rule", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 82
   testRunner.Then("I will have 2 marking the close alerts", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Marking the close raises 0 alerts for differnet days")]
        public virtual void MarkingTheCloseRaises0AlertsForDiffernetDays()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Marking the close raises 0 alerts for differnet days", null, ((string[])(null)));
#line 84
this.ScenarioInitialize(scenarioInfo);
            this.ScenarioStart();
#line 10
this.FeatureBackground();
#line hidden
            TechTalk.SpecFlow.Table table527 = new TechTalk.SpecFlow.Table(new string[] {
                        "SecurityName",
                        "OrderId",
                        "PlacedDate",
                        "CancelledDate",
                        "Type",
                        "Direction",
                        "Currency",
                        "LimitPrice",
                        "AverageFillPrice",
                        "OrderedVolume",
                        "FilledVolume"});
            table527.AddRow(new string[] {
                        "Barclays",
                        "1",
                        "01/01/2019 15:35:00",
                        "",
                        "Market",
                        "Buy",
                        "GBX",
                        "",
                        "",
                        "50",
                        "50"});
            table527.AddRow(new string[] {
                        "Barclays",
                        "2",
                        "01/01/2019 15:35:00",
                        "",
                        "Market",
                        "Buy",
                        "GBX",
                        "",
                        "",
                        "50",
                        "50"});
            table527.AddRow(new string[] {
                        "Barclays",
                        "3",
                        "01/02/2019 15:35:00",
                        "",
                        "Market",
                        "Buy",
                        "GBX",
                        "",
                        "",
                        "50",
                        "50"});
            table527.AddRow(new string[] {
                        "Barclays",
                        "4",
                        "01/02/2019 15:35:00",
                        "",
                        "Market",
                        "Buy",
                        "GBX",
                        "",
                        "",
                        "50",
                        "50"});
#line 85
  testRunner.Given("I have the orders for a universe from 01/01/2019 to 01/02/2019 :", ((string)(null)), table527, "Given ");
#line hidden
            TechTalk.SpecFlow.Table table528 = new TechTalk.SpecFlow.Table(new string[] {
                        "SecurityName",
                        "Epoch",
                        "Bid",
                        "Ask",
                        "Price",
                        "Currency",
                        "Volume"});
            table528.AddRow(new string[] {
                        "Barclays",
                        "01/01/2019  16:00:00",
                        "1",
                        "20",
                        "10",
                        "GBX",
                        "5000"});
            table528.AddRow(new string[] {
                        "Barclays",
                        "01/01/2019  15:55:00",
                        "1",
                        "20",
                        "10",
                        "GBX",
                        "5000"});
            table528.AddRow(new string[] {
                        "Barclays",
                        "01/02/2019  16:00:00",
                        "1",
                        "20",
                        "10",
                        "GBX",
                        "5000"});
            table528.AddRow(new string[] {
                        "Barclays",
                        "01/02/2019  15:55:00",
                        "1",
                        "20",
                        "10",
                        "GBX",
                        "5000"});
#line 91
    testRunner.And("With the intraday market data :", ((string)(null)), table528, "And ");
#line 97
   testRunner.When("I run the marking the close rule", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 98
   testRunner.Then("I will have 0 marking the close alerts", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            this.ScenarioCleanup();
        }
    }
}
#pragma warning restore
#endregion
