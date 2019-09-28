namespace Surveillance.Engine.Rules.Universe
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Domain.Core.Extensions;
    using Domain.Surveillance.Scheduling;

    using Microsoft.Extensions.Logging;

    using RedDeer.Contracts.SurveillanceService.Api.RuleParameter;

    using Surveillance.Auditing.Context.Interfaces;
    using Surveillance.Data.Universe.Interfaces;
    using Surveillance.Engine.Rules.Analytics.Streams.Interfaces;
    using Surveillance.Engine.Rules.Data.Subscribers.Interfaces;
    using Surveillance.Engine.Rules.Judgements.Interfaces;
    using Surveillance.Engine.Rules.RuleParameters.Interfaces;
    using Surveillance.Engine.Rules.Rules.Interfaces;
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
        public async Task<IUniverseRuleSubscriptionSummary> SubscribeRules(
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

                return new UniverseRuleSubscriptionSummary(null, null);
            }

            var subscriptions = this.BuildSubscriptions(
                execution,
                player,
                alertStream,
                dataRequestSubscriber,
                judgementService,
                operationContext,
                ruleParameters,
                this.highVolumeEquitySubscriber.CollateSubscriptions,
                this.washTradeEquitySubscriber.CollateSubscriptions,
                this.highProfitEquitySubscriber.CollateSubscriptions,
                this.cancelledOrderEquitySubscriber.CollateSubscriptions,
                this.markingTheCloseEquitySubscriber.CollateSubscriptions,
                this.spoofingEquitySubscriber.CollateSubscriptions,
                this.rampingEquitySubscriber.CollateSubscriptions,
                this.placingOrdersEquitySubscriber.CollateSubscriptions,
                this.washTradeFixedIncomeSubscriber.CollateSubscriptions,
                this.highVolumeFixedIncomeSubscriber.CollateSubscriptions,
                this.highProfitFixedIncomeSubscriber.CollateSubscriptions);

            foreach (var subscription in subscriptions)
            {
                if (subscription == null)
                {
                    continue;
                }

                this.logger.LogInformation($"Subscribe Rules subscribing a {subscription.Rule.GetDescription()}");
                player.Subscribe(subscription);
            }
            
            var ids = this.idExtractor.ExtractIds(ruleParameters);

            if (ids == null || !ids.Any())
            {
                this.logger.LogError("did not have any ids successfully extracted from the rule parameters");

                return new UniverseRuleSubscriptionSummary(null, null);
            }

            var jointIds = ids.Aggregate((a, b) => $"{a} {b}");
            this.logger.LogInformation($"subscriber processed for the following ids {jointIds}");

            return await Task.FromResult(new UniverseRuleSubscriptionSummary(ids, subscriptions));
        }

        /// <summary>
        /// The build subscriptions.
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
        /// <param name="args">
        /// The args.
        /// </param>
        /// <returns>
        /// The <see cref="IUniverseRule"/>.
        /// </returns>
        private IReadOnlyCollection<IUniverseRule> BuildSubscriptions(
            ScheduledExecution execution,
            IUniversePlayer player,
            IUniverseAlertStream alertStream,
            IUniverseDataRequestsSubscriber dataRequestSubscriber,
            IJudgementService judgementService,
            ISystemProcessOperationContext operationContext,
            RuleParameterDto ruleParameters,
            params Func<ScheduledExecution,
                    RuleParameterDto,
                    ISystemProcessOperationContext,
                    IUniverseDataRequestsSubscriber,
                    IJudgementService,
                    IUniverseAlertStream,
                    IReadOnlyCollection<IUniverseRule>>[] args)
        {
            var universeRules = new List<IUniverseRule>();

            foreach (var func in args)
            {
                var result = func.Invoke(
                    execution,
                    ruleParameters,
                    operationContext,
                    dataRequestSubscriber,
                    judgementService,
                    alertStream);

                universeRules.AddRange(result);
            }

            return universeRules;
        }
    }
}