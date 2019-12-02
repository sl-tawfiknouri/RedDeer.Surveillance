using RedDeer.Surveillance.IntegrationTests.Runner;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using TechTalk.SpecFlow;

namespace RedDeer.Surveillance.IntegrationTests.Steps
{
    [Binding]
    public sealed class RuleSteps
    {
        private readonly ScenarioContext _context;
        private readonly RuleRunner _ruleRunner;

        public RuleSteps(ScenarioContext context, RuleRunner ruleRunner)
        {
            _context = context;
            _ruleRunner = ruleRunner;
        }

        [When(@"the rule is run between ""(.*)"" and ""(.*)""")]
        public void WhenTheRuleIsRun(string from, string to)
        {
            _ruleRunner.From = DateTime.Parse(from, null, DateTimeStyles.AssumeUniversal);
            _ruleRunner.To = DateTime.Parse(to, null, DateTimeStyles.AssumeUniversal).AddDays(1).AddMilliseconds(-1);

            _ruleRunner.Run().Wait();
        }

        [Then(@"there should be no breaches")]
        public void ThenThereShouldBeNoBreaches()
        {
            if (_ruleRunner.OriginalRuleBreaches == null)
            {
                throw new Exception("Checking for no rule breaches, but the data has not been fetched");
            }

            if (_ruleRunner.OriginalRuleBreaches.Count > 0)
            {
                throw new Exception($"Expecting no rule breaches, but {_ruleRunner.OriginalRuleBreaches?.Count} breaches were found");
            }

            _ruleRunner.HasCheckedForNoBreaches = true;
        }


        [Then(@"there should be a breach with order ids ""(.*)""")]
        public void ThenThereShouldBeABreachWithOrderIds(string idString)
        {
            if (!_ruleRunner.RemainingRuleBreaches?.Any() ?? true)
            {
                throw new Exception($"Trying to check for rule breach with ids \"{idString}\" but there are no rule breaches remaining");
            }

            var orderIds = idString
                .Split(",", StringSplitOptions.RemoveEmptyEntries)
                .Select(x => x.Trim())
                .ToList();

            var breach = _ruleRunner.RemainingRuleBreaches
                .Where(x => x.Orders.Count == orderIds.Count)
                .FirstOrDefault(x => orderIds.All(y => x.Orders.Any(z => z.ClientOrderId == y)));

            if (breach == null)
            {
                throw new Exception($"No rule breach exists with matching ids \"{idString}\"");
            }

            _ruleRunner.RemainingRuleBreaches.Remove(breach);
        }

    }
}
