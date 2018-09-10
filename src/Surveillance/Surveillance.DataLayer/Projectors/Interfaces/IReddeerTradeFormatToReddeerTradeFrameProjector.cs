using System.Collections.Generic;
using Domain.Trades.Orders;
using Surveillance.ElasticSearchDtos.Trades;

namespace Surveillance.DataLayer.Projectors.Interfaces
{
    public interface IReddeerTradeFormatToReddeerTradeFrameProjector
    {
        IReadOnlyCollection<TradeOrderFrame> Project(IReadOnlyCollection<ReddeerTradeDocument> tradeDocuments);
    }
}