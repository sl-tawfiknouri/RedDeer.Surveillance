using BoDi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TechTalk.SpecFlow;

namespace RedDeer.Surveillance.IntegrationTests.Runner
{
    [Binding]
    public sealed class Hooks
    {
        private readonly IObjectContainer _objectContainer;

        private RuleRunner _ruleRunner;

        public Hooks(IObjectContainer objectContainer)
        {
            _objectContainer = objectContainer;
        }

        [BeforeScenario]
        public void BeforeScenario()
        {
            _ruleRunner = new RuleRunner();
            _objectContainer.RegisterInstanceAs(_ruleRunner);

            _ruleRunner.Setup().Wait();
        }

        [AfterScenario]
        public void AfterScenario()
        {
            _ruleRunner?.ErrorOnUnaccountedRuleBreaches();
            _ruleRunner?.ErrorIfUncheckedForNoBreaches();
        }
    }
}
