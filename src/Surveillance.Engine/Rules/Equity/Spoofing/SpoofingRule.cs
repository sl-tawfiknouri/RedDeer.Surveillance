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
            IUniverseMarketCacheFactory factory,
            RuleRunMode runMode,
            IPortfolioFactory portfolioFactory,
            IOrderAnalysisService analysisService,
            ILogger logger,
            ILogger<TradingHistoryStack> tradingHistoryLogger)
            : base(
                  equitiesParameters?.WindowSize ?? TimeSpan.FromMinutes(30),
                  Domain.Surveillance.Scheduling.Rules.Spoofing,
                  EquityRuleSpoofingFactory.Version,
                  "Spoofing Rule",
                  ruleCtx,
                  factory,
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

        protected override void RunInitialSubmissionRule(ITradingHistoryStack history)
        {
            var activeTrades = history?.ActiveTradeHistory();
            var portfolio = _portfolioFactory.Build();
            portfolio.Add(activeTrades);

            var lastTrade = history?.ActiveTradeHistory()?.Peek();
            if (lastTrade == null)
            {
                return;
            }

            var exposure = portfolio.TradingExposure.ExposureToInstrument(lastTrade.Instrument);
            if (exposure == null
                || !exposure.HasExposure)
            {
                _logger.LogInformation($"Portfolio not exposed to {lastTrade.Instrument.Identifiers.ToString()} at {UniverseDateTime}");
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

            var analysedOrders = _analysisService.AnalyseOrder(otherTrades);
            var opposingSentiment = _analysisService.OpposingSentiment(analysedOrders, lastTradeSentiment);
            var alignedSentiment = analysedOrders.Where(i => i.Sentiment == lastTradeSentiment).ToList();

            var alignedSentimentPortfolio = _portfolioFactory.Build();
            alignedSentimentPortfolio.Add(alignedSentiment.Select(i => i.Order).ToList());

            var opposingSentimentPortfolio = _portfolioFactory.Build();
            opposingSentimentPortfolio.Add(opposingSentiment.Select(i => i.Order).ToList());

            var percentageByOrderBreach =
                _equitiesParameters.CancellationThreshold <= opposingSentimentPortfolio.Ledger.PercentageInStatusByOrder(OrderStatus.Cancelled);

            var percentageByVolumeBreach =
                _equitiesParameters.CancellationThreshold <= opposingSentimentPortfolio.Ledger.PercentageInStatusByVolume(OrderStatus.Cancelled);

            if (!percentageByOrderBreach
                && !percentageByVolumeBreach)
            {
                _logger.LogInformation($"Order under analysis was considered to not be in breach of spoofing by cancellation ratios");
                return;
            }

            var alignedVolume = alignedSentimentPortfolio.Ledger.VolumeInLedgerWithStatus(OrderStatus.Filled);
            var opposingVolume = opposingSentimentPortfolio.Ledger.VolumeInLedgerWithStatus(OrderStatus.Cancelled);

            if (alignedVolume <= 0
                || opposingVolume <= 0)
            {
                _logger.LogInformation($"Order under analysis was considered to not be in breach of spoofing by volumes traded/cancelled");
                return;
            }

            var scaleOfSpoofExceedingReal = (decimal)opposingVolume / (decimal)alignedVolume;
            if (scaleOfSpoofExceedingReal < _equitiesParameters.RelativeSizeMultipleForSpoofExceedingReal)
            {
                _logger.LogInformation($"Order under analysis had high cancellations but did not exceed the spoofing scale");
                return;
            }

            _logger.LogInformation($"Rule breach for {lastTrade?.Instrument?.Identifiers} at {UniverseDateTime}. Passing to alert stream.");
            RecordRuleBreach(lastTrade, alignedSentimentPortfolio, opposingSentimentPortfolio);

            // O L D    C O D E

            //var mostRecentTrade = activeTrades.Pop();

            //var buyPosition =
            //    new TradePositionCancellations(
            //        new List<Order>(),
            //        _equitiesParameters.CancellationThreshold,
            //        _equitiesParameters.CancellationThreshold,
            //        _logger);

            //var sellPosition =
            //    new TradePositionCancellations(
            //        new List<Order>(),
            //        _equitiesParameters.CancellationThreshold,
            //        _equitiesParameters.CancellationThreshold,
            //        _logger);

            //AddToPositions(buyPosition, sellPosition, mostRecentTrade);

            //var tradingPosition =
            //   (mostRecentTrade.OrderDirection == OrderDirections.BUY
            //    || mostRecentTrade.OrderDirection == OrderDirections.COVER)
            //        ? buyPosition
            //        : sellPosition;

            //var opposingPosition =
            //    (mostRecentTrade.OrderDirection == OrderDirections.SELL
            //     || mostRecentTrade.OrderDirection == OrderDirections.SHORT)
            //        ? buyPosition
            //        : sellPosition;

            //var hasBreachedSpoofingRule = CheckPositionForSpoofs(activeTrades, buyPosition, sellPosition, tradingPosition, opposingPosition);

            //if (hasBreachedSpoofingRule)
            //{
            //    _logger.LogInformation($"RunInitialSubmissionRule had a rule breach for {mostRecentTrade?.Instrument?.Identifiers} at {UniverseDateTime}. Passing to alert stream.");
            //    RecordRuleBreach(mostRecentTrade, tradingPosition, opposingPosition);
            //}
        }
        
        private bool CheckPositionForSpoofs(
            Stack<Order> tradeWindow,
            ITradePositionCancellations buyPosition,
            ITradePositionCancellations sellPosition,
            ITradePositionCancellations tradingPosition,
            ITradePositionCancellations opposingPosition)
        {
            var hasBreachedSpoofingRule = false;
            var hasTradesInWindow = tradeWindow.Any();

            if (!tradeWindow.Any())
            {
                // ReSharper disable once RedundantAssignment
                hasTradesInWindow = false;
                return false;
            }

            foreach (var trade in tradeWindow.ToList())
            {
                AddToPositions(buyPosition, sellPosition, trade);
            }

            if (!opposingPosition.HighCancellationRatioByPositionSize() &&
                !opposingPosition.HighCancellationRatioByTradeCount())
            {
                return false;
            }

            var adjustedFulfilledOrders =
                (tradingPosition.VolumeInStatus(OrderStatus.Filled)
                 * _equitiesParameters.RelativeSizeMultipleForSpoofExceedingReal);

            var opposedOrders = opposingPosition.VolumeInStatus(OrderStatus.Cancelled);
            hasBreachedSpoofingRule = hasBreachedSpoofingRule || adjustedFulfilledOrders <= opposedOrders;

            return hasBreachedSpoofingRule;
        }

        private void AddToPositions(ITradePositionCancellations buyPosition, ITradePositionCancellations sellPosition, Order nextTrade)
        {
            switch (nextTrade.OrderDirection)
            {
                case OrderDirections.BUY:
                case OrderDirections.COVER:
                    buyPosition.Add(nextTrade);
                    break;
                case OrderDirections.SELL:
                case OrderDirections.SHORT:
                    sellPosition.Add(nextTrade);
                    break;
                default:
                    _logger.LogError("not considering an out of range order direction");
                    _ruleCtx.EventException("not considering an out of range order direction");
                    throw new ArgumentOutOfRangeException(nameof(nextTrade));
            }
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
                    _equitiesParameters.WindowSize,
                    tradingPosition,
                    opposingPosition,
                    lastTrade.Instrument, 
                    lastTrade,
                    _equitiesParameters);

            var alert = new UniverseAlertEvent(Domain.Surveillance.Scheduling.Rules.Spoofing, ruleBreach, _ruleCtx);
            _alertStream.Add(alert);
        }

        protected override void RunPostOrderEvent(ITradingHistoryStack history)
        {
            // spoofing rule does not monitor by last status changed
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