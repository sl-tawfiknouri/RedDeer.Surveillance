using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Core.Trading.Execution;
using Domain.Core.Trading.Execution.Interfaces;
using Domain.Core.Trading.Factories.Interfaces;
using Domain.Core.Trading.Interfaces;
using Microsoft.Extensions.Logging;
using Surveillance.Auditing.Context.Interfaces;
using Surveillance.Engine.Rules.Analytics.Streams;
using Surveillance.Engine.Rules.Analytics.Streams.Interfaces;
using Surveillance.Engine.Rules.Factories.Equities;
using Surveillance.Engine.Rules.Factories.Interfaces;
using Surveillance.Engine.Rules.RuleParameters.Equities.Interfaces;
using Surveillance.Engine.Rules.Rules.Interfaces;
using Surveillance.Engine.Rules.Rules.Equity.Spoofing.Interfaces;
using Surveillance.Engine.Rules.Trades;
using Surveillance.Engine.Rules.Trades.Interfaces;
using Surveillance.Engine.Rules.Universe.Filter.Interfaces;
using Surveillance.Engine.Rules.Universe.Interfaces;
using Surveillance.Engine.Rules.Universe.MarketEvents;
using Domain.Core.Trading.Orders;

namespace Surveillance.Engine.Rules.Rules.Equity.Spoofing
{
    public class SpoofingRule : BaseUniverseRule, ISpoofingRule
    {
        private readonly ISpoofingRuleEquitiesParameters _equitiesParameters;
        private readonly ISystemProcessOperationRunRuleContext _ruleCtx;
        private readonly IUniverseAlertStream _alertStream;
        private readonly IUniverseOrderFilter _orderFilter;
        private readonly IPortfolioFactory _portfolioFactory;
        private readonly IOrderAnalysisService _analysisService;
        private readonly ILogger _logger;

        public SpoofingRule(
            ISpoofingRuleEquitiesParameters equitiesParameters,
            ISystemProcessOperationRunRuleContext ruleCtx,
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
                  equitiesParameters?.Windows?.FutureWindowSize ?? TimeSpan.Zero,
                  Domain.Surveillance.Scheduling.Rules.Spoofing,
                  EquityRuleSpoofingFactory.Version,
                  "Spoofing Rule",
                  ruleCtx,
                  marketCacheFactory,
                  runMode,
                  logger,
                  tradingHistoryLogger)
        {
            _equitiesParameters = equitiesParameters ?? throw new ArgumentNullException(nameof(equitiesParameters));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _alertStream = alertStream ?? throw new ArgumentNullException(nameof(alertStream));
            _orderFilter = orderFilter ?? throw new ArgumentNullException(nameof(orderFilter));
            _portfolioFactory = portfolioFactory ?? throw new ArgumentNullException(nameof(portfolioFactory));
            _analysisService = analysisService ?? throw new ArgumentNullException(nameof(analysisService));
            _ruleCtx = ruleCtx ?? throw new ArgumentNullException(nameof(ruleCtx));
        }

        public IFactorValue OrganisationFactorValue { get; set; } = FactorValue.None;

        protected override IUniverseEvent Filter(IUniverseEvent value)
        {
            return _orderFilter.Filter(value);
        }

        protected override void RunInitialSubmissionEvent(ITradingHistoryStack history)
        {
            var activeTrades = history?.ActiveTradeHistory();
            var portfolio = _portfolioFactory.Build();
            portfolio.Add(activeTrades);

            var lastTrade = (history?.ActiveTradeHistory()?.Any() ?? false) ? history?.ActiveTradeHistory()?.Peek() : null;
            if (lastTrade == null)
            {
                return;
            }

            if (lastTrade.OrderStatus() != OrderStatus.Filled)
            {
                _logger.LogInformation($"Order under analysis was not in filled state, exiting spoofing rule");
                return;
            }
            
            var lastTradeSentiment = _analysisService.ResolveSentiment(lastTrade);
            var otherTrades = activeTrades.Where(i => i != lastTrade).ToList();
            var orderLedgerSentiment = _analysisService.ResolveSentiment(otherTrades);

            if (lastTradeSentiment == orderLedgerSentiment)
            {
                _logger.LogInformation($"Order under analysis was consistent with a priori pricing sentiment");
                return;
            }
            else if (lastTradeSentiment == PriceSentiment.Neutral)
            {
                _logger.LogInformation($"Order under analysis was considered price neutral on sentiment");
                return;
            }

            var analysedOrders = _analysisService.AnalyseOrder(activeTrades);
            var alignedSentimentPortfolio = AlignedSentimentPortfolio(analysedOrders, lastTradeSentiment);
            var unalignedSentimentPortfolio = UnalignedSentimentPortfolio(analysedOrders, lastTradeSentiment);

            if (!UnalignedPortfolioOverCancellationThreshold(unalignedSentimentPortfolio))
            {
                return;
            }

            if (!CancellationVolumeOverThreshold(alignedSentimentPortfolio, unalignedSentimentPortfolio))
            {
                return;
            }

            _logger.LogInformation($"Rule breach for {lastTrade?.Instrument?.Identifiers} at {UniverseDateTime}. Passing to alert stream.");
            RecordRuleBreach(lastTrade, alignedSentimentPortfolio, unalignedSentimentPortfolio);
        }

