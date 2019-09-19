namespace Surveillance.Engine.Rules.Trades
{
    using System.Collections.Generic;
    using System.Linq;

    using Domain.Core.Trading.Interfaces;
    using Domain.Core.Trading.Orders;

    using Microsoft.Extensions.Logging;

    using Surveillance.Engine.Rules.Trades.Interfaces;

    public class TradePositionCancellations : ITradePositionCancellations
    {
        private readonly decimal? _cancellationRatioPercentageOrderCount;

        private readonly decimal? _cancellationRatioPercentagePosition;

        private readonly ILogger _logger;

        private readonly IList<Order> _trades;

        public TradePositionCancellations(
            IList<Order> trades,
            decimal? cancellationRatioPercentagePosition,
            decimal? cancellationRatioPercentageOrderCount,
            ILogger logger)
        {
            this._trades = trades?.Where(trad => trad != null).ToList() ?? new List<Order>();
            this._cancellationRatioPercentagePosition = cancellationRatioPercentagePosition;
            this._cancellationRatioPercentageOrderCount = cancellationRatioPercentageOrderCount;
            this._logger = logger;
        }

        public void Add(Order item)
        {
            this._trades.Add(item);
        }

        public decimal CancellationRatioByPositionSize()
        {
            var cancelledOrders = this._trades
                .Where(trad => trad != null && trad.OrderStatus() == OrderStatus.Cancelled).ToList();

            var nonCancelledOrders = this._trades
                .Where(trad => trad != null && trad.OrderStatus() != OrderStatus.Cancelled).ToList();

            if (cancelledOrders.Count == 0) return 0;

            var cancelledOrderVolume = cancelledOrders.Sum(co => co.OrderOrderedVolume.GetValueOrDefault(0));
            var nonCancelledOrderVolume = nonCancelledOrders.Sum(co => co.OrderOrderedVolume.GetValueOrDefault(0));

            if (cancelledOrderVolume < 0 || nonCancelledOrderVolume < 0)
            {
                this._logger?.LogError("Negative values for order volume in position. Check data integrity");
                return 0;
            }

            if (cancelledOrderVolume == 0) return 0;

            if (nonCancelledOrderVolume == 0) return 1;

            var cancelledOrderVolumeRatio = cancelledOrderVolume / (cancelledOrderVolume + nonCancelledOrderVolume);

            return cancelledOrderVolumeRatio;
        }

        public decimal CancellationRatioByTradeCount()
        {
            var cancelled = this._trades.Count(trad => trad.OrderStatus() == OrderStatus.Cancelled);
            var total = this._trades.Count;

            if (cancelled == 0 || total == 0) return 0;

            var byTradeCancellationRatio = cancelled / (decimal)total;

            return byTradeCancellationRatio;
        }

        public IList<Order> Get()
        {
            return new List<Order>(this._trades);
        }

        public bool HighCancellationRatioByPositionSize()
        {
            if (this._cancellationRatioPercentagePosition == null) return false;

            var cancelledOrderVolumeRatio = this.CancellationRatioByPositionSize();

            return cancelledOrderVolumeRatio >= this._cancellationRatioPercentagePosition;
        }

        public bool HighCancellationRatioByTradeCount()
        {
            if (this._cancellationRatioPercentageOrderCount == null) return false;

            var byTradeCancellationRatio = this.CancellationRatioByTradeCount();

            return byTradeCancellationRatio >= this._cancellationRatioPercentageOrderCount;
        }

        /// <summary>
        ///     Check if the current position (this) is a subset of the provided (arg) position
        ///     uses BY REFERENCE for comparision
        /// </summary>
        public bool PositionIsSubsetOf(ITradePosition position)
        {
            if (position == null) return false;

            return !this._trades.Except(position.Get()).Any();
        }

        public decimal TotalVolume()
        {
            return this._trades.Where(trad => trad != null).Sum(
                trad => trad.OrderFilledVolume == 0
                            ? trad.OrderOrderedVolume.GetValueOrDefault(0)
                            : trad.OrderFilledVolume.GetValueOrDefault(0));
        }

        public decimal TotalVolumeOrderedOrFilled()
        {
            return this._trades.Where(trad => trad != null).Sum(
                trad => trad.OrderFilledVolume.GetValueOrDefault() == 0
                            ? trad.OrderOrderedVolume.GetValueOrDefault(0)
                            : trad.OrderFilledVolume.GetValueOrDefault(0));
        }

        public decimal VolumeInStatus(OrderStatus status)
        {
            return this._trades.Where(trad => trad != null && trad.OrderStatus() == status).Sum(
                trad => trad.OrderFilledVolume.GetValueOrDefault(0) != 0
                            ? trad.OrderFilledVolume.GetValueOrDefault(0)
                            : trad.OrderOrderedVolume.GetValueOrDefault(0));
        }

        public decimal VolumeNotInStatus(OrderStatus status)
        {
            return this._trades.Where(trad => trad != null && trad.OrderStatus() != status).Sum(
                trad => trad.OrderFilledVolume.GetValueOrDefault(0) != 0
                            ? trad.OrderFilledVolume.GetValueOrDefault(0)
                            : trad.OrderOrderedVolume.GetValueOrDefault(0));
        }
    }
}