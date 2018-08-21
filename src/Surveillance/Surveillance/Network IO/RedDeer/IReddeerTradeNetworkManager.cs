using Domain.Equity.Trading;
using Domain.Equity.Trading.Orders;

namespace Surveillance.Network_IO.RedDeer
{
    public interface IReddeerTradeNetworkManager 
        : INetworkManager<TradeOrderStream, TradeOrderFrame>
    {
    }
}
