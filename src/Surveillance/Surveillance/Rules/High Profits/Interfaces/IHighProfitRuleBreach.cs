using Domain.Equity;
using Surveillance.Rule_Parameters.Interfaces;
using Surveillance.Trades.Interfaces;

namespace Surveillance.Rules.High_Profits.Interfaces
{
    public interface IHighProfitRuleBreach
    {
        IHighProfitsRuleParameters Parameters { get; }
        bool HasAbsoluteProfitBreach { get; }
        bool HasRelativeProfitBreach { get; }
        decimal? AbsoluteProfits { get; }
        string AbsoluteProfitCurrency { get; }
        decimal? RelativeProfits { get; }
        Security Security { get; }
        ITradePosition Trades { get; }
    }
}