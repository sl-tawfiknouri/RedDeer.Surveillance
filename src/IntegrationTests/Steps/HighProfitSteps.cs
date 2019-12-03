using RedDeer.Contracts.SurveillanceService.Api.RuleParameter;
using RedDeer.Contracts.SurveillanceService.Api.RuleParameter.Equities;
using RedDeer.Contracts.SurveillanceService.Rules;
using RedDeer.Surveillance.IntegrationTests.Runner;
using RedDeer.Surveillance.IntegrationTests.Steps.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TechTalk.SpecFlow;

namespace RedDeer.Surveillance.IntegrationTests.Steps
{
    [Binding]
    public sealed class HighProfitSteps
    {
        private readonly ScenarioContext _context;
        private readonly RuleRunner _ruleRunner;

        public HighProfitSteps(ScenarioContext context, RuleRunner ruleRunner)
        {
            _context = context;
            _ruleRunner = ruleRunner;
        }

        [Given(@"the high profit core settings")]
        public void GivenTheHighProfitCoreSettings(Table table)
        {
            var config = table.Rows.First();

            var dto = new HighProfitsRuleParameterDto
            {
                Id = RuleRunner.RuleId,
                OrganisationalFactors = new[] { OrganisationalFactors.None }
            };

            dto.HighProfitPercentageThreshold = Convert.ToDecimal(config.ValueOrNull("ProfitThresholdPercent")) / 100;

            switch (config.ValueOrNull("PriceType"))
            {
                case "Close":
                    dto.PerformHighProfitDailyAnalysis = true;
                    break;
                case "Spot":
                    dto.PerformHighProfitWindowAnalysis = true;
                    break;
            }

            dto.WindowSize = RuleParameterHelpers.ToWindowSize(config.ValueOrNull("ForwardWindow"));
            dto.ForwardWindow = RuleParameterHelpers.ToWindowSize(config.ValueOrNull("BackwardWindow"));

            _ruleRunner.RuleParameterDto.HighProfits = dto.CreateArray();
            _ruleRunner.RuleType = Rules.HighProfits;
        }

    }
}
