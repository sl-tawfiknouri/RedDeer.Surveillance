using Domain.Trades.Orders;
using Surveillance.Trades;

namespace Surveillance.Rules.Spoofing.Interfaces
{
    public interface ISpoofingRuleMessageSender
    {
        void Send(TradeOrderFrame mostRecentTrade, TradePosition tradingPosition, TradePosition opposingPosition);
    }
}