using TechTalk.SpecFlow;

namespace Surveillance.Specflow.Tests.StepDefinitions
{
    [Binding]
    public class WashTradeSteps : BaseUniverseSteps
    {
        private readonly ScenarioContext _scenarioContext;


        public WashTradeSteps(ScenarioContext scenarioContext)
        {
            _scenarioContext = scenarioContext;
        }

        [Given(@"I have the wash trade rule parameter values")]
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

        }
    }
}
