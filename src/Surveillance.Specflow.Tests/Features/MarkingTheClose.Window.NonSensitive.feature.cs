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
    [NUnit.Framework.DescriptionAttribute("MarkingTheClose Window Non Sensitive Parameters")]
    [NUnit.Framework.CategoryAttribute("markingtheclose")]
    [NUnit.Framework.CategoryAttribute("markingtheclosewindow")]
    [NUnit.Framework.CategoryAttribute("markingtheclosewindownonsensitive")]
    public partial class MarkingTheCloseWindowNonSensitiveParametersFeature
    {
        
        private TechTalk.SpecFlow.ITestRunner testRunner;
        
#line 1 "MarkingTheClose.Window.NonSensitive.feature"
#line hidden
        
        [NUnit.Framework.OneTimeSetUpAttribute()]
        public virtual void FeatureSetup()
        {
            testRunner = TechTalk.SpecFlow.TestRunnerManager.GetTestRunner();
            TechTalk.SpecFlow.FeatureInfo featureInfo = new TechTalk.SpecFlow.FeatureInfo(new System.Globalization.CultureInfo("en-US"), "MarkingTheClose Window Non Sensitive Parameters", "\tIn order to meet MAR compliance requirements\r\n\tI need to be able to detect when " +
                    "traders are executing trades\r\n\ttowards the market closure time at an unusually\r\n" +
                    "\thigh volume in order to extract supernormal profits", ProgrammingLanguage.CSharp, new string[] {
                        "markingtheclose",
                        "markingtheclosewindow",
                        "markingtheclosewindownonsensitive"});
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
            TechTalk.SpecFlow.Table table501 = new TechTalk.SpecFlow.Table(new string[] {
                        "WindowHours",
                        "PercentageThresholdDailyVolume",
                        "PercentageThresholdWindowVolume"});
            table501.AddRow(new string[] {
                        "1",
                        "",
                        "0.5"});
#line 11
  testRunner.Given("I have the marking the close rule parameter values", ((string)(null)), table501, "Given ");
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
            TechTalk.SpecFlow.Table table502 = new TechTalk.SpecFlow.Table(new string[] {
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
   testRunner.Given("I have the orders for a universe from 01/01/2018 to 01/05/2018 :", ((string)(null)), table502, "Given ");
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
            TechTalk.SpecFlow.Table table503 = new TechTalk.SpecFlow.Table(new string[] {
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
            table503.AddRow(new string[] {
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
            table503.AddRow(new string[] {
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
  testRunner.Given("I have the orders for a universe from 01/01/2019 to 01/01/2019 :", ((string)(null)), table503, "Given ");
#line hidden
            TechTalk.SpecFlow.Table table504 = new TechTalk.SpecFlow.Table(new string[] {
                        "SecurityName",
                        "Epoch",
                        "Bid",
                        "Ask",
                        "Price",
                        "Currency",
                        "Volume"});
            table504.AddRow(new string[] {
                        "Barclays",
                        "01/01/2019  16:00:00",
                        "1",
                        "20",
                        "10",
                        "GBX",
                        "5000"});
            table504.AddRow(new string[] {
                        "Barclays",
                        "01/01/2019  15:55:00",
                        "1",
                        "20",
                        "10",
                        "GBX",
                        "5000"});
#line 27
    testRunner.And("With the intraday market data :", ((string)(null)), table504, "And ");
#line 31
   testRunner.When("I run the marking the close rule", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 32
   testRunner.Then("I will have 0 marking the close alerts", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Marking the close raises 1 alerts")]
        public virtual void MarkingTheCloseRaises1Alerts()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Marking the close raises 1 alerts", null, ((string[])(null)));
#line 34
this.ScenarioInitialize(scenarioInfo);
            this.ScenarioStart();
#line 10
this.FeatureBackground();
#line hidden
            TechTalk.SpecFlow.Table table505 = new TechTalk.SpecFlow.Table(new string[] {
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
            table505.AddRow(new string[] {
                        "Barclays",
                        "1",
                        "01/01/2019 15:35:00",
                        "",
                        "Market",
                        "Buy",
                        "GBX",
                        "",
                        "",
                        "2500",
                        "2500"});
            table505.AddRow(new string[] {
                        "Barclays",
                        "2",
                        "01/01/2019 15:35:00",
                        "",
                        "Market",
                        "Buy",
                        "GBX",
                        "",
                        "",
                        "2500",
                        "2500"});
#line 35
  testRunner.Given("I have the orders for a universe from 01/01/2019 to 01/01/2019 :", ((string)(null)), table505, "Given ");
#line hidden
            TechTalk.SpecFlow.Table table506 = new TechTalk.SpecFlow.Table(new string[] {
                        "SecurityName",
                        "Epoch",
                        "Bid",
                        "Ask",
                        "Price",
                        "Currency",
                        "Volume"});
            table506.AddRow(new string[] {
                        "Barclays",
                        "01/01/2019  16:00:00",
                        "1",
                        "20",
                        "10",
                        "GBX",
                        "5000"});
            table506.AddRow(new string[] {
                        "Barclays",
                        "01/01/2019  15:55:00",
                        "1",
                        "20",
                        "10",
                        "GBX",
                        "5000"});
#line 39
    testRunner.And("With the intraday market data :", ((string)(null)), table506, "And ");
#line 43
   testRunner.When("I run the marking the close rule", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 44
   testRunner.Then("I will have 1 marking the close alerts", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Marking the close raises 2 alerts for differnet days")]
        public virtual void MarkingTheCloseRaises2AlertsForDiffernetDays()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Marking the close raises 2 alerts for differnet days", null, ((string[])(null)));
#line 46
this.ScenarioInitialize(scenarioInfo);
            this.ScenarioStart();
#line 10
this.FeatureBackground();
#line hidden
            TechTalk.SpecFlow.Table table507 = new TechTalk.SpecFlow.Table(new string[] {
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
            table507.AddRow(new string[] {
                        "Barclays",
                        "1",
                        "01/01/2019 15:35:00",
                        "",
                        "Market",
                        "Buy",
                        "GBX",
                        "",
                        "",
                        "2500",
                        "2500"});
            table507.AddRow(new string[] {
                        "Barclays",
                        "2",
                        "01/01/2019 15:35:00",
                        "",
                        "Market",
                        "Buy",
                        "GBX",
                        "",
                        "",
                        "2500",
                        "2500"});
            table507.AddRow(new string[] {
                        "Barclays",
                        "3",
                        "01/02/2019 15:35:00",
                        "",
                        "Market",
                        "Buy",
                        "GBX",
                        "",
                        "",
                        "2500",
                        "2500"});
            table507.AddRow(new string[] {
                        "Barclays",
                        "4",
                        "01/02/2019 15:35:00",
                        "",
                        "Market",
                        "Buy",
                        "GBX",
                        "",
                        "",
                        "2500",
                        "2500"});
#line 47
  testRunner.Given("I have the orders for a universe from 01/01/2019 to 01/02/2019 :", ((string)(null)), table507, "Given ");
#line hidden
            TechTalk.SpecFlow.Table table508 = new TechTalk.SpecFlow.Table(new string[] {
                        "SecurityName",
                        "Epoch",
                        "Bid",
                        "Ask",
                        "Price",
                        "Currency",
                        "Volume"});
            table508.AddRow(new string[] {
                        "Barclays",
                        "01/01/2019  16:00:00",
                        "1",
                        "20",
                        "10",
                        "GBX",
                        "5000"});
            table508.AddRow(new string[] {
                        "Barclays",
                        "01/01/2019  15:55:00",
                        "1",
                        "20",
                        "10",
                        "GBX",
                        "5000"});
            table508.AddRow(new string[] {
                        "Barclays",
                        "01/02/2019  16:00:00",
                        "1",
                        "20",
                        "10",
                        "GBX",
                        "5000"});
            table508.AddRow(new string[] {
                        "Barclays",
                        "01/02/2019  15:55:00",
                        "1",
                        "20",
                        "10",
                        "GBX",
                        "5000"});
#line 53
    testRunner.And("With the intraday market data :", ((string)(null)), table508, "And ");
#line 59
   testRunner.When("I run the marking the close rule", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 60
   testRunner.Then("I will have 2 marking the close alerts", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Marking the close raises 0 alerts for differnet days")]
        public virtual void MarkingTheCloseRaises0AlertsForDiffernetDays()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Marking the close raises 0 alerts for differnet days", null, ((string[])(null)));
#line 62
this.ScenarioInitialize(scenarioInfo);
            this.ScenarioStart();
#line 10
this.FeatureBackground();
#line hidden
            TechTalk.SpecFlow.Table table509 = new TechTalk.SpecFlow.Table(new string[] {
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
            table509.AddRow(new string[] {
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
            table509.AddRow(new string[] {
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
            table509.AddRow(new string[] {
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
            table509.AddRow(new string[] {
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
#line 63
  testRunner.Given("I have the orders for a universe from 01/01/2019 to 01/02/2019 :", ((string)(null)), table509, "Given ");
#line hidden
            TechTalk.SpecFlow.Table table510 = new TechTalk.SpecFlow.Table(new string[] {
                        "SecurityName",
                        "Epoch",
                        "Bid",
                        "Ask",
                        "Price",
                        "Currency",
                        "Volume"});
            table510.AddRow(new string[] {
                        "Barclays",
                        "01/01/2019  16:00:00",
                        "1",
                        "20",
                        "10",
                        "GBX",
                        "5000"});
            table510.AddRow(new string[] {
                        "Barclays",
                        "01/01/2019  15:55:00",
                        "1",
                        "20",
                        "10",
                        "GBX",
                        "5000"});
            table510.AddRow(new string[] {
                        "Barclays",
                        "01/02/2019  16:00:00",
                        "1",
                        "20",
                        "10",
                        "GBX",
                        "5000"});
            table510.AddRow(new string[] {
                        "Barclays",
                        "01/02/2019  15:55:00",
                        "1",
                        "20",
                        "10",
                        "GBX",
                        "5000"});
#line 69
    testRunner.And("With the intraday market data :", ((string)(null)), table510, "And ");
#line 75
   testRunner.When("I run the marking the close rule", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 76
   testRunner.Then("I will have 0 marking the close alerts", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            this.ScenarioCleanup();
        }
    }
}
#pragma warning restore
#endregion