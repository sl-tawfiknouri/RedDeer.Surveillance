namespace Surveillance.Engine.Rules.Trades
{
    using System.Collections.Generic;
    using System.Linq;

    using Domain.Core.Trading.Orders;

    using Surveillance.Engine.Rules.Trades.Interfaces;

    /// <summary>
    ///     Buy or Sell position on an security
    ///     Can mix both
    /// </summary>
    public class TradePosition : ITradePosition
    {
        private readonly IList<Order> _trades;

        public TradePosition(IList<Order> trades)
        {
            this._trades = trades?.Where(trad => trad != null).ToList() ?? new List<Order>();
        }

        public void Add(Order item)
        {
            this._trades.Add(item);
        }

        public IList<Order> Get()
        {
            return new List<Order>(this._trades);
        }

        /// <summary>
        ///     Check if the current position (this) is a subset of the provided (arg) position
        ///     uses BY REFERENCE for comparision
        /// </summary>
        public bool PositionIsSubsetOf(ITradePosition position)
        {
            if (position == null) return false;

            if (position.Get().SequenceEqual(this._trades)) return true;

            return !this._trades.Except(position.Get()).Any();
        }

        public decimal TotalVolume()
        {
            return this._trades.Sum(trad => trad?.OrderFilledVolume ?? 0);
        }

        public decimal TotalVolumeOrderedOrFilled()
        {
            return this._trades.Where(trad => trad != null).Sum(
                trad => trad.OrderFilledVolume == 0
                            ? trad.OrderOrderedVolume.GetValueOrDefault(0)
                            : trad.OrderFilledVolume.GetValueOrDefault(0));
        }

        public decimal VolumeInStatus(OrderStatus status)
        {
            return this._trades.Where(trad => trad != null && trad.OrderStatus() == status)
                .Sum(trad => trad.OrderFilledVolume.GetValueOrDefault(0));
        }

        public decimal VolumeNotInStatus(OrderStatus status)
        {
            return this._trades.Where(trad => trad != null && trad.OrderStatus() != status)
                .Sum(trad => trad.OrderFilledVolume.GetValueOrDefault(0));
        }
    }
}