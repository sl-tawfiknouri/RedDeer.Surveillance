using RedDeer.Contracts.SurveillanceService.Api.RuleParameter;
using RedDeer.Contracts.SurveillanceService.Api.RuleParameter.Equities;
using RedDeer.Surveillance.IntegrationTests.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TechTalk.SpecFlow;

namespace RedDeer.Surveillance.IntegrationTests.Steps
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
            var dto = new WashTradeRuleParameterDto
            {
                Id = RuleRunner.RuleId,
                OrganisationalFactors = new[] { OrganisationalFactors.None }
            };

            var config = table.Rows.First();

            var timeWindow = config.ValueOrNull("TimeWindow");
            if (timeWindow != null)
            {
                var split = timeWindow.Split(" ");
                if (split.Length == 2)
                {
                    var value = split[0];
                    var type = split[1];
                    if (type == "day" || type == "days")
                    {
                        dto.WindowSize = TimeSpan.FromDays(Convert.ToInt32(split[0]));
                    }
                }
            }

            var analysePositionsBy = config.ValueOrNull("AnalysePositionsBy");
            if (analysePositionsBy == "Clustering")
            {
                dto.PerformClusteringPositionAnalysis = true;
                dto.ClusteringPositionMinimumNumberOfTrades = Convert.ToInt32(config.ValueOrNull("MinNumberOfTrades"));
                dto.ClusteringPercentageValueDifferenceThreshold = Convert.ToDecimal(config.ValueOrNull("MaxValueChangePercent")) / 100;
            }

            _ruleRunner.WashTradeParameters = dto;
        }
    }
}
