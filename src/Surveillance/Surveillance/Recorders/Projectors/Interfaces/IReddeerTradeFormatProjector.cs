using Domain.Trades.Orders;
using Surveillance.ElasticSearchDtos.Trades;

namespace Surveillance.Recorders.Projectors.Interfaces
{
    public interface IReddeerTradeFormatProjector
    {
        ReddeerTradeDocument Project(TradeOrderFrame order);
    }
}