using Domain.Equity.Trading.Orders;
using Domain.Equity.Trading.Streams.Interfaces;

namespace Surveillance.Rules
{
    public interface IRuleManager
    {
        void RegisterProhibitedAssetRule(ITradeOrderStream<TradeOrderFrame> stream);
        void RegisterTradingRules(ITradeOrderStream<TradeOrderFrame> stream);
    }
}