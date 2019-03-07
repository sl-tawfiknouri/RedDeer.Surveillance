using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Surveillance.Scheduling;
using Microsoft.Extensions.Logging;
using RedDeer.Contracts.SurveillanceService.Api.RuleParameter;
using Surveillance.DataLayer.Api.RuleParameter.Interfaces;
using Surveillance.Engine.Rules.RuleParameters.Manager.Interfaces;

namespace Surveillance.Engine.Rules.RuleParameters.Manager
{
    public class RuleParameterManager : IRuleParameterManager
    {
        private readonly IRuleParameterApiRepository _ruleParameterApiRepository;
        private readonly ILogger<RuleParameterManager> _logger;

        public RuleParameterManager(
            IRuleParameterApiRepository ruleParameterApiRepository,
            ILogger<RuleParameterManager> logger)
        {
            _ruleParameterApiRepository = ruleParameterApiRepository ?? throw new ArgumentNullException(nameof(ruleParameterApiRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<RuleParameterDto> RuleParameters(ScheduledExecution execution)
        {
            _logger.LogInformation($"RuleParameterManager fetching rule parameters");

            if (!execution.IsBackTest)
            {
                _logger.LogInformation($"UniverseRuleSubscriber Subscribe Rules noted not a back test run. Fetching all dtos.");
                return await _ruleParameterApiRepository.Get();
            }

            var ids = execution.Rules.SelectMany(ru => ru.Ids).ToList();

            var ruleDtos = new List<RuleParameterDto>();
            foreach (var id in ids)
            {
                _logger.LogInformation($"UniverseRuleSubscriber Subscribe Rules fetching rule dto for {id}");
                var apiResult = await _ruleParameterApiRepository.Get(id);

                if (apiResult != null)
                    ruleDtos.Add(apiResult);
            }

            if (!ruleDtos.Any())
            {
                _logger.LogError($"UniverseRuleSubscriber Subscribe Rules did not find any matching rule dtos");
                return new RuleParameterDto();
            }

            if (ruleDtos.Count != ids.Count)
            {
                _logger.LogError($"UniverseRuleSubscriber Subscribe Rules did not finding a matching amount of ids to rule dtos");
            }

            if (ruleDtos.Count == 1)
            {
                return ruleDtos.First();
            }

            var allCancelledOrders = ruleDtos.SelectMany(ru => ru.CancelledOrders).ToArray();
            var allHighProfits = ruleDtos.SelectMany(ru => ru.HighProfits).ToArray();
            var allMarkingTheClose = ruleDtos.SelectMany(ru => ru.MarkingTheCloses).ToArray();
            var allSpoofings = ruleDtos.SelectMany(ru => ru.Spoofings).ToArray();
            var allLayerings = ruleDtos.SelectMany(ru => ru.Layerings).ToArray();
            var allHighVolumes = ruleDtos.SelectMany(ru => ru.HighVolumes).ToArray();
            var allWashTrades = ruleDtos.SelectMany(ru => ru.WashTrades).ToArray();

            _logger.LogInformation($"RuleParameterManager has fetched the rule parameters");

            return new RuleParameterDto
            {
                CancelledOrders = allCancelledOrders,
                HighProfits = allHighProfits,
                MarkingTheCloses = allMarkingTheClose,
                Spoofings = allSpoofings,
                Layerings = allLayerings,
                HighVolumes = allHighVolumes,
                WashTrades = allWashTrades
            };
        }
    }
}
