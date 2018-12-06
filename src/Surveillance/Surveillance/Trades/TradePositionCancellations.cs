using System.Collections.Generic;
using System.Linq;
using DomainV2.Financial;
using DomainV2.Trading;
using Microsoft.Extensions.Logging;
using Surveillance.Trades.Interfaces;

namespace Surveillance.Trades
{
    public class TradePositionCancellations : ITradePositionCancellations
    {
        private readonly IList<Order> _trades;
        private readonly decimal? _cancellationRatioPercentagePosition;
        private readonly decimal? _cancellationRatioPercentageOrderCount;
        private readonly ILogger _logger;

        public TradePositionCancellations(
            IList<Order> trades,
            decimal? cancellationRatioPercentagePosition,
            decimal? cancellationRatioPercentageOrderCount,
            ILogger logger)
        {
            _trades = trades?.Where(trad => trad != null).ToList() ?? new List<Order>();
            _cancellationRatioPercentagePosition = cancellationRatioPercentagePosition;
            _cancellationRatioPercentageOrderCount = cancellationRatioPercentageOrderCount;
            _logger = logger;
        }

        public IList<Order> Get()
        {
            return new List<Order>(_trades);
        }

        public void Add(Order item)
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

            var cancelledOrderVolume = cancelledOrders.Sum(co => co.OrderedVolume);
            var nonCancelledOrderVolume = nonCancelledOrders.Sum(co => co.OrderedVolume);

            if (cancelledOrderVolume < 0
                || nonCancelledOrderVolume < 0)
            {
                _logger?.LogError("Negative values for order volume in position. Check data integrity");
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

        public long TotalVolume()
        {
            return _trades
                .Where(trad => trad != null)
                .Sum(trad =>
                    trad.OrderFilledVolume == 0 
                        ? trad.OrderOrderedVolume.GetValueOrDefault(0) 
                        : (trad.OrderFilledVolume.GetValueOrDefault(0)));
        }

        public long TotalVolumeOrderedOrFilled()
        {
            return _trades
                .Where(trad => trad != null)
                .Sum(trad => trad.FulfilledVolume == 0 ? trad.OrderedVolume : (trad.FulfilledVolume));
        }

        public long VolumeInStatus(OrderStatus status)
        {
            return
                _trades
                .Where(trad => trad != null && trad.OrderStatus == status)
                .Sum(trad => trad.FulfilledVolume != 0 ? trad.FulfilledVolume : trad.OrderedVolume);
        }

        public long VolumeNotInStatus(OrderStatus status)
        {
            return
                _trades
                .Where(trad => trad != null && trad.OrderStatus != status)
                .Sum(trad => trad.FulfilledVolume != 0 ? trad.FulfilledVolume : trad.OrderedVolume);
        }

        /// <summary>
        /// Check if the current position (this) is a subset of the provided (arg) position
        /// uses BY REFERENCE for comparision
        /// </summary>
        public bool PositionIsSubsetOf(ITradePosition position)
        {
            if (position == null)
            {
                return false;
            }

            return !_trades.Except(position.Get()).Any();
        }
    }
}