        private IPortfolio AlignedSentimentPortfolio(
            IReadOnlyCollection<IOrderAnalysis> analysedOrders,
            PriceSentiment lastTradeSentiment)
        {
            var alignedSentiment = analysedOrders.Where(i => i.Sentiment == lastTradeSentiment).ToList();
            var alignedSentimentPortfolio = _portfolioFactory.Build();
            alignedSentimentPortfolio.Add(alignedSentiment.Select(i => i.Order).ToList());

            return alignedSentimentPortfolio;
        }

        private IPortfolio UnalignedSentimentPortfolio(
            IReadOnlyCollection<IOrderAnalysis> analysedOrders,
            PriceSentiment lastTradeSentiment)
        {
            var opposingSentiment = _analysisService.OpposingSentiment(analysedOrders, lastTradeSentiment);
            var opposingSentimentPortfolio = _portfolioFactory.Build();
            opposingSentimentPortfolio.Add(opposingSentiment.Select(i => i.Order).ToList());

            return opposingSentimentPortfolio;
        }

        private bool UnalignedPortfolioOverCancellationThreshold(
            IPortfolio unalignedSentimentPortfolio)
        {
            var percentageByOrderBreach =
                _equitiesParameters.CancellationThreshold <= unalignedSentimentPortfolio.Ledger.PercentageInStatusByOrder(OrderStatus.Cancelled);

            var percentageByVolumeBreach =
                _equitiesParameters.CancellationThreshold <= unalignedSentimentPortfolio.Ledger.PercentageInStatusByVolume(OrderStatus.Cancelled);

            return percentageByOrderBreach || percentageByVolumeBreach;
        }

        private bool CancellationVolumeOverThreshold(
            IPortfolio alignedSentimentPortfolio,
            IPortfolio unalignedSentimentPortfolio)
        {
            var alignedVolume = alignedSentimentPortfolio.Ledger.VolumeInLedgerWithStatus(OrderStatus.Filled);
            var opposingVolume = unalignedSentimentPortfolio.Ledger.VolumeInLedgerWithStatus(OrderStatus.Cancelled);

            if (alignedVolume <= 0
                || opposingVolume <= 0)
            {
                _logger.LogInformation($"Order under analysis was considered to not be in breach of spoofing by volumes traded/cancelled");
                return false;
            }

            var scaleOfSpoofExceedingReal = (decimal)opposingVolume / (decimal)alignedVolume;

            return scaleOfSpoofExceedingReal >= _equitiesParameters.RelativeSizeMultipleForSpoofExceedingReal;
        }

        private void RecordRuleBreach(
            Order lastTrade,
            IPortfolio alignedSentiment,
            IPortfolio opposingSentiment)
        {
            _logger.LogInformation($"rule breach detected for {lastTrade.Instrument?.Identifiers}");

            var tradingPosition = new TradePosition(alignedSentiment.Ledger.FullLedger().ToList());
            var opposingPosition = new TradePosition(opposingSentiment.Ledger.FullLedger().ToList());

            var ruleBreach =
                new SpoofingRuleBreach(
                    OrganisationFactorValue,
                    _ruleCtx.SystemProcessOperationContext(),
                    _ruleCtx.CorrelationId(),
                    _equitiesParameters.Windows?.BackwardWindowSize ?? TimeSpan.FromMinutes(30),
                    tradingPosition,
                    opposingPosition,
                    lastTrade.Instrument, 
                    lastTrade,
                    _equitiesParameters,
                    UniverseDateTime);

            var alert = new UniverseAlertEvent(Domain.Surveillance.Scheduling.Rules.Spoofing, ruleBreach, _ruleCtx);
            _alertStream.Add(alert);
        }

        protected override void RunPostOrderEvent(ITradingHistoryStack history)
        {
            // spoofing rule does not monitor by last status changed
        }

        public override void RunOrderFilledEvent(ITradingHistoryStack history)
        {
            // spoofing rule does not monitor by filled orders
        }

        protected override void RunPostOrderEventDelayed(ITradingHistoryStack history)
        {
            // do nothing
        }

        protected override void RunInitialSubmissionEventDelayed(ITradingHistoryStack history)
        {
            // do nothing
        }

        public override void RunOrderFilledEventDelayed(ITradingHistoryStack history)
        {
            // do nothing
        }

        protected override void Genesis()
        {
            _logger.LogInformation("Genesis occurred");
        }

        protected override void MarketOpen(MarketOpenClose exchange)
        {
            _logger.LogInformation($"Market Open ({exchange?.MarketId}) occurred at {exchange?.MarketOpen}");
        }

        protected override void MarketClose(MarketOpenClose exchange)
        {
            _logger.LogInformation($"Market Close ({exchange?.MarketId}) occurred at {exchange?.MarketClose}");
        }

        protected override void EndOfUniverse()
        {
            _logger.LogInformation("Eschaton occured");
            _ruleCtx?.EndEvent();
        }

        public IUniverseCloneableRule Clone(IFactorValue factor)
        {
            var clone = (SpoofingRule)Clone();
            clone.OrganisationFactorValue = factor;

            return clone;
        }

        public object Clone()
        {
            var clone = (SpoofingRule)this.MemberwiseClone();
            clone.BaseClone();

            return clone;
        }
    }
}