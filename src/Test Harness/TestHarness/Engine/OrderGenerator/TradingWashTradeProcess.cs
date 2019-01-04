using System;
using System.Collections.Generic;
using System.Linq;
using DomainV2.Equity.TimeBars;
using DomainV2.Financial;
using DomainV2.Trading;
using Microsoft.Extensions.Logging;
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
            ITradeStrategy<Order> orderStrategy,
            DataGenerationPlan plan,
            ILogger logger)
            : base(logger, orderStrategy)
        {
            _plan = plan;
            _thirdGroupActivation = _plan.EquityInstructions.TerminationInUtc.AddHours(4);
        }

        protected override void _InitiateTrading()
        { }

        public override void OnNext(MarketTimeBarCollection value)
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
                && _plan.EquityInstructions.CommencementInUtc <= value.Epoch)
                {
                    WashTradeInSecurityWithClustering(_plan.Sedol, value, 20);

                    _initialCluster = true;
                    return;
                }

                if (!_secondaryCluster
                    && _plan.EquityInstructions.TerminationInUtc <= value.Epoch)
                {
                    WashTradeInSecurityWithClustering(_plan.Sedol, value, 30);

                    _secondaryCluster = true;
                    return;
                }

                if (!_thirdCluster
                    && _thirdGroupActivation <= value.Epoch)
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

        private void WashTradeInSecurityWithClustering(string sedol, MarketTimeBarCollection value, int clusterSize)
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
            var frames = new List<Order>();
            for (var i = 0; i < clusterSize; i++)
            {
                var frame2 = new Order(
                    security.Security,
                    security.Market,
                    null,
                    Guid.NewGuid().ToString(),
                    value.Epoch,
                    value.Epoch,
                    null,
                    null,
                    null,
                    value.Epoch.AddSeconds(i),
                    OrderTypes.LIMIT,
                    i < splitSize ? OrderPositions.SELL : OrderPositions.BUY,
                    new Currency("GBP"),
                    new CurrencyAmount(security.Spread.Price.Value * 1.05m, security.Spread.Price.Currency),
                    new CurrencyAmount(security.Spread.Price.Value * 1.05m, security.Spread.Price.Currency),
                    (int) (security.DailyVolume.Traded * 0.001m),
                    (int) (security.DailyVolume.Traded * 0.001m),
                    null,
                    null,
                    null,
                    null,
                    null,
                    null,
                    null,
                    null,
                    null,
                    new Trade[0]);

                frames.Add(frame2);
            }

            foreach (var trade in frames.OrderBy(i => i.MostRecentDateEvent()))
            {
                TradeStream.Add(trade);
            }
        }
    }
}
