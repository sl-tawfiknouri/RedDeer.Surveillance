using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Domain.Scheduling;
using Microsoft.Extensions.Logging;
using RedDeer.Contracts.SurveillanceService.Api.RuleParameter;
using Surveillance.Auditing.Context.Interfaces;
using Surveillance.Engine.Rules.Analytics.Streams.Interfaces;
using Surveillance.Engine.Rules.Data.Subscribers.Interfaces;
using Surveillance.Engine.Rules.RuleParameters.Interfaces;
using Surveillance.Engine.Rules.Universe.Interfaces;
using Surveillance.Engine.Rules.Universe.Subscribers.Equity.Interfaces;
using Surveillance.Engine.Rules.Universe.Subscribers.Interfaces;

namespace Surveillance.Engine.Rules.Universe
{
    public class UniverseRuleSubscriber : IUniverseRuleSubscriber
    {
        private readonly ISpoofingEquitySubscriber _spoofingEquitySubscriber;
        private readonly ICancelledOrderEquitySubscriber _cancelledOrderEquitySubscriber;
        private readonly IHighProfitsEquitySubscriber _highProfitEquitySubscriber;
        private readonly IHighVolumeEquitySubscriber _highVolumeEquitySubscriber;
        private readonly IMarkingTheCloseEquitySubscriber _markingTheCloseEquitySubscriber;
        private readonly ILayeringEquitySubscriber _layeringEquitySubscriber;
        private readonly IWashTradeEquitySubscriber _washTradeEquitySubscriber;

        private readonly IRuleParameterDtoIdExtractor _idExtractor;
        private readonly ILogger<UniverseRuleSubscriber> _logger;

        public UniverseRuleSubscriber(
            ISpoofingEquitySubscriber spoofingEquitySubscriber,
            ICancelledOrderEquitySubscriber cancelledOrderEquitySubscriber,
            IHighProfitsEquitySubscriber highProfitEquitySubscriber,
            IHighVolumeEquitySubscriber highVolumeEquitySubscriber,
            IMarkingTheCloseEquitySubscriber markingTheCloseEquitySubscriber,
            ILayeringEquitySubscriber layeringEquitySubscriber,
            IWashTradeEquitySubscriber washTradeEquitySubscriber,
            IRuleParameterDtoIdExtractor idExtractor,
            ILogger<UniverseRuleSubscriber> logger)
        {
            _spoofingEquitySubscriber = spoofingEquitySubscriber ?? throw new ArgumentNullException(nameof(spoofingEquitySubscriber));
            _cancelledOrderEquitySubscriber = cancelledOrderEquitySubscriber ?? throw new ArgumentNullException(nameof(cancelledOrderEquitySubscriber));
            _highProfitEquitySubscriber = highProfitEquitySubscriber ?? throw new ArgumentNullException(nameof(highProfitEquitySubscriber));
            _highVolumeEquitySubscriber = highVolumeEquitySubscriber ?? throw new ArgumentNullException(nameof(highVolumeEquitySubscriber));
            _markingTheCloseEquitySubscriber = markingTheCloseEquitySubscriber ?? throw new ArgumentNullException(nameof(markingTheCloseEquitySubscriber));
            _layeringEquitySubscriber = layeringEquitySubscriber ?? throw new ArgumentNullException(nameof(layeringEquitySubscriber));
            _washTradeEquitySubscriber = washTradeEquitySubscriber ?? throw new ArgumentNullException(nameof(washTradeEquitySubscriber));
            _idExtractor = idExtractor ?? throw new ArgumentNullException(nameof(idExtractor));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<IReadOnlyCollection<string>> SubscribeRules(
             ScheduledExecution execution,
             IUniversePlayer player,
             IUniverseAlertStream alertStream,
             IUniverseDataRequestsSubscriber dataRequestSubscriber,
             ISystemProcessOperationContext opCtx,
             RuleParameterDto ruleParameters)
        {
            if (execution == null
                || player == null)
            {
                _logger.LogInformation($"UniverseRuleSubscriber received null execution or player. Returning");
                return new string[0];
            }

            var highVolumeSubscriptions =
                _highVolumeEquitySubscriber.CollateSubscriptions(execution, ruleParameters, opCtx, dataRequestSubscriber, alertStream);

            var washTradeSubscriptions =
                _washTradeEquitySubscriber.CollateSubscriptions(execution, ruleParameters, opCtx, dataRequestSubscriber, alertStream);

            var highProfitSubscriptions =
                _highProfitEquitySubscriber.CollateSubscriptions(execution, ruleParameters, opCtx, dataRequestSubscriber, alertStream);

            var cancelledSubscriptions =
                _cancelledOrderEquitySubscriber.CollateSubscriptions(execution, ruleParameters, opCtx, dataRequestSubscriber, alertStream);

            var markingTheCloseSubscriptions =
                _markingTheCloseEquitySubscriber.CollateSubscriptions(execution, ruleParameters, opCtx, dataRequestSubscriber, alertStream);

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

            foreach (var sub in cancelledSubscriptions)
            {
                _logger.LogInformation($"UniverseRuleSubscriber Subscribe Rules subscribing a cancellation ratio rule");
                player.Subscribe(sub);
            }

            foreach (var sub in markingTheCloseSubscriptions)
            {
                _logger.LogInformation($"UniverseRuleSubscriber Subscribe Rules subscribing a marking the close rule");
                player.Subscribe(sub);
            }

            var ids = _idExtractor.ExtractIds(ruleParameters);

            return ids;

            // _RegisterNotSupportedSubscriptions(ruleParameters, execution, player, alertStream, opCtx);
        }

        private void _RegisterNotSupportedSubscriptions(
            RuleParameterDto ruleParameters,
            ScheduledExecution execution,
            IUniversePlayer player,
            IUniverseAlertStream alertStream,
            IUniverseDataRequestsSubscriber dataRequestSubscriber,
            ISystemProcessOperationContext opCtx)
        {
            var spoofingSubscriptions = 
                _spoofingEquitySubscriber.CollateSubscriptions(execution, ruleParameters, opCtx, dataRequestSubscriber, alertStream);

            var layeringSubscriptions =
                _layeringEquitySubscriber.CollateSubscriptions(execution, ruleParameters, opCtx, dataRequestSubscriber, alertStream);

            foreach (var sub in spoofingSubscriptions)
                player.Subscribe(sub);

            foreach (var sub in layeringSubscriptions)
                player.Subscribe(sub);
        }
    }
}