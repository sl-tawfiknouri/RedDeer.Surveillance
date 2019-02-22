﻿using System;
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
using Surveillance.Engine.Rules.Universe.Subscribers.Interfaces;

namespace Surveillance.Engine.Rules.Universe
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

        private readonly IRuleParameterDtoIdExtractor _idExtractor;
        private readonly ILogger<UniverseRuleSubscriber> _logger;

        public UniverseRuleSubscriber(
            ISpoofingSubscriber spoofingSubscriber,
            ICancelledOrderSubscriber cancelledOrderSubscriber,
            IHighProfitsSubscriber highProfitSubscriber,
            IHighVolumeSubscriber highVolumeSubscriber,
            IMarkingTheCloseSubscriber markingTheCloseSubscriber,
            ILayeringSubscriber layeringSubscriber,
            IWashTradeSubscriber washTradeSubscriber,
            IRuleParameterDtoIdExtractor idExtractor,
            ILogger<UniverseRuleSubscriber> logger)
        {
            _spoofingSubscriber = spoofingSubscriber ?? throw new ArgumentNullException(nameof(spoofingSubscriber));
            _cancelledOrderSubscriber = cancelledOrderSubscriber ?? throw new ArgumentNullException(nameof(cancelledOrderSubscriber));
            _highProfitSubscriber = highProfitSubscriber ?? throw new ArgumentNullException(nameof(highProfitSubscriber));
            _highVolumeSubscriber = highVolumeSubscriber ?? throw new ArgumentNullException(nameof(highVolumeSubscriber));
            _markingTheCloseSubscriber = markingTheCloseSubscriber ?? throw new ArgumentNullException(nameof(markingTheCloseSubscriber));
            _layeringSubscriber = layeringSubscriber ?? throw new ArgumentNullException(nameof(layeringSubscriber));
            _washTradeSubscriber = washTradeSubscriber ?? throw new ArgumentNullException(nameof(washTradeSubscriber));
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
                _highVolumeSubscriber.CollateSubscriptions(execution, ruleParameters, opCtx, dataRequestSubscriber, alertStream);

            var washTradeSubscriptions =
                _washTradeSubscriber.CollateSubscriptions(execution, ruleParameters, opCtx, dataRequestSubscriber, alertStream);

            var highProfitSubscriptions =
                _highProfitSubscriber.CollateSubscriptions(execution, ruleParameters, opCtx, dataRequestSubscriber, alertStream);

            var cancelledSubscriptions =
                _cancelledOrderSubscriber.CollateSubscriptions(execution, ruleParameters, opCtx, dataRequestSubscriber, alertStream);

            var markingTheCloseSubscriptions =
                _markingTheCloseSubscriber.CollateSubscriptions(execution, ruleParameters, opCtx, dataRequestSubscriber, alertStream);

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
                _spoofingSubscriber.CollateSubscriptions(execution, ruleParameters, opCtx, dataRequestSubscriber, alertStream);

            var layeringSubscriptions =
                _layeringSubscriber.CollateSubscriptions(execution, ruleParameters, opCtx, dataRequestSubscriber, alertStream);

            foreach (var sub in spoofingSubscriptions)
                player.Subscribe(sub);

            foreach (var sub in layeringSubscriptions)
                player.Subscribe(sub);
        }
    }
}