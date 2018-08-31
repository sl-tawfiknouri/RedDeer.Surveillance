using Domain.Equity.Trading.Orders;
using Microsoft.Extensions.Logging;
using Surveillance.Trades.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Surveillance.Trades
{
    /// <summary>
    /// Buy or Sell position on an security
    /// </summary>
    public class TradePosition : ITradePosition
    {
        private IList<TradeOrderFrame> _trades;
        private decimal _cancellationRatioPercentage;
        private ILogger _logger;

        /// <param name="percent">between 0 and 1</param>
        public TradePosition(
            IList<TradeOrderFrame> trades,
            decimal percentageThresholdForHighCancellationRatio,
            ILogger logger)
        {
            _trades = trades?.Where(trad => trad != null).ToList() ?? new List<TradeOrderFrame>();
            _cancellationRatioPercentage = percentageThresholdForHighCancellationRatio;
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public void Add(TradeOrderFrame item)
        {
            _trades.Add(item);
        }

        public bool HighCancellationRatioByTradeQuantity()
        {
            var cancelled = _trades.Count(trad => trad.OrderStatus == OrderStatus.Cancelled);
            var total = _trades.Count();

            if (cancelled == 0
                || total == 0)
            {
                return false;
            }

            var byTradeCancellationRatio = (cancelled / (decimal)total);

            return byTradeCancellationRatio >= _cancellationRatioPercentage;
        }

        public bool HighCancellationRatioByTradeSize()
        {
            var cancelledOrders =
                _trades
                .Where(trad => trad != null && trad.OrderStatus == OrderStatus.Cancelled)
                .ToList();

            var nonCancelledOrders =
                _trades
                .Where(trad => trad != null && trad.OrderStatus != OrderStatus.Cancelled)
                .ToList();

            if (cancelledOrders.Count == 0)
            {
                return false;
            }

            var cancelledOrderVolume = cancelledOrders.Sum(co => co.Volume);
            var nonCancelledOrderVolume = nonCancelledOrders.Sum(co => co.Volume);

            if (cancelledOrderVolume < 0
                || nonCancelledOrderVolume < 0)
            {
                _logger.LogError("Negative values for order volume in position. Check data integrity");
            }

            if (cancelledOrderVolume == 0)
            {
                return false;
            }

            if (nonCancelledOrderVolume == 0)
            {
                return true;
            }

            var cancelledOrderVolumeRatio =
                cancelledOrderVolume / (decimal)(cancelledOrderVolume + nonCancelledOrderVolume);

            return cancelledOrderVolumeRatio >= _cancellationRatioPercentage;
        }

        public int TotalVolume()
        {
            return _trades.Sum(trad => trad != null ? trad.Volume : 0);
        }

        public int VolumeInStatus(OrderStatus status)
        {
            return
                _trades
                .Where(trad => trad != null && trad.OrderStatus == status)
                .Sum(trad => trad.Volume);
        }

        public int VolumeNotInStatus(OrderStatus status)
        {
            return
                _trades
                .Where(trad => trad != null && trad.OrderStatus != status)
                .Sum(trad => trad.Volume);
        }
    }
}
