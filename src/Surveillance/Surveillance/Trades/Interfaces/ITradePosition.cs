using System.Collections.Generic;
using Domain.Trades.Orders;

namespace Surveillance.Trades.Interfaces
{
    public interface ITradePosition
    {
        IList<TradeOrderFrame> Get();
        void Add(TradeOrderFrame item);
        bool HighCancellationRatioByTradeCount();
        bool HighCancellationRatioByPositionSize();
        int TotalVolume();
        int VolumeInStatus(OrderStatus status);
        int VolumeNotInStatus(OrderStatus status);
    }
}