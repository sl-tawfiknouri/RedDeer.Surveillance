using System.Collections.Generic;
using Domain.Financial;
using Domain.Trading;

// ReSharper disable UnusedMember.Global

namespace Surveillance.Engine.Rules.Trades.Interfaces
{
    public interface ITradePosition
    {
        IList<Order> Get();
        void Add(Order item);

        long TotalVolume();
        long TotalVolumeOrderedOrFilled();
        long VolumeInStatus(OrderStatus status);
        long VolumeNotInStatus(OrderStatus status);

        bool PositionIsSubsetOf(ITradePosition position);
    }
}