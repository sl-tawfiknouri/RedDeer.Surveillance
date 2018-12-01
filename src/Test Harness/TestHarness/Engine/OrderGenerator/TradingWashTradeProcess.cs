using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Equity;
using Domain.Equity.Frames;
using Domain.Finance;
using Domain.Trades.Orders;
using NLog;
using TestHarness.Engine.OrderGenerator.Strategies.Interfaces;
using TestHarness.Engine.Plans;

namespace TestHarness.Engine.OrderGenerator
{
    public class TradingWashTradeProcess : BaseTradingProcess
    {
        private readonly DataGenerationPlan _plan;
        private readonly DateTime _thirdGroupActivation;
        private readonly object _lock = new object();
        private bool _initialCluster = false;
        private bool _secondaryCluster = false;
        private bool _thirdCluster = false;

        public TradingWashTradeProcess(
            ITradeStrategy<TradeOrderFrame> orderStrategy,
            DataGenerationPlan plan,
            ILogger logger)
            : base(logger, orderStrategy)
        {
            _plan = plan;
            _thirdGroupActivation = _plan.EquityInstructions.TerminationInUtc.AddHours(4);
        }

        protected override void _InitiateTrading()
        { }

        public override void OnNext(ExchangeFrame value)
        {
            if (value == null)
            {
                return;
            }

            if (_plan == null)
            {
                return;
            }

            lock (_lock)
            {
                if (!_initialCluster
                && _plan.EquityInstructions.CommencementInUtc <= value.TimeStamp)
                {
                    WashTradeInSecurityWithClustering(_plan.Sedol, value, 20);

                    _initialCluster = true;
                    return;
                }

                if (!_secondaryCluster
                    && _plan.EquityInstructions.TerminationInUtc <= value.TimeStamp)
                {
                    WashTradeInSecurityWithClustering(_plan.Sedol, value, 30);

                    _secondaryCluster = true;
                    return;
                }

                if (!_thirdCluster
                    && _thirdGroupActivation <= value.TimeStamp)
                {
                    WashTradeInSecurityWithClustering(_plan.Sedol, value, 20);

                    _thirdCluster = true;
                    return;
                }
            }
        }

        protected override void _TerminateTradingStrategy()
        {

        }

        private void WashTradeInSecurityWithClustering(string sedol, ExchangeFrame value, int clusterSize)
        {
            if (value == null)
            {
                return;
            }

            var correctSecurity =
                value
                    .Securities
                    .Where(sec =>
                        string.Equals(
                            sec.Security?.Identifiers.Sedol,
                            sedol,
                            StringComparison.InvariantCultureIgnoreCase))
                    .ToList();

            if (!correctSecurity.Any())
            {
                return;
            }

            var security = correctSecurity.FirstOrDefault();

            var splitSize = clusterSize / 2;
            var frames = new List<TradeOrderFrame>();
            for (var i = 0; i < clusterSize; i++)
            {
                var frame = new TradeOrderFrame(
                    null,
                    OrderType.Limit,
                    value.Exchange,
                    security.Security,
                    new CurrencyAmount(security.Spread.Price.Value * 1.05m, security.Spread.Price.Currency),
                    new CurrencyAmount(security.Spread.Price.Value * 1.05m, security.Spread.Price.Currency),
                    (int)(security.DailyVolume.Traded * 0.001m),
                    (int)(security.DailyVolume.Traded * 0.001m),
                    i < splitSize ? OrderPosition.Sell : OrderPosition.Buy,
                    OrderStatus.Fulfilled,
                    value.TimeStamp.AddSeconds(i),
                    value.TimeStamp,
                    null,
                    null,
                    null,
                    null,
                    null,
                    null,
                    null,
                    null,
                    security.Spread.Price.Currency.Value);

                frames.Add(frame);
            }

            foreach (var trade in frames.OrderBy(i => i.StatusChangedOn))
            {
                TradeStream.Add(trade);
            }
        }
    }
}
