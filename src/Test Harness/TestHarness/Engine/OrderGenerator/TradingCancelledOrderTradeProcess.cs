using System;
using System.Collections.Generic;
using System.Linq;
using DomainV2.Equity.Frames;
using DomainV2.Financial;
using DomainV2.Trading;
using Microsoft.Extensions.Logging;
using TestHarness.Engine.OrderGenerator.Strategies.Interfaces;

namespace TestHarness.Engine.OrderGenerator
{
    /// <summary>
    /// generate a bunch of cancelled orders (v2)
    /// </summary>
    public class TradingCancelledOrderTradeProcess : BaseTradingProcess
    {
        private readonly IReadOnlyCollection<string> _cancelTargetSedols;
        private readonly object _lock = new object();
        private readonly DateTime _triggerCount;
        private bool _hasProcessedCount;

        public TradingCancelledOrderTradeProcess(
            ITradeStrategy<Order> orderStrategy,
            IReadOnlyCollection<string> cancelTargetSedols,
            DateTime triggerCount,
            ILogger logger)
            : base(logger, orderStrategy)
        {
            _cancelTargetSedols = cancelTargetSedols ?? new string[0];
            _triggerCount = triggerCount;
        }

        protected override void _InitiateTrading()
        { }

        public override void OnNext(ExchangeFrame value)
        {
            if (value == null)
            {
                return;
            }

            if (_hasProcessedCount)
            {
                return;
            }

            lock (_lock)
            {
                if (_hasProcessedCount)
                {
                    return;
                }

                if (!_hasProcessedCount && value.TimeStamp >= _triggerCount)
                {
                    var i = 0;
                    foreach (var sedol in _cancelTargetSedols)
                    {
                        if (string.IsNullOrWhiteSpace(sedol))
                        {
                            continue;
                        }

                        switch (i)
                        {
                            case 0:
                                CancelForSedolByCount(sedol, value, 8);
                                break;
                            case 1:
                                CancelForSedolByPosition(sedol, value, 0.8m);
                                break;
                            case 2:
                                CancelForSedolByCount(sedol, value, 6);
                                break;
                            case 3:
                                CancelForSedolByPosition(sedol, value, 0.6m);
                                break;
                            case 4:
                                CancelForSedolByCount(sedol, value, 4);
                                break;
                            case 5:
                                CancelForSedolByPosition(sedol, value, 0.4m);
                                break;
                        }
                        i++;
                    }

                    _hasProcessedCount = true;
                }
            }
        }

        private void CancelForSedolByPosition(string sedol, ExchangeFrame value, decimal positionPercentageToCancel)
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
            var frames = new List<Order>();

            var totalPurchase = security.DailyVolume.Traded * 0.1m;
            var initialBuyShare = totalPurchase * positionPercentageToCancel;
            var splitShare = ((totalPurchase - initialBuyShare) * (1m / 9m)) - 1;

            var cancelledFrame = new Order(
                security.Security,
                security.Market,
                null,
                Guid.NewGuid().ToString(),
                value.TimeStamp,
                value.TimeStamp,
                null,
                null,
                value.TimeStamp,
                null,
                OrderTypes.MARKET,
                OrderPositions.BUY,
                new Currency("GBP"),
                new CurrencyAmount(security.Spread.Price.Value * 1.05m, security.Spread.Price.Currency),
                new CurrencyAmount(security.Spread.Price.Value * 1.05m, security.Spread.Price.Currency),
                (int)initialBuyShare,
                (int)initialBuyShare,
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

            frames.Add(cancelledFrame);

            for (var i = 1; i < 10; i++)
            {
                var frame = new Order(
                    security.Security,
                    security.Market,
                    null,
                    Guid.NewGuid().ToString(),
                    value.TimeStamp.AddMinutes(1),
                    value.TimeStamp.AddMinutes(1),
                    null,
                    null,
                    value.TimeStamp.AddMinutes(1),
                    null,
                    OrderTypes.MARKET,
                    OrderPositions.BUY,
                    new Currency("GBP"),
                    new CurrencyAmount(security.Spread.Price.Value * 1.05m, security.Spread.Price.Currency),
                    new CurrencyAmount(security.Spread.Price.Value * 1.05m, security.Spread.Price.Currency),
                    (int)splitShare,
                    (int)splitShare,
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
                
                frames.Add(frame);
            }

            foreach (var trade in frames.OrderBy(i => i.MostRecentDateEvent()))
            {
                TradeStream.Add(trade);
            }
        }

        private void CancelForSedolByCount(string sedol, ExchangeFrame value, int amountToCancel)
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

            var frames = new List<Order>();
            for (var i = 0; i < 10; i++)
            {
                var frame = new Order(
                    security.Security,
                    security.Market,
                    null,
                    Guid.NewGuid().ToString(),
                    value.TimeStamp.AddMinutes(1),
                    value.TimeStamp.AddMinutes(1),
                    null,
                    null,
                    i < amountToCancel ? (DateTime?)value.TimeStamp.AddMinutes(1) : null,
                    null,
                    OrderTypes.MARKET,
                    OrderPositions.BUY,
                    new Currency("GBP"),
                    new CurrencyAmount(security.Spread.Price.Value * 1.05m, security.Spread.Price.Currency),
                    new CurrencyAmount(security.Spread.Price.Value * 1.05m, security.Spread.Price.Currency),
                    i < amountToCancel ? 0 : (int)(security.DailyVolume.Traded * 0.01m),
                    0,
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

                frames.Add(frame);
            }

            foreach (var trade in frames.OrderBy(i => i.MostRecentDateEvent()))
            {
                TradeStream.Add(trade);
            }
        }
        
        protected override void _TerminateTradingStrategy()
        { }
    }
}
