using Domain.Finance;
using Surveillance.Rule_Parameters.Interfaces;
using Surveillance.Trades;

namespace Surveillance.Rules.WashTrade.Interfaces
{
    public interface IWashTradeRuleBreach
    {
        CurrencyAmount? AveragePositionAbsoluteValueChange { get; }
        int? AveragePositionAmountOfTrades { get; }
        decimal? AveragePositionRelativeValueChange { get; }
        bool AveragePositionRuleBreach { get; }
        TradePosition BreachingTradePosition { get; }
        IWashTradeRuleParameters Parameters { get; }
    }
}