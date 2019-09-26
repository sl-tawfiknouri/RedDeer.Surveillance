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
    using Surveillance.Data.Universe.Interfaces;
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
    using Surveillance.Engine.Rules.Rules.FixedIncome.HighVolume;
    using Surveillance.Engine.Rules.Rules.FixedIncome.WashTrade;
    using Surveillance.Engine.Rules.Universe.Interfaces;
    using Surveillance.Engine.Rules.Universe.Subscribers.Equity.Interfaces;
    using Surveillance.Engine.Rules.Universe.Subscribers.FixedIncome.Interfaces;

    /// <summary>
    /// The universe rule subscriber.
    /// </summary>
    public class UniverseRuleSubscriber : IUniverseRuleSubscriber
    {
        /// <summary>
        /// The cancelled order equity subscriber.
        /// </summary>
        private readonly ICancelledOrderEquitySubscriber cancelledOrderEquitySubscriber;

        /// <summary>
        /// The high profit equity subscriber.
        /// </summary>
        private readonly IHighProfitsEquitySubscriber highProfitEquitySubscriber;

        /// <summary>
        /// The high profit fixed income subscriber.
        /// </summary>
        private readonly IHighProfitsFixedIncomeSubscriber highProfitFixedIncomeSubscriber;

        /// <summary>
        /// The high volume equity subscriber.
        /// </summary>
        private readonly IHighVolumeEquitySubscriber highVolumeEquitySubscriber;

        /// <summary>
        /// The high volume fixed income subscriber.
        /// </summary>
        private readonly IHighVolumeFixedIncomeSubscriber highVolumeFixedIncomeSubscriber;

        /// <summary>
        /// The id extractor.
        /// </summary>
        private readonly IRuleParameterDtoIdExtractor idExtractor;

        /// <summary>
        /// The layering equity subscriber.
        /// </summary>
        private readonly ILayeringEquitySubscriber layeringEquitySubscriber;

        /// <summary>
        /// The marking the close equity subscriber.
        /// </summary>
        private readonly IMarkingTheCloseEquitySubscriber markingTheCloseEquitySubscriber;

        /// <summary>
        /// The placing orders equity subscriber.
        /// </summary>
        private readonly IPlacingOrdersWithNoIntentToExecuteEquitySubscriber placingOrdersEquitySubscriber;

        /// <summary>
        /// The ramping equity subscriber.
        /// </summary>
        private readonly IRampingEquitySubscriber rampingEquitySubscriber;

        /// <summary>
        /// The spoofing equity subscriber.
        /// </summary>
        private readonly ISpoofingEquitySubscriber spoofingEquitySubscriber;

        /// <summary>
        /// The wash trade equity subscriber.
        /// </summary>
        private readonly IWashTradeEquitySubscriber washTradeEquitySubscriber;

        /// <summary>
        /// The wash trade fixed income subscriber.
        /// </summary>
        private readonly IWashTradeFixedIncomeSubscriber washTradeFixedIncomeSubscriber;

        /// <summary>
        /// The logger.
        /// </summary>
        private readonly ILogger<UniverseRuleSubscriber> logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="UniverseRuleSubscriber"/> class.
        /// </summary>
        /// <param name="spoofingEquitySubscriber">
        /// The spoofing equity subscriber.
        /// </param>
        /// <param name="cancelledOrderEquitySubscriber">
        /// The cancelled order equity subscriber.
        /// </param>
        /// <param name="highProfitEquitySubscriber">
        /// The high profit equity subscriber.
        /// </param>
        /// <param name="highVolumeEquitySubscriber">
        /// The high volume equity subscriber.
        /// </param>
        /// <param name="markingTheCloseEquitySubscriber">
        /// The marking the close equity subscriber.
        /// </param>
        /// <param name="layeringEquitySubscriber">
        /// The layering equity subscriber.
        /// </param>
        /// <param name="washTradeEquitySubscriber">
        /// The wash trade equity subscriber.
        /// </param>
        /// <param name="rampingEquitySubscriber">
        /// The ramping equity subscriber.
        /// </param>
        /// <param name="placingOrdersEquitySubscriber">
        /// The placing orders equity subscriber.
        /// </param>
        /// <param name="idExtractor">
        /// The id extractor.
        /// </param>
        /// <param name="logger">
        /// The logger.
        /// </param>
        /// <param name="washTradeFixedIncomeSubscriber">
        /// The wash trade fixed income subscriber.
        /// </param>
        /// <param name="highVolumeFixedIncomeSubscriber">
        /// The high volume fixed income subscriber.
        /// </param>
        /// <param name="highProfitFixedIncomeSubscriber">
        /// The high profit fixed income subscriber.
        /// </param>
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
            this.spoofingEquitySubscriber = 
                spoofingEquitySubscriber ?? throw new ArgumentNullException(nameof(spoofingEquitySubscriber));
            this.cancelledOrderEquitySubscriber = 
                cancelledOrderEquitySubscriber ?? throw new ArgumentNullException(nameof(cancelledOrderEquitySubscriber));
            this.highProfitEquitySubscriber = 
                highProfitEquitySubscriber ?? throw new ArgumentNullException(nameof(highProfitEquitySubscriber));
            this.highVolumeEquitySubscriber = 
                highVolumeEquitySubscriber ?? throw new ArgumentNullException(nameof(highVolumeEquitySubscriber));
            this.markingTheCloseEquitySubscriber = 
                markingTheCloseEquitySubscriber ?? throw new ArgumentNullException(nameof(markingTheCloseEquitySubscriber));
            this.layeringEquitySubscriber = 
                layeringEquitySubscriber ?? throw new ArgumentNullException(nameof(layeringEquitySubscriber));
            this.washTradeEquitySubscriber = 
                washTradeEquitySubscriber ?? throw new ArgumentNullException(nameof(washTradeEquitySubscriber));
            this.rampingEquitySubscriber = 
                rampingEquitySubscriber ?? throw new ArgumentNullException(nameof(rampingEquitySubscriber));
            this.placingOrdersEquitySubscriber =
                placingOrdersEquitySubscriber ?? throw new ArgumentNullException(nameof(placingOrdersEquitySubscriber));
            this.idExtractor = idExtractor ?? throw new ArgumentNullException(nameof(idExtractor));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.washTradeFixedIncomeSubscriber = 
                washTradeFixedIncomeSubscriber ?? throw new ArgumentNullException(nameof(washTradeFixedIncomeSubscriber));
            this.highVolumeFixedIncomeSubscriber =
                highVolumeFixedIncomeSubscriber ?? throw new ArgumentNullException(nameof(highVolumeFixedIncomeSubscriber));
            this.highProfitFixedIncomeSubscriber = 
                highProfitFixedIncomeSubscriber ?? throw new ArgumentNullException(nameof(highProfitFixedIncomeSubscriber));
        }

        /// <summary>
        /// The subscribe rules.
        /// </summary>
        /// <param name="execution">
        /// The execution.
        /// </param>
        /// <param name="player">
        /// The player.
        /// </param>
        /// <param name="alertStream">
        /// The alert stream.
        /// </param>
        /// <param name="dataRequestSubscriber">
        /// The data request subscriber.
        /// </param>
        /// <param name="judgementService">
        /// The judgement service.
        /// </param>
        /// <param name="operationContext">
        /// The operation context.
        /// </param>
        /// <param name="ruleParameters">
        /// The rule parameters.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task<IReadOnlyCollection<string>> SubscribeRules(
            ScheduledExecution execution,
            IUniversePlayer player,
            IUniverseAlertStream alertStream,
            IUniverseDataRequestsSubscriber dataRequestSubscriber,
            IJudgementService judgementService,
            ISystemProcessOperationContext operationContext,
            RuleParameterDto ruleParameters)
        {
            if (execution == null || player == null)
            {
                this.logger.LogInformation("received null execution or player. Returning");
                return new string[0];
            }

            // EQUITY
            var highVolumeSubscriptions = this.highVolumeEquitySubscriber.CollateSubscriptions(
                execution,
                ruleParameters,
                operationContext,
                dataRequestSubscriber,
                judgementService,
                alertStream);

            var washTradeSubscriptions = this.washTradeEquitySubscriber.CollateSubscriptions(
                execution,
                ruleParameters,
                operationContext,
                dataRequestSubscriber,
                judgementService,
                alertStream);

            var highProfitSubscriptions = this.highProfitEquitySubscriber.CollateSubscriptions(
                execution,
                ruleParameters,
                operationContext,
                dataRequestSubscriber,
                judgementService,
                alertStream);

            var cancelledSubscriptions = this.cancelledOrderEquitySubscriber.CollateSubscriptions(
                execution,
                ruleParameters,
                operationContext,
                dataRequestSubscriber,
                judgementService,
                alertStream);

            var markingTheCloseSubscriptions = this.markingTheCloseEquitySubscriber.CollateSubscriptions(
                execution,
                ruleParameters,
                operationContext,
                dataRequestSubscriber,
                judgementService,
                alertStream);

            var spoofingSubscriptions = this.spoofingEquitySubscriber.CollateSubscriptions(
                execution,
                ruleParameters,
                operationContext,
                dataRequestSubscriber,
                judgementService,
                alertStream);

            var rampingSubscriptions = this.rampingEquitySubscriber.CollateSubscriptions(
                execution,
                ruleParameters,
                operationContext,
                dataRequestSubscriber,
                judgementService,
                alertStream);

            var placingOrdersSubscriptions = this.placingOrdersEquitySubscriber.CollateSubscriptions(
                execution,
                ruleParameters,
                operationContext,
                dataRequestSubscriber,
                judgementService,
                alertStream);

            // FIXED INCOME
            var washTradeFixedIncomeSubscriptions = this.washTradeFixedIncomeSubscriber.CollateSubscriptions(
                execution,
                ruleParameters,
                operationContext,
                dataRequestSubscriber,
                judgementService,
                alertStream);

            var highVolumeFixedIncomeSubscriptions = this.highVolumeFixedIncomeSubscriber.CollateSubscriptions(
                execution,
                ruleParameters,
                operationContext,
                dataRequestSubscriber,
                judgementService,
                alertStream);

            var highProfitFixedIncomeSubscriptions = this.highProfitFixedIncomeSubscriber.CollateSubscriptions(
                execution,
                ruleParameters,
                operationContext,
                dataRequestSubscriber,
                judgementService,
                alertStream);

            // EQUITY
            foreach (var sub in highVolumeSubscriptions)
            {
                this.logger.LogInformation($"Subscribe Rules subscribing a {nameof(HighVolumeRule)}");
                player.Subscribe(sub);
            }

            foreach (var sub in washTradeSubscriptions)
            {
                this.logger.LogInformation($"Subscribe Rules subscribing a {nameof(WashTradeRule)}");
                player.Subscribe(sub);
            }

            foreach (var sub in highProfitSubscriptions)
            {
                this.logger.LogInformation($"Subscribe Rules subscribing a {nameof(HighProfitsRule)}");
                player.Subscribe(sub);
            }

            foreach (var sub in cancelledSubscriptions)
            {
                this.logger.LogInformation($"Subscribe Rules subscribing a {nameof(CancelledOrderRule)}");
                player.Subscribe(sub);
            }

            foreach (var sub in markingTheCloseSubscriptions)
            {
                this.logger.LogInformation($"Subscribe Rules subscribing a {nameof(MarkingTheCloseRule)}");
                player.Subscribe(sub);
            }

            foreach (var sub in spoofingSubscriptions)
            {
                this.logger.LogInformation($"Subscribe Rules subscribing a {nameof(SpoofingRule)}");
                player.Subscribe(sub);
            }

            foreach (var sub in rampingSubscriptions)
            {
                this.logger.LogInformation($"Subscribe rules subscribing a {nameof(RampingRule)}");
                player.Subscribe(sub);
            }

            foreach (var sub in placingOrdersSubscriptions)
            {
                this.logger.LogInformation(
                    $"Subscribe Rules subscribing a {nameof(PlacingOrdersWithNoIntentToExecuteRule)}");
                player.Subscribe(sub);
            }

            // FIXED INCOME
            foreach (var sub in washTradeFixedIncomeSubscriptions)
            {
                this.logger.LogInformation($"Subscribe Rules subscribing a {nameof(FixedIncomeWashTradeRule)}");
                player.Subscribe(sub);
            }

            foreach (var sub in highVolumeFixedIncomeSubscriptions)
            {
                this.logger.LogInformation(
                    $"Subscribe Rules subscribing a {nameof(FixedIncomeHighVolumeRule)}");
                player.Subscribe(sub);
            }

            foreach (var sub in highProfitFixedIncomeSubscriptions)
            {
                this.logger.LogInformation($"Subscribe Rules subscribing a {nameof(FixedIncomeHighProfitsRule)}");
                player.Subscribe(sub);
            }

            var ids = this.idExtractor.ExtractIds(ruleParameters);

            if (ids == null || !ids.Any())
            {
                this.logger.LogError("did not have any ids successfully extracted from the rule parameters");

                return ids;
            }

            var jointIds = ids.Aggregate((a, b) => $"{a} {b}");
            this.logger.LogInformation($"subscriber processed for the following ids {jointIds}");

            return await Task.FromResult(ids);
        }
    }
}