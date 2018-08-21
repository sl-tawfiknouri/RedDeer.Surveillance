using Domain.Equity.Trading.Streams.Interfaces;

namespace Surveillance.Rules
{
    public interface IRuleManager
    {
        void RegisterProhibitedAssetRule(ITradeOrderStream stream);
        void RegisterTradingRules(ITradeOrderStream stream);
    }
}