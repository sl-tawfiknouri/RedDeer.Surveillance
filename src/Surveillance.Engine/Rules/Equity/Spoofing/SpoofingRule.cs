namespace Surveillance.Engine.Rules.Rules.Equity.Spoofing
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Domain.Core.Trading;
    using Domain.Core.Trading.Execution;
    using Domain.Core.Trading.Execution.Interfaces;
    using Domain.Core.Trading.Factories.Interfaces;
    using Domain.Core.Trading.Interfaces;
    using Domain.Core.Trading.Orders;
    using Domain.Surveillance.Rules;
    using Domain.Surveillance.Rules.Interfaces;
    using Domain.Surveillance.Scheduling;

    using Microsoft.Extensions.Logging;

    using SharedKernel.Contracts.Markets;

    using Surveillance.Auditing.Context.Interfaces;
    using Surveillance.Data.Universe.Interfaces;
    using Surveillance.Data.Universe.MarketEvents;
    using Surveillance.Engine.Rules.Analytics.Streams;
    using Surveillance.Engine.Rules.Analytics.Streams.Interfaces;
    using Surveillance.Engine.Rules.Factories.Equities;
    using Surveillance.Engine.Rules.Factories.Interfaces;
    using Surveillance.Engine.Rules.RuleParameters.Equities.Interfaces;
    using Surveillance.Engine.Rules.Rules.Equity.Spoofing.Interfaces;
    using Surveillance.Engine.Rules.Rules.Interfaces;
    using Surveillance.Engine.Rules.Trades;
    using Surveillance.Engine.Rules.Trades.Interfaces;
    using Surveillance.Engine.Rules.Universe.Filter.Interfaces;

    /// <summary>
    /// The spoofing rule.
    /// </summary>
    public class SpoofingRule : BaseUniverseRule, ISpoofingRule
    {
        /// <summary>
        /// The alert stream.
        /// </summary>
        private readonly IUniverseAlertStream alertStream;

        /// <summary>
        /// The analysis service.
        /// </summary>
        private readonly IOrderAnalysisService analysisService;

        /// <summary>
        /// The equities parameters.
        /// </summary>
        private readonly ISpoofingRuleEquitiesParameters equitiesParameters;

        /// <summary>
        /// The order filter.
        /// </summary>
        private readonly IUniverseOrderFilter orderFilter;

        /// <summary>
        /// The portfolio factory.
        /// </summary>
        private readonly IPortfolioFactory portfolioFactory;

        /// <summary>
        /// The rule context.
        /// </summary>
        private readonly ISystemProcessOperationRunRuleContext ruleContext;

        /// <summary>
        /// The logger.
        /// </summary>
        private readonly ILogger logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="SpoofingRule"/> class.
        /// </summary>
        /// <param name="equitiesParameters">
        /// The equities parameters.
        /// </param>
        /// <param name="ruleContext">
        /// The rule context.
        /// </param>
        /// <param name="alertStream">
        /// The alert stream.
        /// </param>
        /// <param name="orderFilter">
        /// The order filter.
        /// </param>
        /// <param name="marketCacheFactory">
        /// The market cache factory.
        /// </param>
        /// <param name="runMode">
        /// The run mode.
        /// </param>
        /// <param name="portfolioFactory">
        /// The portfolio factory.
        /// </param>
        /// <param name="analysisService">
        /// The analysis service.
        /// </param>
        /// <param name="logger">
        /// The logger.
        /// </param>
        /// <param name="tradingHistoryLogger">
        /// The trading history logger.
        /// </param>
        public SpoofingRule(
            ISpoofingRuleEquitiesParameters equitiesParameters,
            ISystemProcessOperationRunRuleContext ruleContext,
            IUniverseAlertStream alertStream,
            IUniverseOrderFilter orderFilter,
            IUniverseMarketCacheFactory marketCacheFactory,
            RuleRunMode runMode,
            IPortfolioFactory portfolioFactory,
            IOrderAnalysisService analysisService,
            ILogger logger,
            ILogger<TradingHistoryStack> tradingHistoryLogger)
            : base(
                equitiesParameters?.Windows?.BackwardWindowSize ?? TimeSpan.FromMinutes(30),
                  equitiesParameters?.Windows?.BackwardWindowSize ?? TimeSpan.FromMinutes(30),
                equitiesParameters?.Windows?.FutureWindowSize ?? TimeSpan.Zero,
                Rules.Spoofing,
                EquityRuleSpoofingFactory.Version,
                "Spoofing Rule",
                ruleContext,
                marketCacheFactory,
                runMode,
                logger,
                tradingHistoryLogger)
        {
            this.equitiesParameters =
                equitiesParameters ?? throw new ArgumentNullException(nameof(equitiesParameters));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.alertStream = alertStream ?? throw new ArgumentNullException(nameof(alertStream));
            this.orderFilter = orderFilter ?? throw new ArgumentNullException(nameof(orderFilter));
            this.portfolioFactory = portfolioFactory ?? throw new ArgumentNullException(nameof(portfolioFactory));
            this.analysisService = analysisService ?? throw new ArgumentNullException(nameof(analysisService));
            this.ruleContext = ruleContext ?? throw new ArgumentNullException(nameof(ruleContext));
        }

        /// <summary>
        /// Gets or sets the organization factor value.
        /// </summary>
        public IFactorValue OrganisationFactorValue { get; set; } = FactorValue.None;

        /// <summary>
        /// The clone.
        /// </summary>
        /// <param name="factor">
        /// The factor.
        /// </param>
        /// <returns>
        /// The <see cref="IUniverseCloneableRule"/>.
        /// </returns>
        public IUniverseCloneableRule Clone(IFactorValue factor)
        {
            var clone = (SpoofingRule)this.Clone();
            clone.OrganisationFactorValue = factor;

            return clone;
        }

        /// <summary>
        /// The clone.
        /// </summary>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        public object Clone()
        {
            var clone = (SpoofingRule)this.MemberwiseClone();
            clone.BaseClone();

            return clone;
        }

        /// <summary>
        /// The run order filled event.
        /// </summary>
        /// <param name="history">
        /// The history.
        /// </param>
        public override void RunOrderFilledEvent(ITradingHistoryStack history)
        {
            // spoofing rule does not monitor by filled orders
        }

        /// <summary>
        /// The run order filled event delayed.
        /// </summary>
        /// <param name="history">
        /// The history.
        /// </param>
        public override void RunOrderFilledEventDelayed(ITradingHistoryStack history)
        {
            // do nothing
        }

        /// <summary>
        /// The data constraints.
        /// </summary>
        /// <returns>
        /// The <see cref="IRuleDataConstraint"/>.
        /// </returns>
        public override IRuleDataConstraint DataConstraints()
        {
            if (this.equitiesParameters == null)
            {
                return RuleDataConstraint.Empty().Case;
            }

            var intradayConstraint = new RuleDataSubConstraint(
                this.ForwardWindowSize,
                this.TradeBackwardWindowSize,
                DataSource.AnyIntraday,
                _ => !this.orderFilter.Filter(_));

            var interdayConstraint = new RuleDataSubConstraint(
                this.ForwardWindowSize,
                this.TradeBackwardWindowSize,
                DataSource.AnyInterday,
                _ => !this.orderFilter.Filter(_));

            return new RuleDataConstraint(
                this.Rule,
                this.equitiesParameters.Id,
                new[] { intradayConstraint, interdayConstraint });
        }

        /// <summary>
        /// The end of universe.
        /// </summary>
        protected override void EndOfUniverse()
        {
            this.logger.LogInformation("Eschaton occured");
            this.ruleContext?.EndEvent();
        }

        /// <summary>
        /// The filter.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <returns>
        /// The <see cref="IUniverseEvent"/>.
        /// </returns>
        protected override IUniverseEvent Filter(IUniverseEvent value)
        {
            return this.orderFilter.Filter(value);
        }

        /// <summary>
        /// The genesis.
        /// </summary>
        protected override void Genesis()
        {
            this.logger.LogInformation("Genesis occurred");
        }

        /// <summary>
        /// The market close.
        /// </summary>
        /// <param name="exchange">
        /// The exchange.
        /// </param>
        protected override void MarketClose(MarketOpenClose exchange)
        {
            this.logger.LogInformation($"Market Close ({exchange?.MarketId}) occurred at {exchange?.MarketClose}");
        }

        /// <summary>
        /// The market open.
        /// </summary>
        /// <param name="exchange">
        /// The exchange.
        /// </param>
        protected override void MarketOpen(MarketOpenClose exchange)
        {
            this.logger.LogInformation($"Market Open ({exchange?.MarketId}) occurred at {exchange?.MarketOpen}");
        }

        /// <summary>
        /// The run initial submission event.
        /// </summary>
        /// <param name="history">
        /// The history.
        /// </param>
        protected override void RunInitialSubmissionEvent(ITradingHistoryStack history)
        {
            var activeTrades = history?.ActiveTradeHistory();
            var portfolio = this.portfolioFactory.Build();
            portfolio.Add(activeTrades);

            var lastTrade = history?.ActiveTradeHistory()?.Any() ?? false
                                ? history?.ActiveTradeHistory()?.Peek()
                                : null;

            if (lastTrade == null)
            {
                return;
            } 

            if (lastTrade.OrderStatus() != OrderStatus.Filled)
            {
                this.logger.LogInformation("Order under analysis was not in filled state, exiting spoofing rule");
                return;
            }

            var lastTradeSentiment = this.analysisService.ResolveSentiment(lastTrade);
            var otherTrades = activeTrades.Where(i => i != lastTrade).ToList();
            var orderLedgerSentiment = this.analysisService.ResolveSentiment(otherTrades);

            if (lastTradeSentiment == orderLedgerSentiment)
            {
                this.logger.LogInformation("Order under analysis was consistent with a priori pricing sentiment");
                return;
            }

            if (lastTradeSentiment == PriceSentiment.Neutral)
            {
                this.logger.LogInformation("Order under analysis was considered price neutral on sentiment");
                return;
            }

            var analyzedOrders = this.analysisService.AnalyseOrder(activeTrades);
            var alignedSentimentPortfolio = this.AlignedSentimentPortfolio(analyzedOrders, lastTradeSentiment);
            var unalignedSentimentPortfolio = this.UnalignedSentimentPortfolio(analyzedOrders, lastTradeSentiment);

            if (!this.UnalignedPortfolioOverCancellationThreshold(unalignedSentimentPortfolio))
            {
                return;
            }

            if (!this.CancellationVolumeOverThreshold(alignedSentimentPortfolio, unalignedSentimentPortfolio))
            {
                return;
            }

            this.logger.LogInformation(
                $"Rule breach for {lastTrade?.Instrument?.Identifiers} at {this.UniverseDateTime}. Passing to alert stream.");
            this.RecordRuleBreach(lastTrade, alignedSentimentPortfolio, unalignedSentimentPortfolio);
        }

        /// <summary>
        /// The run initial submission event delayed.
        /// </summary>
        /// <param name="history">
        /// The history.
        /// </param>
        protected override void RunInitialSubmissionEventDelayed(ITradingHistoryStack history)
        {
            // do nothing
        }

        /// <summary>
        /// The run post order event.
        /// </summary>
        /// <param name="history">
        /// The history.
        /// </param>
        protected override void RunPostOrderEvent(ITradingHistoryStack history)
        {
            // spoofing rule does not monitor by last status changed
        }

        /// <summary>
        /// The run post order event delayed.
        /// </summary>
        /// <param name="history">
        /// The history.
        /// </param>
        protected override void RunPostOrderEventDelayed(ITradingHistoryStack history)
        {
            // do nothing
        }

        /// <summary>
        /// The aligned sentiment portfolio.
        /// </summary>
        /// <param name="analyzedOrders">
        /// The analyzed orders.
        /// </param>
        /// <param name="lastTradeSentiment">
        /// The last trade sentiment.
        /// </param>
        /// <returns>
        /// The <see cref="IPortfolio"/>.
        /// </returns>
        private IPortfolio AlignedSentimentPortfolio(
            IReadOnlyCollection<IOrderAnalysis> analyzedOrders,
            PriceSentiment lastTradeSentiment)
        {
            var alignedSentiment = analyzedOrders.Where(i => i.Sentiment == lastTradeSentiment).ToList();
            var alignedSentimentPortfolio = this.portfolioFactory.Build();
            alignedSentimentPortfolio.Add(alignedSentiment.Select(i => i.Order).ToList());

            return alignedSentimentPortfolio;
        }

        /// <summary>
        /// The cancellation volume over threshold.
        /// </summary>
        /// <param name="alignedSentimentPortfolio">
        /// The aligned sentiment portfolio.
        /// </param>
        /// <param name="unalignedSentimentPortfolio">
        /// The unaligned sentiment portfolio.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        private bool CancellationVolumeOverThreshold(
            IPortfolio alignedSentimentPortfolio,
            IPortfolio unalignedSentimentPortfolio)
        {
            var alignedVolume = alignedSentimentPortfolio.Ledger.VolumeInLedgerWithStatus(OrderStatus.Filled);
            var opposingVolume = unalignedSentimentPortfolio.Ledger.VolumeInLedgerWithStatus(OrderStatus.Cancelled);

            if (alignedVolume <= 0 || opposingVolume <= 0)
            {
                this.logger.LogInformation(
                    "Order under analysis was considered to not be in breach of spoofing by volumes traded/cancelled");
                return false;
            }

            var scaleOfSpoofExceedingReal = opposingVolume / alignedVolume;

            return scaleOfSpoofExceedingReal >= this.equitiesParameters.RelativeSizeMultipleForSpoofExceedingReal;
        }

        /// <summary>
        /// The record rule breach.
        /// </summary>
        /// <param name="lastTrade">
        /// The last trade.
        /// </param>
        /// <param name="alignedSentiment">
        /// The aligned sentiment.
        /// </param>
        /// <param name="opposingSentiment">
        /// The opposing sentiment.
        /// </param>
        private void RecordRuleBreach(Order lastTrade, IPortfolio alignedSentiment, IPortfolio opposingSentiment)
        {
            this.logger.LogInformation($"rule breach detected for {lastTrade.Instrument?.Identifiers}");

            var tradingPosition = new TradePosition(alignedSentiment.Ledger.FullLedger().ToList());
            var opposingPosition = new TradePosition(opposingSentiment.Ledger.FullLedger().ToList());

            // wrong but should be a judgement anyway
            var ruleBreach = new SpoofingRuleBreach(
                this.OrganisationFactorValue,
                this.ruleContext.SystemProcessOperationContext(),
                this.ruleContext.CorrelationId(),
                this.equitiesParameters.Windows?.BackwardWindowSize ?? TimeSpan.FromMinutes(30),
                tradingPosition,
                opposingPosition,
                lastTrade.Instrument,
                lastTrade,
                this.equitiesParameters,
                null,
                null,
                this.UniverseDateTime);

            var alert = new UniverseAlertEvent(Rules.Spoofing, ruleBreach, this.ruleContext);
            this.alertStream.Add(alert);
        }

        /// <summary>
        /// The unaligned portfolio over cancellation threshold.
        /// </summary>
        /// <param name="unalignedSentimentPortfolio">
        /// The unaligned sentiment portfolio.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        private bool UnalignedPortfolioOverCancellationThreshold(IPortfolio unalignedSentimentPortfolio)
        {
            var percentageByOrderBreach = this.equitiesParameters.CancellationThreshold
                                          <= unalignedSentimentPortfolio.Ledger.PercentageInStatusByOrder(
                                              OrderStatus.Cancelled);

            var percentageByVolumeBreach = this.equitiesParameters.CancellationThreshold
                                           <= unalignedSentimentPortfolio.Ledger.PercentageInStatusByVolume(
                                               OrderStatus.Cancelled);

            return percentageByOrderBreach || percentageByVolumeBreach;
        }

        /// <summary>
        /// The unaligned sentiment portfolio.
        /// </summary>
        /// <param name="analyzedOrders">
        /// The analyzed orders.
        /// </param>
        /// <param name="lastTradeSentiment">
        /// The last trade sentiment.
        /// </param>
        /// <returns>
        /// The <see cref="IPortfolio"/>.
        /// </returns>
        private IPortfolio UnalignedSentimentPortfolio(
            IReadOnlyCollection<IOrderAnalysis> analyzedOrders,
            PriceSentiment lastTradeSentiment)
        {
            var opposingSentiment = this.analysisService.OpposingSentiment(analyzedOrders, lastTradeSentiment);
            var opposingSentimentPortfolio = this.portfolioFactory.Build();
            opposingSentimentPortfolio.Add(opposingSentiment.Select(i => i.Order).ToList());

            return opposingSentimentPortfolio;
        }
    }
}