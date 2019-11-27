using RedDeer.Contracts.SurveillanceService.Api.RuleParameter;
using RedDeer.Contracts.SurveillanceService.Api.RuleParameter.Equities;
using RedDeer.Surveillance.IntegrationTests.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TechTalk.SpecFlow;

namespace RedDeer.Surveillance.IntegrationTests.Steps.WashTrade
{
    [Binding]
    public sealed class WashTradeSteps
    {
        private readonly ScenarioContext _context;
        private readonly RuleRunner _ruleRunner;

        public WashTradeSteps(ScenarioContext context, RuleRunner ruleRunner)
        {
            _context = context;
            _ruleRunner = ruleRunner;
        }

        [Given(@"the wash trade core settings")]
        public void GivenTheWashTradeCoreSettings(Table table)
        {
            _ruleRunner.WashTradeParameters = new WashTradeRuleParameterDto
            {
                Id = "rule123",
                WindowSize = TimeSpan.FromDays(1),
                PerformClusteringPositionAnalysis = true,
                ClusteringPercentageValueDifferenceThreshold = 0.010M,
                ClusteringPositionMinimumNumberOfTrades = 2,
                OrganisationalFactors = new[] { OrganisationalFactors.None }
            };
        }
    }
}
