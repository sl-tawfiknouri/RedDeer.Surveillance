using Surveillance.RuleParameters;
using Surveillance.RuleParameters.OrganisationalFactors;
using TechTalk.SpecFlow;

namespace Surveillance.Specflow.Tests.StepDefinitions
{
    [Binding]
    public class WashTradeSteps : BaseUniverseSteps
    {
        private readonly ScenarioContext _scenarioContext;
        private WashTradeRuleParameters _washTradeRuleParameters;

        public WashTradeSteps(ScenarioContext scenarioContext)
        {
            _scenarioContext = scenarioContext;
        }

        [Given(@"I have the wash trade rule average netting parameter values:")]
        public void GivenIHaveTheWashTradeRuleParameterValues(Table ruleParameters)
        {
            if (ruleParameters.RowCount != 1)
            {
                _scenarioContext.Pending();
                return;
            }

            ruleParameters.Rows[0].TryGetValue("window hours", out string windowHours);
            ruleParameters.Rows[0].TryGetValue("minimum number of trades", out string numberOfTrades);
            ruleParameters.Rows[0].TryGetValue("maximum position value change", out string maxPositionChangeValue);
            ruleParameters.Rows[0].TryGetValue("maximum absolute value change", out string maxAbsoluteValueChange);
            ruleParameters.Rows[0].TryGetValue("maximum absolute value change currency", out string maxAbsoluteValueChangeCurrency);

            if (!int.TryParse(windowHours, out var wh))
            {
                _scenarioContext.Pending();
                return;
            }

            if (!int.TryParse(numberOfTrades, out var not))
            {
                _scenarioContext.Pending();
                return;
            }

            if (!decimal.TryParse(maxPositionChangeValue, out var mpcv))
            {
                _scenarioContext.Pending();
                return;
            }

            if (!int.TryParse(maxAbsoluteValueChange, out var mavc))
            {
                _scenarioContext.Pending();
                return;
            }

            _washTradeRuleParameters =
                new WashTradeRuleParameters(
                    "0",
                    new System.TimeSpan(wh, 0, 0),
                    true,
                    false,
                    false,
                    not,
                    mpcv,
                    mavc,
                    maxAbsoluteValueChangeCurrency,
                    null,
                    null,
                    null,
                    null,
                    null,
                    null,
                    null,
                    new[] { ClientOrganisationalFactors.None },
                    true);
        }

        [When(@"I run the wash trade rule")]
        public void WhenIRunTheWashTradeRule()
        {

        }

        [Then(@"I will have (.*) wash trade alerts")]
        public void ThenIWillHaveAlerts(int p0)
        {
            var r = p0;
        }
    }
}
