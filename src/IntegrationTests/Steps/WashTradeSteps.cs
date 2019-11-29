﻿using RedDeer.Contracts.SurveillanceService.Api.RuleParameter;
using RedDeer.Contracts.SurveillanceService.Api.RuleParameter.Equities;
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
            var config = table.Rows.First();

            var dto = new WashTradeRuleParameterDto
            {
                Id = RuleRunner.RuleId,
                OrganisationalFactors = new[] { OrganisationalFactors.None }
            };

            dto.WindowSize = RuleParameterHelpers.ToWindowSize(config.ValueOrNull("TimeWindow"));

            var analysePositionsBy = config.ValueOrNull("AnalysePositionsBy");
            if (analysePositionsBy == "Clustering")
            {
                dto.PerformClusteringPositionAnalysis = true;
                dto.ClusteringPositionMinimumNumberOfTrades = Convert.ToInt32(config.ValueOrNull("MinNumberOfTrades"));
                dto.ClusteringPercentageValueDifferenceThreshold = Convert.ToDecimal(config.ValueOrNull("MaxValueChangePercent")) / 100;
            }

            _ruleRunner.RuleParameterDto.WashTrades = dto.CreateArray();
        }
    }
}
