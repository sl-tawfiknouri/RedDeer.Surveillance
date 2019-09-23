namespace Surveillance.Engine.Rules.Universe
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Domain.Surveillance.Scheduling;

    using Microsoft.Extensions.Logging;

    using RedDeer.Contracts.SurveillanceService.Api.RuleParameter;

    using Surveillance.Auditing.Context.Interfaces;
    using Surveillance.Engine.Rules.Analytics.Streams.Interfaces;
    using Surveillance.Engine.Rules.Data.Subscribers.Interfaces;
    using Surveillance.Engine.Rules.Judgements.Interfaces;
    using Surveillance.Engine.Rules.RuleParameters.Interfaces;
    using Surveillance.Engine.Rules.Rules.Equity.CancelledOrders;
    using Surveillance.Engine.Rules.Rules.Equity.HighProfits;
    using Surveillance.Engine.Rules.Rules.Equity.HighVolume;
    using Surveillance.Engine.Rules.Rules.Equity.MarkingTheClose;
    using Surveillance.Engine.Rules.Rules.Equity.PlacingOrderNoIntentToExecute;
    using Surveillance.Engine.Rules.Rules.Equity.Ramping;
    using Surveillance.Engine.Rules.Rules.Equity.Spoofing;
    using Surveillance.Engine.Rules.Rules.Equity.WashTrade;
    using Surveillance.Engine.Rules.Rules.FixedIncome.HighProfits;
    using Surveillance.Engine.Rules.Rules.FixedIncome.HighVolumeIssuance;
    using Surveillance.Engine.Rules.Rules.FixedIncome.WashTrade;
    using Surveillance.Engine.Rules.Universe.Interfaces;
    using Surveillance.Engine.Rules.Universe.Subscribers.Equity.Interfaces;
    using Surveillance.Engine.Rules.Universe.Subscribers.FixedIncome.Interfaces;

    public class UniverseRuleSubscriber : IUniverseRuleSubscriber
    {
        private readonly ICancelledOrderEquitySubscriber _cancelledOrderEquitySubscriber;

        private readonly IHighProfitsEquitySubscriber _highProfitEquitySubscriber;

        private readonly IHighProfitsFixedIncomeSubscriber _highProfitFixedIncomeSubscriber;

        private readonly IHighVolumeEquitySubscriber _highVolumeEquitySubscriber;

        private readonly IHighVolumeFixedIncomeSubscriber _highVolumeFixedIncomeSubscriber;

        private readonly IRuleParameterDtoIdExtractor _idExtractor;

        private readonly ILayeringEquitySubscriber _layeringEquitySubscriber;

        private readonly ILogger<UniverseRuleSubscriber> _logger;

        private readonly IMarkingTheCloseEquitySubscriber _markingTheCloseEquitySubscriber;

        private readonly IPlacingOrdersWithNoIntentToExecuteEquitySubscriber _placingOrdersEquitySubscriber;

        private readonly IRampingEquitySubscriber _rampingEquitySubscriber;

        // Equity
        private readonly ISpoofingEquitySubscriber _spoofingEquitySubscriber;

        private readonly IWashTradeEquitySubscriber _washTradeEquitySubscriber;

        // Fixed Income
        private readonly IWashTradeFixedIncomeSubscriber _washTradeFixedIncomeSubscriber;

        public UniverseRuleSubscriber(
            ISpoofingEquitySubscriber spoofingEquitySubscriber,
            ICancelledOrderEquitySubscriber cancelledOrderEquitySubscriber,
            IHighProfitsEquitySubscriber highProfitEquitySubscriber,
            IHighVolumeEquitySubscriber highVolumeEquitySubscriber,
            IMarkingTheCloseEquitySubscriber markingTheCloseEquitySubscriber,
            ILayeringEquitySubscriber layeringEquitySubscriber,
            IWashTradeEquitySubscriber washTradeEquitySubscriber,
            IRampingEquitySubscriber rampingEquitySubscriber,
            IPlacingOrdersWithNoIntentToExecuteEquitySubscriber placingOrdersEquitySubscriber,
            IRuleParameterDtoIdExtractor idExtractor,
            ILogger<UniverseRuleSubscriber> logger,
            IWashTradeFixedIncomeSubscriber washTradeFixedIncomeSubscriber,
            IHighVolumeFixedIncomeSubscriber highVolumeFixedIncomeSubscriber,
            IHighProfitsFixedIncomeSubscriber highProfitFixedIncomeSubscriber)
        {
            this._spoofingEquitySubscriber = spoofingEquitySubscriber
                                             ?? throw new ArgumentNullException(nameof(spoofingEquitySubscriber));
            this._cancelledOrderEquitySubscriber = cancelledOrderEquitySubscriber
                                                   ?? throw new ArgumentNullException(
                                                       nameof(cancelledOrderEquitySubscriber));
            this._highProfitEquitySubscriber = highProfitEquitySubscriber
                                               ?? throw new ArgumentNullException(nameof(highProfitEquitySubscriber));
            this._highVolumeEquitySubscriber = highVolumeEquitySubscriber
                                               ?? throw new ArgumentNullException(nameof(highVolumeEquitySubscriber));
            this._markingTheCloseEquitySubscriber = markingTheCloseEquitySubscriber
                                                    ?? throw new ArgumentNullException(
                                                        nameof(markingTheCloseEquitySubscriber));
            this._layeringEquitySubscriber = layeringEquitySubscriber
                                             ?? throw new ArgumentNullException(nameof(layeringEquitySubscriber));
            this._washTradeEquitySubscriber = washTradeEquitySubscriber
                                              ?? throw new ArgumentNullException(nameof(washTradeEquitySubscriber));
            this._rampingEquitySubscriber = rampingEquitySubscriber
                                            ?? throw new ArgumentNullException(nameof(rampingEquitySubscriber));
            this._placingOrdersEquitySubscriber = placingOrdersEquitySubscriber
                                                  ?? throw new ArgumentNullException(
                                                      nameof(placingOrdersEquitySubscriber));

            this._idExtractor = idExtractor ?? throw new ArgumentNullException(nameof(idExtractor));
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));

            this._washTradeFixedIncomeSubscriber = washTradeFixedIncomeSubscriber
                                                   ?? throw new ArgumentNullException(
                                                       nameof(washTradeFixedIncomeSubscriber));
            this._highVolumeFixedIncomeSubscriber = highVolumeFixedIncomeSubscriber
                                                    ?? throw new ArgumentNullException(
                                                        nameof(highVolumeFixedIncomeSubscriber));
            this._highProfitFixedIncomeSubscriber = highProfitFixedIncomeSubscriber
                                                    ?? throw new ArgumentNullException(
                                                        nameof(highProfitFixedIncomeSubscriber));
        }

        public async Task<IReadOnlyCollection<string>> SubscribeRules(
            ScheduledExecution execution,
            IUniversePlayer player,
            IUniverseAlertStream alertStream,
            IUniverseDataRequestsSubscriber dataRequestSubscriber,
            IJudgementService judgementService,
            ISystemProcessOperationContext opCtx,
            RuleParameterDto ruleParameters)
        {
            if (execution == null || player == null)
            {
                this._logger.LogInformation("received null execution or player. Returning");
                return new string[0];
            }

            // EQUITY
            var highVolumeSubscriptions = this._highVolumeEquitySubscriber.CollateSubscriptions(
                execution,
                ruleParameters,
                opCtx,
                dataRequestSubscriber,
                judgementService,
                alertStream);

            var washTradeSubscriptions = this._washTradeEquitySubscriber.CollateSubscriptions(
                execution,
                ruleParameters,
                opCtx,
                dataRequestSubscriber,
                judgementService,
                alertStream);

            var highProfitSubscriptions = this._highProfitEquitySubscriber.CollateSubscriptions(
                execution,
                ruleParameters,
                opCtx,
                dataRequestSubscriber,
                judgementService,
                alertStream);

            var cancelledSubscriptions = this._cancelledOrderEquitySubscriber.CollateSubscriptions(
                execution,
                ruleParameters,
                opCtx,
                dataRequestSubscriber,
                judgementService,
                alertStream);

            var markingTheCloseSubscriptions = this._markingTheCloseEquitySubscriber.CollateSubscriptions(
                execution,
                ruleParameters,
                opCtx,
                dataRequestSubscriber,
                judgementService,
                alertStream);

            var spoofingSubscriptions = this._spoofingEquitySubscriber.CollateSubscriptions(
                execution,
                ruleParameters,
                opCtx,
                dataRequestSubscriber,
                judgementService,
                alertStream);

            var rampingSubscriptions = this._rampingEquitySubscriber.CollateSubscriptions(
                execution,
                ruleParameters,
                opCtx,
                dataRequestSubscriber,
                judgementService,
                alertStream);

            var placingOrdersSubscriptions = this._placingOrdersEquitySubscriber.CollateSubscriptions(
                execution,
                ruleParameters,
                opCtx,
                dataRequestSubscriber,
                judgementService,
                alertStream);

            // FIXED INCOME
            var washTradeFixedIncomeSubscriptions = this._washTradeFixedIncomeSubscriber.CollateSubscriptions(
                execution,
                ruleParameters,
                opCtx,
                dataRequestSubscriber,
                judgementService,
                alertStream);

            var highVolumeFixedIncomeSubscriptions = this._highVolumeFixedIncomeSubscriber.CollateSubscriptions(
                execution,
                ruleParameters,
                opCtx,
                dataRequestSubscriber,
                judgementService,
                alertStream);

            var highProfitFixedIncomeSubscriptions = this._highProfitFixedIncomeSubscriber.CollateSubscriptions(
                execution,
                ruleParameters,
                opCtx,
                dataRequestSubscriber,
                judgementService,
                alertStream);

            // EQUITY
            foreach (var sub in highVolumeSubscriptions)
            {
                this._logger.LogInformation($"Subscribe Rules subscribing a {nameof(HighVolumeRule)}");
                player.Subscribe(sub);
            }

            foreach (var sub in washTradeSubscriptions)
            {
                this._logger.LogInformation($"Subscribe Rules subscribing a {nameof(WashTradeRule)}");
                player.Subscribe(sub);
            }

            foreach (var sub in highProfitSubscriptions)
            {
                this._logger.LogInformation($"Subscribe Rules subscribing a {nameof(HighProfitsRule)}");
                player.Subscribe(sub);
            }

            foreach (var sub in cancelledSubscriptions)
            {
                this._logger.LogInformation($"Subscribe Rules subscribing a {nameof(CancelledOrderRule)}");
                player.Subscribe(sub);
            }

            foreach (var sub in markingTheCloseSubscriptions)
            {
                this._logger.LogInformation($"Subscribe Rules subscribing a {nameof(MarkingTheCloseRule)}");
                player.Subscribe(sub);
            }

            foreach (var sub in spoofingSubscriptions)
            {
                this._logger.LogInformation($"Subscribe Rules subscribing a {nameof(SpoofingRule)}");
                player.Subscribe(sub);
            }

            foreach (var sub in rampingSubscriptions)
            {
                this._logger.LogInformation($"Subscribe rules subscribing a {nameof(RampingRule)}");
                player.Subscribe(sub);
            }

            foreach (var sub in placingOrdersSubscriptions)
            {
                this._logger.LogInformation(
                    $"Subscribe Rules subscribing a {nameof(PlacingOrdersWithNoIntentToExecuteRule)}");
                player.Subscribe(sub);
            }

            // FIXED INCOME
            foreach (var sub in washTradeFixedIncomeSubscriptions)
            {
                this._logger.LogInformation($"Subscribe Rules subscribing a {nameof(FixedIncomeWashTradeRule)}");
                player.Subscribe(sub);
            }

            foreach (var sub in highVolumeFixedIncomeSubscriptions)
            {
                this._logger.LogInformation(
                    $"Subscribe Rules subscribing a {nameof(FixedIncomeHighVolumeIssuanceRule)}");
                player.Subscribe(sub);
            }

            foreach (var sub in highProfitFixedIncomeSubscriptions)
            {
                this._logger.LogInformation($"Subscribe Rules subscribing a {nameof(FixedIncomeHighProfitsRule)}");
                player.Subscribe(sub);
            }

            var ids = this._idExtractor.ExtractIds(ruleParameters);

            if (ids == null || !ids.Any())
            {
                this._logger.LogError("did not have any ids successfully extracted from the rule parameters");

                return ids;
            }

            var jointIds = ids.Aggregate((a, b) => $"{a} {b}");
            this._logger.LogInformation($"subscriber processed for the following ids {jointIds}");

            return await Task.FromResult(ids);

            // _RegisterNotSupportedSubscriptions(ruleParameters, execution, player, alertStream, opCtx);
        }

        private void _RegisterNotSupportedSubscriptions(
            RuleParameterDto ruleParameters,
            ScheduledExecution execution,
            IUniversePlayer player,
            IUniverseAlertStream alertStream,
            IUniverseDataRequestsSubscriber dataRequestSubscriber,
            IJudgementService judgementService,
            ISystemProcessOperationContext opCtx)
        {
            var layeringSubscriptions = this._layeringEquitySubscriber.CollateSubscriptions(
                execution,
                ruleParameters,
                opCtx,
                dataRequestSubscriber,
                judgementService,
                alertStream);

            foreach (var sub in layeringSubscriptions)
                player.Subscribe(sub);
        }
    }
}