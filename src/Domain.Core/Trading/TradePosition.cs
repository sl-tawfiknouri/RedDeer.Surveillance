namespace Domain.Core.Trading
{
    using System.Collections.Generic;

    using System.Linq;

    using Domain.Core.Trading.Interfaces;
    using Domain.Core.Trading.Orders;

    /// <summary>
    ///     Buy or Sell position on an security
    ///     Can mix both
    /// </summary>
    public class TradePosition : ITradePosition
    {
        /// <summary>
        /// The trades.
        /// </summary>
        private readonly IList<Order> trades;

        /// <summary>
        /// Initializes a new instance of the <see cref="TradePosition"/> class.
        /// </summary>
        /// <param name="trades">
        /// The trades.
        /// </param>
        public TradePosition(IList<Order> trades)
        {
            this.trades = trades?.Where(_ => _ != null).ToList() ?? new List<Order>();
        }

        /// <summary>
        /// The add.
        /// </summary>
        /// <param name="item">
        /// The item.
        /// </param>
        public void Add(Order item)
        {
            this.trades.Add(item);
        }

        /// <summary>
        /// The get.
        /// </summary>
        /// <returns>
        /// The <see cref="Order"/>.
        /// </returns>
        public IList<Order> Get()
        {
            return new List<Order>(this.trades);
        }

        /// <summary>
        /// Check if the current position (this) is a subset of the provided (argument) position uses BY REFERENCE for comparison
        /// </summary>
        /// <param name="position">
        /// The position.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool PositionIsSubsetOf(ITradePosition position)
        {
            if (position == null)
            {
                return false;
            }

            if (position.Get().SequenceEqual(this.trades))
            {
                return true;
            }

            return !this.trades.Except(position.Get()).Any();
        }

        /// <summary>
        /// The total volume.
        /// </summary>
        /// <returns>
        /// The <see cref="decimal"/>.
        /// </returns>
        public decimal TotalVolume()
        {
            return this.trades.Sum(_ => _?.OrderFilledVolume ?? 0);
        }

        /// <summary>
        /// The total volume ordered or filled.
        /// </summary>
        /// <returns>
        /// The <see cref="decimal"/>.
        /// </returns>
        public decimal TotalVolumeOrderedOrFilled()
        {
            return this.trades
                .Where(_ => _ != null)
                .Sum(_ => _.OrderFilledVolume == 0
                            ? _.OrderOrderedVolume.GetValueOrDefault(0)
                            : _.OrderFilledVolume.GetValueOrDefault(0));
        }

        /// <summary>
        /// The volume in status.
        /// </summary>
        /// <param name="status">
        /// The status.
        /// </param>
        /// <returns>
        /// The <see cref="decimal"/>.
        /// </returns>
        public decimal VolumeInStatus(OrderStatus status)
        {
            return this.trades
                .Where(_ => _ != null && _.OrderStatus() == status)
                .Sum(_ => _.OrderFilledVolume.GetValueOrDefault(0));
        }

        /// <summary>
        /// The volume not in status.
        /// </summary>
        /// <param name="status">
        /// The status.
        /// </param>
        /// <returns>
        /// The <see cref="decimal"/>.
        /// </returns>
        public decimal VolumeNotInStatus(OrderStatus status)
        {
            return this.trades
                .Where(_ => _ != null && _.OrderStatus() != status)
                .Sum(_ => _.OrderFilledVolume.GetValueOrDefault(0));
        }
    }
}
