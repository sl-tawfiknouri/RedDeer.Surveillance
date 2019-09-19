namespace Surveillance.Engine.Rules.RuleParameters.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Domain.Surveillance.Scheduling;

    using Microsoft.Extensions.Logging;

    using Newtonsoft.Json;

    using RedDeer.Contracts.SurveillanceService.Api.RuleParameter;

    using Surveillance.Engine.Rules.RuleParameters.Services.Interfaces;
    using Surveillance.Reddeer.ApiClient.RuleParameter.Interfaces;

    public class RuleParameterService : IRuleParameterService
    {
        private readonly ILogger<RuleParameterService> _logger;

        private readonly IRuleParameterApi _ruleParameterApiRepository;

        public RuleParameterService(IRuleParameterApi ruleParameterApiRepository, ILogger<RuleParameterService> logger)
        {
            this._ruleParameterApiRepository = ruleParameterApiRepository
                                               ?? throw new ArgumentNullException(nameof(ruleParameterApiRepository));
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<RuleParameterDto> RuleParameters(ScheduledExecution execution)
        {
            this._logger.LogInformation("fetching rule parameters");

            if (!execution.IsBackTest)
            {
                this._logger.LogInformation("Subscribe Rules noted not a back test run. Fetching all dtos.");
                return await this._ruleParameterApiRepository.GetAsync();
            }

            var executionJson = JsonConvert.SerializeObject(execution);
            this._logger.LogInformation($"about to select rule ids from {executionJson}");
            var ids = execution.Rules.SelectMany(ru => ru.Ids).ToList();

            var ruleDtos = new List<RuleParameterDto>();
            foreach (var id in ids)
            {
                this._logger.LogInformation($"Subscribe Rules fetching rule dto for {id}");
                var apiResult = await this._ruleParameterApiRepository.GetAsync(id);

                if (apiResult != null) ruleDtos.Add(apiResult);
                else this._logger.LogError($"Subscribe Rules fetching rule dto for {id} returned null from api");
            }

            if (!ruleDtos.Any())
            {
                this._logger.LogError("Subscribe Rules did not find any matching rule dtos");
                return new RuleParameterDto();
            }

            if (ruleDtos.Count != ids.Count)
                this._logger.LogError("Subscribe Rules did not finding a matching amount of ids to rule dtos");

            if (ruleDtos.Count == 1) return ruleDtos.First();

            var allCancelledOrders = ruleDtos.SelectMany(_ => _.CancelledOrders).ToArray();
            var allHighProfits = ruleDtos.SelectMany(_ => _.HighProfits).ToArray();
            var allMarkingTheClose = ruleDtos.SelectMany(_ => _.MarkingTheCloses).ToArray();
            var allSpoofings = ruleDtos.SelectMany(_ => _.Spoofings).ToArray();
            var allLayerings = ruleDtos.SelectMany(_ => _.Layerings).ToArray();
            var allHighVolumes = ruleDtos.SelectMany(_ => _.HighVolumes).ToArray();
            var allWashTrades = ruleDtos.SelectMany(_ => _.WashTrades).ToArray();
            var allRampings = ruleDtos.SelectMany(_ => _.Rampings).ToArray();
            var allPlacingOrders = ruleDtos.SelectMany(_ => _.PlacingOrders).ToArray();

            this._logger.LogInformation("has fetched the rule parameters");

            return new RuleParameterDto
           {
               CancelledOrders = allCancelledOrders,
               HighProfits = allHighProfits,
               MarkingTheCloses = allMarkingTheClose,
               Spoofings = allSpoofings,
               Layerings = allLayerings,
               HighVolumes = allHighVolumes,
               WashTrades = allWashTrades,
               Rampings = allRampings,
               PlacingOrders = allPlacingOrders
           };
        }
    }
}