using System.Collections.Generic;
using Domain.Trades.Orders;
// ReSharper disable UnusedMember.Global

namespace Surveillance.Trades.Interfaces
{
    public interface ITradePosition
    {
        IList<TradeOrderFrame> Get();
        void Add(TradeOrderFrame item);

        int TotalVolume();
        int VolumeInStatus(OrderStatus status);
        int VolumeNotInStatus(OrderStatus status);

        bool PositionIsSubsetOf(ITradePosition position);
    }
}