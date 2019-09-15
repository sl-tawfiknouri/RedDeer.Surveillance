namespace Surveillance.Engine.Rules.Rules.Equity.Spoofing
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Domain.Core.Trading.Execution;
    using Domain.Core.Trading.Execution.Interfaces;
    using Domain.Core.Trading.Factories.Interfaces;
    using Domain.Core.Trading.Interfaces;
    using Domain.Core.Trading.Orders;
    using Domain.Surveillance.Scheduling;

    using Microsoft.Extensions.Logging;

    using Surveillance.Auditing.Context.Interfaces;
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
    using Surveillance.Engine.Rules.Universe.Interfaces;
    using Surveillance.Engine.Rules.Universe.MarketEvents;

    public class SpoofingRule : BaseUniverseRule, ISpoofingRule
    {
        private readonly IUniverseAlertStream _alertStream;

        private readonly IOrderAnalysisService _analysisService;

        private readonly ISpoofingRuleEquitiesParameters _equitiesParameters;

        private readonly ILogger _logger;

        private readonly IUniverseOrderFilter _orderFilter;

        private readonly IPortfolioFactory _portfolioFactory;

        private readonly ISystemProcessOperationRunRuleContext _ruleCtx;

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
            this._equitiesParameters =
                equitiesParameters ?? throw new ArgumentNullException(nameof(equitiesParameters));
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this._alertStream = alertStream ?? throw new ArgumentNullException(nameof(alertStream));
            this._orderFilter = orderFilter ?? throw new ArgumentNullException(nameof(orderFilter));
            this._portfolioFactory = portfolioFactory ?? throw new ArgumentNullException(nameof(portfolioFactory));
            this._analysisService = analysisService ?? throw new ArgumentNullException(nameof(analysisService));
            this._ruleCtx = ruleContext ?? throw new ArgumentNullException(nameof(ruleContext));
        }

        public IFactorValue OrganisationFactorValue { get; set; } = FactorValue.None;

        public IUniverseCloneableRule Clone(IFactorValue factor)
        {
            var clone = (SpoofingRule)this.Clone();
            clone.OrganisationFactorValue = factor;

            return clone;
        }

        public object Clone()
        {
            var clone = (SpoofingRule)this.MemberwiseClone();
            clone.BaseClone();

            return clone;
        }

        public override void RunOrderFilledEvent(ITradingHistoryStack history)
        {
            // spoofing rule does not monitor by filled orders
        }

        public override void RunOrderFilledEventDelayed(ITradingHistoryStack history)
        {
            // do nothing
        }

        protected override void EndOfUniverse()
        {
            this._logger.LogInformation("Eschaton occured");
            this._ruleCtx?.EndEvent();
        }

        protected override IUniverseEvent Filter(IUniverseEvent value)
        {
            return this._orderFilter.Filter(value);
        }

        protected override void Genesis()
        {
            this._logger.LogInformation("Genesis occurred");
        }

        protected override void MarketClose(MarketOpenClose exchange)
        {
            this._logger.LogInformation($"Market Close ({exchange?.MarketId}) occurred at {exchange?.MarketClose}");
        }

        protected override void MarketOpen(MarketOpenClose exchange)
        {
            this._logger.LogInformation($"Market Open ({exchange?.MarketId}) occurred at {exchange?.MarketOpen}");
        }

        protected override void RunInitialSubmissionEvent(ITradingHistoryStack history)
        {
            var activeTrades = history?.ActiveTradeHistory();
            var portfolio = this._portfolioFactory.Build();
            portfolio.Add(activeTrades);

            var lastTrade = history?.ActiveTradeHistory()?.Any() ?? false
                                ? history?.ActiveTradeHistory()?.Peek()
                                : null;
            if (lastTrade == null) return;

            if (lastTrade.OrderStatus() != OrderStatus.Filled)
            {
                this._logger.LogInformation("Order under analysis was not in filled state, exiting spoofing rule");
                return;
            }

            var lastTradeSentiment = this._analysisService.ResolveSentiment(lastTrade);
            var otherTrades = activeTrades.Where(i => i != lastTrade).ToList();
            var orderLedgerSentiment = this._analysisService.ResolveSentiment(otherTrades);

            if (lastTradeSentiment == orderLedgerSentiment)
            {
                this._logger.LogInformation("Order under analysis was consistent with a priori pricing sentiment");
                return;
            }

            if (lastTradeSentiment == PriceSentiment.Neutral)
            {
                this._logger.LogInformation("Order under analysis was considered price neutral on sentiment");
                return;
            }

            var analysedOrders = this._analysisService.AnalyseOrder(activeTrades);
            var alignedSentimentPortfolio = this.AlignedSentimentPortfolio(analysedOrders, lastTradeSentiment);
            var unalignedSentimentPortfolio = this.UnalignedSentimentPortfolio(analysedOrders, lastTradeSentiment);

            if (!this.UnalignedPortfolioOverCancellationThreshold(unalignedSentimentPortfolio)) return;

            if (!this.CancellationVolumeOverThreshold(alignedSentimentPortfolio, unalignedSentimentPortfolio)) return;

            this._logger.LogInformation(
                $"Rule breach for {lastTrade?.Instrument?.Identifiers} at {this.UniverseDateTime}. Passing to alert stream.");
            this.RecordRuleBreach(lastTrade, alignedSentimentPortfolio, unalignedSentimentPortfolio);
        }

        protected override void RunInitialSubmissionEventDelayed(ITradingHistoryStack history)
        {
            // do nothing
        }

        protected override void RunPostOrderEvent(ITradingHistoryStack history)
        {
            // spoofing rule does not monitor by last status changed
        }

        protected override void RunPostOrderEventDelayed(ITradingHistoryStack history)
        {
            // do nothing
        }

        private IPortfolio AlignedSentimentPortfolio(
            IReadOnlyCollection<IOrderAnalysis> analysedOrders,
            PriceSentiment lastTradeSentiment)
        {
            var alignedSentiment = analysedOrders.Where(i => i.Sentiment == lastTradeSentiment).ToList();
            var alignedSentimentPortfolio = this._portfolioFactory.Build();
            alignedSentimentPortfolio.Add(alignedSentiment.Select(i => i.Order).ToList());

            return alignedSentimentPortfolio;
        }

        private bool CancellationVolumeOverThreshold(
            IPortfolio alignedSentimentPortfolio,
            IPortfolio unalignedSentimentPortfolio)
        {
            var alignedVolume = alignedSentimentPortfolio.Ledger.VolumeInLedgerWithStatus(OrderStatus.Filled);
            var opposingVolume = unalignedSentimentPortfolio.Ledger.VolumeInLedgerWithStatus(OrderStatus.Cancelled);

            if (alignedVolume <= 0 || opposingVolume <= 0)
            {
                this._logger.LogInformation(
                    "Order under analysis was considered to not be in breach of spoofing by volumes traded/cancelled");
                return false;
            }

            var scaleOfSpoofExceedingReal = opposingVolume / alignedVolume;

            return scaleOfSpoofExceedingReal >= this._equitiesParameters.RelativeSizeMultipleForSpoofExceedingReal;
        }

        private void RecordRuleBreach(Order lastTrade, IPortfolio alignedSentiment, IPortfolio opposingSentiment)
        {
            this._logger.LogInformation($"rule breach detected for {lastTrade.Instrument?.Identifiers}");

            var tradingPosition = new TradePosition(alignedSentiment.Ledger.FullLedger().ToList());
            var opposingPosition = new TradePosition(opposingSentiment.Ledger.FullLedger().ToList());

            // wrong but should be a judgement anyway
            var ruleBreach = new SpoofingRuleBreach(
                this.OrganisationFactorValue,
                this._ruleCtx.SystemProcessOperationContext(),
                this._ruleCtx.CorrelationId(),
                this._equitiesParameters.Windows?.BackwardWindowSize ?? TimeSpan.FromMinutes(30),
                tradingPosition,
                opposingPosition,
                lastTrade.Instrument,
                lastTrade,
                this._equitiesParameters,
                null,
                null,
                this.UniverseDateTime);

            var alert = new UniverseAlertEvent(Rules.Spoofing, ruleBreach, this._ruleCtx);
            this._alertStream.Add(alert);
        }

        private bool UnalignedPortfolioOverCancellationThreshold(IPortfolio unalignedSentimentPortfolio)
        {
            var percentageByOrderBreach = this._equitiesParameters.CancellationThreshold
                                          <= unalignedSentimentPortfolio.Ledger.PercentageInStatusByOrder(
                                              OrderStatus.Cancelled);

            var percentageByVolumeBreach = this._equitiesParameters.CancellationThreshold
                                           <= unalignedSentimentPortfolio.Ledger.PercentageInStatusByVolume(
                                               OrderStatus.Cancelled);

            return percentageByOrderBreach || percentageByVolumeBreach;
        }

        private IPortfolio UnalignedSentimentPortfolio(
            IReadOnlyCollection<IOrderAnalysis> analysedOrders,
            PriceSentiment lastTradeSentiment)
        {
            var opposingSentiment = this._analysisService.OpposingSentiment(analysedOrders, lastTradeSentiment);
            var opposingSentimentPortfolio = this._portfolioFactory.Build();
            opposingSentimentPortfolio.Add(opposingSentiment.Select(i => i.Order).ToList());

            return opposingSentimentPortfolio;
        }
    }
}