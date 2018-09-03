using Domain.Trades.Orders;

namespace Surveillance.Trades.Interfaces
{
    public interface ITradePosition
    {
        void Add(TradeOrderFrame item);
        bool HighCancellationRatioByTradeQuantity();
        bool HighCancellationRatioByTradeSize();
        int TotalVolume();
        int VolumeInStatus(OrderStatus status);
        int VolumeNotInStatus(OrderStatus status);
    }
}