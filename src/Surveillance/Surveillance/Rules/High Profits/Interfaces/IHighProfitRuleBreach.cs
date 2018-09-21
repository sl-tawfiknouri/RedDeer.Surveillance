using System.Collections.Generic;
using Domain.Equity;
using Domain.Trades.Orders;
using Surveillance.Rule_Parameters.Interfaces;

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
        IReadOnlyCollection<TradeOrderFrame> Trades { get; }
    }
}