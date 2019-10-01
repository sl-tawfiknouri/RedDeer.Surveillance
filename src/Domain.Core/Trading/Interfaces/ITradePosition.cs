namespace Domain.Core.Trading.Interfaces
{
    using System.Collections.Generic;

    using Domain.Core.Trading.Orders;

    /// <summary>
    /// The TradePosition interface.
    /// </summary>
    public interface ITradePosition
    {
        /// <summary>
        /// The add.
        /// </summary>
        /// <param name="item">
        /// The item.
        /// </param>
        void Add(Order item);

        /// <summary>
        /// The get.
        /// </summary>
        /// <returns>
        /// The <see cref="IList"/>.
        /// </returns>
        IList<Order> Get();

        /// <summary>
        /// The position is subset of.
        /// </summary>
        /// <param name="position">
        /// The position.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        bool PositionIsSubsetOf(ITradePosition position);

        /// <summary>
        /// The total volume.
        /// </summary>
        /// <returns>
        /// The <see cref="decimal"/>.
        /// </returns>
        decimal TotalVolume();

        /// <summary>
        /// The total volume ordered or filled.
        /// </summary>
        /// <returns>
        /// The <see cref="decimal"/>.
        /// </returns>
        decimal TotalVolumeOrderedOrFilled();

        /// <summary>
        /// The volume in status.
        /// </summary>
        /// <param name="status">
        /// The status.
        /// </param>
        /// <returns>
        /// The <see cref="decimal"/>.
        /// </returns>
        decimal VolumeInStatus(OrderStatus status);

        /// <summary>
        /// The volume not in status.
        /// </summary>
        /// <param name="status">
        /// The status.
        /// </param>
        /// <returns>
        /// The <see cref="decimal"/>.
        /// </returns>
        decimal VolumeNotInStatus(OrderStatus status);
    }
}