using RedDeer.Contracts.SurveillanceService.Api.RuleParameter;
using RedDeer.Surveillance.IntegrationTests.Runner;
using RedDeer.Surveillance.IntegrationTests.Steps.Common;
using System;
using System.Linq;
using RedDeer.Contracts.SurveillanceService.Api.RuleParameter.FixedIncome;
using RedDeer.Contracts.SurveillanceService.Rules;
using TechTalk.SpecFlow;

namespace RedDeer.Surveillance.IntegrationTests.Steps
{
    [Binding]
    public sealed class FixedIncomeHighProfitSteps
    {
        private readonly ScenarioContext _context;
        private readonly RuleRunner _ruleRunner;

        public FixedIncomeHighProfitSteps(ScenarioContext context, RuleRunner ruleRunner)
        {
            _context = context;
            _ruleRunner = ruleRunner;
        }

        [Given(@"the fixed income high profit core settings")]
        public void GivenTheHighProfitCoreSettings(Table table)
        {
            var config = table.Rows.First();

            var dto = new FixedIncomeHighProfitRuleParameterDto
            {
                Id = RuleRunner.RuleId, 
                OrganisationalFactors = new[] {OrganisationalFactors.None}, 
                HighProfitPercentageThreshold = Convert.ToDecimal(config.ValueOrNull("ProfitThresholdPercent")) / 100
            };
            
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

            _ruleRunner.RuleParameterDto.FixedIncomeHighProfits = dto.CreateArray();
            _ruleRunner.RuleType = Rules.FixedIncomeHighProfits;
        }

    }
}
