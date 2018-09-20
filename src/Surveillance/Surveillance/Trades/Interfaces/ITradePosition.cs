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
        decimal CancellationRatioByTradeCount();
        decimal CancellationRatioByPositionSize();

        int TotalVolume();
        int VolumeInStatus(OrderStatus status);
        int VolumeNotInStatus(OrderStatus status);

        bool PositionIsSubsetOf(ITradePosition position);
    }
}