using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DomainV2.Scheduling;
using Microsoft.Extensions.Logging;
using RedDeer.Contracts.SurveillanceService.Api.RuleParameter;
using Surveillance.DataLayer.Api.RuleParameter.Interfaces;
using Surveillance.System.Auditing.Context.Interfaces;
using Surveillance.Universe.Interfaces;
using Surveillance.Analytics.Streams.Interfaces;
using Surveillance.Universe.Subscribers.Interfaces;

namespace Surveillance.Universe
{
    public class UniverseRuleSubscriber : IUniverseRuleSubscriber
    {
        private readonly ISpoofingSubscriber _spoofingSubscriber;
        private readonly ICancelledOrderSubscriber _cancelledOrderSubscriber;
        private readonly IHighProfitsSubscriber _highProfitSubscriber;
        private readonly IHighVolumeSubscriber _highVolumeSubscriber;
        private readonly IMarkingTheCloseSubscriber _markingTheCloseSubscriber;
        private readonly ILayeringSubscriber _layeringSubscriber;
        private readonly IWashTradeSubscriber _washTradeSubscriber;

        private readonly IRuleParameterApiRepository _ruleParameterApiRepository;
        private readonly ILogger<UniverseRuleSubscriber> _logger;

        public UniverseRuleSubscriber(
            ISpoofingSubscriber spoofingSubscriber,
            ICancelledOrderSubscriber cancelledOrderSubscriber,
            IHighProfitsSubscriber highProfitSubscriber,
            IHighVolumeSubscriber highVolumeSubscriber,
            IMarkingTheCloseSubscriber markingTheCloseSubscriber,
            ILayeringSubscriber layeringSubscriber,
            IWashTradeSubscriber washTradeSubscriber,
            IRuleParameterApiRepository ruleParameterApiRepository,
            ILogger<UniverseRuleSubscriber> logger)
        {
            _spoofingSubscriber = spoofingSubscriber ?? throw new ArgumentNullException(nameof(spoofingSubscriber));
            _cancelledOrderSubscriber = cancelledOrderSubscriber ?? throw new ArgumentNullException(nameof(cancelledOrderSubscriber));
            _highProfitSubscriber = highProfitSubscriber ?? throw new ArgumentNullException(nameof(highProfitSubscriber));
            _highVolumeSubscriber = highVolumeSubscriber ?? throw new ArgumentNullException(nameof(highVolumeSubscriber));
            _markingTheCloseSubscriber = markingTheCloseSubscriber ?? throw new ArgumentNullException(nameof(markingTheCloseSubscriber));
            _layeringSubscriber = layeringSubscriber ?? throw new ArgumentNullException(nameof(layeringSubscriber));
            _washTradeSubscriber = washTradeSubscriber ?? throw new ArgumentNullException(nameof(washTradeSubscriber));

            _ruleParameterApiRepository = ruleParameterApiRepository
                ?? throw new ArgumentNullException(nameof(ruleParameterApiRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task SubscribeRules(
             ScheduledExecution execution,
             IUniversePlayer player,
             IUniverseAlertStream alertStream,
             ISystemProcessOperationContext opCtx)
        {
            if (execution == null
                || player == null)
            {
                _logger.LogInformation($"UniverseRuleSubscriber received null execution or player. Returning");
                return;
            }

            _logger.LogInformation($"UniverseRuleSubscriber fetching rule parameters");
            var ruleParameters = await RuleParameters(execution);
            _logger.LogInformation($"UniverseRuleSubscriber has fetched the rule parameters");

            var highVolumeSubscriptions = _highVolumeSubscriber.CollateSubscriptions(execution, ruleParameters, opCtx, alertStream);
            var washTradeSubscriptions = _washTradeSubscriber.CollateSubscriptions(execution, ruleParameters, opCtx, alertStream);
            var highProfitSubscriptions = _highProfitSubscriber.CollateSubscriptions(execution, ruleParameters, opCtx, alertStream);

            foreach (var sub in highVolumeSubscriptions)
            {
                _logger.LogInformation($"UniverseRuleSubscriber Subscribe Rules subscribing a high volume rule");
                player.Subscribe(sub);
            }

            foreach (var sub in washTradeSubscriptions)
            {
                _logger.LogInformation($"UniverseRuleSubscriber Subscribe Rules subscribing a wash trade rule");
                player.Subscribe(sub);
            }

            foreach (var sub in highProfitSubscriptions)
            {
                _logger.LogInformation($"UniverseRuleSubscriber Subscribe Rules subscribing a high profit rule");
                player.Subscribe(sub);
            }

            // _RegisterNotSupportedSubscriptions(ruleParameters, execution, player, alertStream, opCtx);
        }

        private async Task<RuleParameterDto> RuleParameters(ScheduledExecution execution)
        {
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

        private void _RegisterNotSupportedSubscriptions(
            RuleParameterDto ruleParameters,
            ScheduledExecution execution,
            IUniversePlayer player,
            IUniverseAlertStream alertStream,
            ISystemProcessOperationContext opCtx)
        {
            var spoofingSubscriptions = _spoofingSubscriber.CollateSubscriptions(execution, ruleParameters, opCtx, alertStream);
            var cancelledSubscriptions = _cancelledOrderSubscriber.CollateSubscriptions(execution, ruleParameters, opCtx, alertStream);
            var markingTheCloseSubscriptions = _markingTheCloseSubscriber.CollateSubscriptions(execution, ruleParameters, opCtx, alertStream);
            var layeringSubscriptions = _layeringSubscriber.CollateSubscriptions(execution, ruleParameters, opCtx, alertStream);

            foreach (var sub in spoofingSubscriptions)
                player.Subscribe(sub);

            foreach (var sub in cancelledSubscriptions)
                player.Subscribe(sub);

            foreach (var sub in markingTheCloseSubscriptions)
                player.Subscribe(sub);

            foreach (var sub in layeringSubscriptions)
                player.Subscribe(sub);
        }
    }
}