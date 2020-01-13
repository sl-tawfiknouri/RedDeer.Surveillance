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
    public class HighVolumeSteps
    {
        private readonly ScenarioContext _context;
        private readonly RuleRunner _ruleRunner;

        public HighVolumeSteps(ScenarioContext context, RuleRunner ruleRunner)
        {
            _context = context;
            _ruleRunner = ruleRunner;
        }

        [Given(@"the high volume core settings")]
        public void GivenTheHighVolumeCoreSettings(Table table)
        {
            var config = table.Rows.First();

            var dto = new HighVolumeRuleParameterDto
            {
                Id = RuleRunner.RuleId,
                OrganisationalFactors = new[] { OrganisationalFactors.None }
            };

            var volumeType = config.ValueOrNull("VolumeType");
            if (volumeType == "Daily")
            {
                dto.WindowSize = RuleParameterHelpers.ToWindowSize("1 day");
                dto.HighVolumePercentageDaily = Convert.ToDecimal(config.ValueOrNull("VolumePercentage")) / 100;
            }

            _ruleRunner.RuleParameterDto.HighVolumes = dto.CreateArray();
            _ruleRunner.RuleType = Rules.HighVolume;
        }
    }
}
