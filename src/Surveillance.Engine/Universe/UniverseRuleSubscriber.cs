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
using Surveillance.Engine.Rules.Rules.Equity.CancelledOrders;
using Surveillance.Engine.Rules.Rules.Equity.HighProfits;
using Surveillance.Engine.Rules.Rules.Equity.HighVolume;
using Surveillance.Engine.Rules.Rules.Equity.MarkingTheClose;
using Surveillance.Engine.Rules.Rules.Equity.WashTrade;
using Surveillance.Engine.Rules.Rules.FixedIncome.HighVolume;
using Surveillance.Engine.Rules.Rules.FixedIncome.WashTrade;
using Surveillance.Engine.Rules.Universe.Interfaces;
using Surveillance.Engine.Rules.Universe.Subscribers.Equity.Interfaces;
using Surveillance.Engine.Rules.Universe.Subscribers.FixedIncome.Interfaces;

namespace Surveillance.Engine.Rules.Universe
{
    public class UniverseRuleSubscriber : IUniverseRuleSubscriber
    {
        // Equity
        private readonly ISpoofingEquitySubscriber _spoofingEquitySubscriber;
        private readonly ICancelledOrderEquitySubscriber _cancelledOrderEquitySubscriber;
        private readonly IHighProfitsEquitySubscriber _highProfitEquitySubscriber;
        private readonly IHighVolumeEquitySubscriber _highVolumeEquitySubscriber;
        private readonly IMarkingTheCloseEquitySubscriber _markingTheCloseEquitySubscriber;
        private readonly ILayeringEquitySubscriber _layeringEquitySubscriber;
        private readonly IWashTradeEquitySubscriber _washTradeEquitySubscriber;

        // Fixed Income
        private readonly IWashTradeFixedIncomeSubscriber _washTradeFixedIncomeSubscriber;
        private readonly IHighVolumeFixedIncomeSubscriber _highVolumeFixedIncomeSubscriber;

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
            ILogger<UniverseRuleSubscriber> logger, 
            IWashTradeFixedIncomeSubscriber washTradeFixedIncomeSubscriber,
            IHighVolumeFixedIncomeSubscriber highVolumeFixedIncomeSubscriber)
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

            _washTradeFixedIncomeSubscriber = washTradeFixedIncomeSubscriber ?? throw new ArgumentNullException(nameof(washTradeFixedIncomeSubscriber));
            _highVolumeFixedIncomeSubscriber = highVolumeFixedIncomeSubscriber ?? throw new ArgumentNullException(nameof(highVolumeFixedIncomeSubscriber));
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
                _logger.LogInformation($"{nameof(UniverseRuleSubscriber)} received null execution or player. Returning");
                return new string[0];
            }

            // EQUITY

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

            // FIXED INCOME

            var washTradeFixedIncomeSubscriptions =
                _washTradeFixedIncomeSubscriber.CollateSubscriptions(execution, ruleParameters, opCtx, dataRequestSubscriber, alertStream);

            var highVolumeFixedIncomeSubscriptions =
                _highVolumeFixedIncomeSubscriber.CollateSubscriptions(execution, ruleParameters, opCtx, dataRequestSubscriber, alertStream);

            // EQUITY

            foreach (var sub in highVolumeSubscriptions)
            {
                _logger.LogInformation($"{nameof(UniverseRuleSubscriber)} Subscribe Rules subscribing a {nameof(HighVolumeRule)}");
                player.Subscribe(sub);
            }

            foreach (var sub in washTradeSubscriptions)
            {
                _logger.LogInformation($"{nameof(UniverseRuleSubscriber)} Subscribe Rules subscribing a {nameof(WashTradeRule)}");
                player.Subscribe(sub);
            }

            foreach (var sub in highProfitSubscriptions)
            {
                _logger.LogInformation($"{nameof(UniverseRuleSubscriber)} Subscribe Rules subscribing a {nameof(HighProfitsRule)}");
                player.Subscribe(sub);
            }

            foreach (var sub in cancelledSubscriptions)
            {
                _logger.LogInformation($"{nameof(UniverseRuleSubscriber)} Subscribe Rules subscribing a {nameof(CancelledOrderRule)}");
                player.Subscribe(sub);
            }

            foreach (var sub in markingTheCloseSubscriptions)
            {
                _logger.LogInformation($"{nameof(UniverseRuleSubscriber)} Subscribe Rules subscribing a {nameof(MarkingTheCloseRule)}");
                player.Subscribe(sub);
            }

            // FIXED INCOME

            foreach (var sub in washTradeFixedIncomeSubscriptions)
            {
                _logger.LogInformation($"{nameof(UniverseRuleSubscriber)} Subscribe Rules subscribing a {nameof(FixedIncomeWashTradeRule)}");
                player.Subscribe(sub);
            }

            foreach (var sub in highVolumeFixedIncomeSubscriptions)
            {
                _logger.LogInformation($"{nameof(UniverseRuleSubscriber)} Subscribe Rules subscribing a {nameof(FixedIncomeHighVolumeRule)}");
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