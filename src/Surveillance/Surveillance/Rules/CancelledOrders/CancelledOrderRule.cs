using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Trades.Orders;
using Microsoft.Extensions.Logging;
using Surveillance.Analytics.Streams;
using Surveillance.Analytics.Streams.Interfaces;
using Surveillance.RuleParameters.Interfaces;
using Surveillance.Rules.CancelledOrders.Interfaces;
using Surveillance.System.Auditing.Context.Interfaces;
using Surveillance.Trades;
using Surveillance.Trades.Interfaces;
using Surveillance.Universe.MarketEvents;

namespace Surveillance.Rules.CancelledOrders
{
    public class CancelledOrderRule : BaseUniverseRule, ICancelledOrderRule
    {
        private readonly ICancelledOrderRuleParameters _parameters;
        private readonly ISystemProcessOperationRunRuleContext _opCtx;
        private readonly IUniverseAlertStream _alertStream;

        private readonly ILogger _logger;
        
        public CancelledOrderRule(
            ICancelledOrderRuleParameters parameters,
            ISystemProcessOperationRunRuleContext opCtx,
            IUniverseAlertStream alertStream,
            ILogger<CancelledOrderRule> logger)
            : base(
                parameters?.WindowSize ?? TimeSpan.FromMinutes(60),
                DomainV2.Scheduling.Rules.CancelledOrders,
                Versioner.Version(2, 0),
                "Cancelled Order Rule",
                opCtx,
                logger)
        {
            _parameters = parameters ?? throw new ArgumentNullException(nameof(parameters));
            _opCtx = opCtx ?? throw new ArgumentNullException(nameof(opCtx));
            _alertStream = alertStream ?? throw new ArgumentNullException(nameof(alertStream));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        protected override void RunRule(ITradingHistoryStack history)
        {
            var tradeWindow = history?.ActiveTradeHistory();

            if (tradeWindow == null
                || !tradeWindow.Any())
            {
                return;
            }

            var mostRecentTrade = tradeWindow.Pop();

            var tradingPosition =
                new TradePositionCancellations(
                    new List<TradeOrderFrame>(),
                    _parameters.CancelledOrderPercentagePositionThreshold,
                    _parameters.CancelledOrderCountPercentageThreshold,
                    _logger);

            tradingPosition.Add(mostRecentTrade);
            var ruleBreach = CheckPositionForCancellations(tradeWindow, mostRecentTrade, tradingPosition);

            if (ruleBreach.HasBreachedRule())
            {
                var message = new UniverseAlertEvent(DomainV2.Scheduling.Rules.CancelledOrders, ruleBreach, _opCtx);
                _alertStream.Add(message);
            }
        }

        private ICancelledOrderRuleBreach CheckPositionForCancellations(
            Stack<TradeOrderFrame> tradeWindow,
            TradeOrderFrame mostRecentTrade,
            ITradePositionCancellations tradingPosition)
        {
            var hasBreachedRuleByOrderCount = false;
            var hasBreachedRuleByPositionSize = false;
            var cancellationRatioByOrderCount = 0m;
            var cancellationRatioByPositionSize = 0m;

            var hasTradesInWindow = tradeWindow.Any();

            // ReSharper disable once LoopVariableIsNeverChangedInsideLoop
            while (hasTradesInWindow)
            {
                if (!tradeWindow.Any())
                {
                    // ReSharper disable once RedundantAssignment
                    hasTradesInWindow = false;
                    break;
                }

                var nextTrade = tradeWindow.Pop();
                if (nextTrade.Position != mostRecentTrade.Position)
                {
                    continue;
                }

                tradingPosition.Add(nextTrade);

                if (_parameters.MinimumNumberOfTradesToApplyRuleTo > tradingPosition.Get().Count
                    || (_parameters.MaximumNumberOfTradesToApplyRuleTo.HasValue
                        && _parameters.MaximumNumberOfTradesToApplyRuleTo.Value < tradingPosition.Get().Count))
                {
                    continue;
                }

                if (_parameters.CancelledOrderCountPercentageThreshold != null
                    && tradingPosition.HighCancellationRatioByTradeCount())
                {
                    hasBreachedRuleByOrderCount = true;
                    cancellationRatioByOrderCount = tradingPosition.CancellationRatioByTradeCount();
                }

                if (_parameters.CancelledOrderPercentagePositionThreshold != null
                    && tradingPosition.HighCancellationRatioByPositionSize())
                {
                    hasBreachedRuleByPositionSize = true;
                    cancellationRatioByPositionSize = tradingPosition.CancellationRatioByPositionSize();
                }
            }

            var cancelledPositionOrders = tradingPosition.Get().Count(tp =>
                tp.OrderStatus == OrderStatus.Cancelled
                || tp.OrderStatus == OrderStatus.CancelledPostBooking);

            var totalPositionOrders = tradingPosition.Get().Count;

            return new CancelledOrderRuleBreach(
                _parameters,
                tradingPosition,
                tradingPosition?.Get()?.FirstOrDefault()?.Security,
                hasBreachedRuleByPositionSize,
                cancellationRatioByPositionSize,
                cancelledPositionOrders,
                totalPositionOrders,
                hasBreachedRuleByOrderCount,
                cancellationRatioByOrderCount);
        }

        protected override void RunInitialSubmissionRule(ITradingHistoryStack history)
        {
            // do nothing
        }

        protected override void Genesis()
        {
            _logger.LogInformation("Universe Genesis occurred in the Cancelled Order Rule");
        }

        protected override void MarketOpen(MarketOpenClose exchange)
        {
            _logger.LogInformation($"Trading Opened for exchange {exchange.MarketId} in the Cancelled Order Rule");
        }

        protected override void MarketClose(MarketOpenClose exchange)
        {
            _logger.LogInformation($"Trading closed for exchange {exchange.MarketId} in the Cancelled Order Rule.");
        }

        protected override void EndOfUniverse()
        {
            _logger.LogInformation("Universe Eschaton occurred in the Cancelled Order Rule");

            var alertMessage = new UniverseAlertEvent(DomainV2.Scheduling.Rules.CancelledOrders, null, _opCtx, true);
            _alertStream.Add(alertMessage);
            _opCtx?.EndEvent();
        }

        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}
