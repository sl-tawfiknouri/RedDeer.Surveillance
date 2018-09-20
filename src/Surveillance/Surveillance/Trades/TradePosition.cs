using Microsoft.Extensions.Logging;
using Surveillance.Trades.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Trades.Orders;

namespace Surveillance.Trades
{
    /// <summary>
    /// Buy or Sell position on an security
    /// </summary>
    public class TradePosition : ITradePosition
    {
        private readonly IList<TradeOrderFrame> _trades;
        private readonly decimal? _cancellationRatioPercentagePosition;
        private readonly decimal? _cancellationRatioPercentageOrderCount;
        private readonly ILogger _logger;

        public TradePosition(
            IList<TradeOrderFrame> trades,
            decimal? cancellationRatioPercentagePosition,
            decimal? cancellationRatioPercentageOrderCount,
            ILogger logger)
        {
            _trades = trades?.Where(trad => trad != null).ToList() ?? new List<TradeOrderFrame>();
            _cancellationRatioPercentagePosition = cancellationRatioPercentagePosition;
            _cancellationRatioPercentageOrderCount = cancellationRatioPercentageOrderCount;
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public IList<TradeOrderFrame> Get()
        {
            return new List<TradeOrderFrame>(_trades);
        }

        public void Add(TradeOrderFrame item)
        {
            _trades.Add(item);
        }

        public bool HighCancellationRatioByTradeCount()
        {
            if (_cancellationRatioPercentageOrderCount == null)
            {
                return false;
            }

            var byTradeCancellationRatio = CancellationRatioByTradeCount();

            return byTradeCancellationRatio >= _cancellationRatioPercentageOrderCount;
        }

        public decimal CancellationRatioByTradeCount()
        {
            var cancelled = _trades.Count(trad => trad.OrderStatus == OrderStatus.Cancelled);
            var total = _trades.Count;

            if (cancelled == 0
                || total == 0)
            {
                return 0;
            }

            var byTradeCancellationRatio = (cancelled / (decimal)total);

            return byTradeCancellationRatio;
        }

        public bool HighCancellationRatioByPositionSize()
        {
            if (_cancellationRatioPercentagePosition == null)
            {
                return false;
            }

            var cancelledOrderVolumeRatio = CancellationRatioByPositionSize();

            return cancelledOrderVolumeRatio >= _cancellationRatioPercentagePosition;
        }

        public decimal CancellationRatioByPositionSize()
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
                return 0;
            }

            var cancelledOrderVolume = cancelledOrders.Sum(co => co.Volume);
            var nonCancelledOrderVolume = nonCancelledOrders.Sum(co => co.Volume);

            if (cancelledOrderVolume < 0
                || nonCancelledOrderVolume < 0)
            {
                _logger.LogError("Negative values for order volume in position. Check data integrity");
                return 0;
            }

            if (cancelledOrderVolume == 0)
            {
                return 0;
            }

            if (nonCancelledOrderVolume == 0)
            {
                return 0;
            }

            var cancelledOrderVolumeRatio =
                cancelledOrderVolume / (decimal)(cancelledOrderVolume + nonCancelledOrderVolume);

            return cancelledOrderVolumeRatio;
        }

        public int TotalVolume()
        {
            return _trades.Sum(trad => trad?.Volume ?? 0);
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
