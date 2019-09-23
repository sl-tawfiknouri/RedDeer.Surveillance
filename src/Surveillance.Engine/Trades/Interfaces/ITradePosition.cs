// ReSharper disable UnusedMember.Global

namespace Surveillance.Engine.Rules.Trades.Interfaces
{
    using System.Collections.Generic;

    using Domain.Core.Trading.Orders;

    public interface ITradePosition
    {
        void Add(Order item);

        IList<Order> Get();

        bool PositionIsSubsetOf(ITradePosition position);

        decimal TotalVolume();

        decimal TotalVolumeOrderedOrFilled();

        decimal VolumeInStatus(OrderStatus status);

        decimal VolumeNotInStatus(OrderStatus status);
    }
}