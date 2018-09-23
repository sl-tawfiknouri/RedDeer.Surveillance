using Surveillance.Rules.Interfaces;
using Surveillance.Rule_Parameters.Interfaces;

namespace Surveillance.Rules.High_Profits.Interfaces
{
    public interface IHighProfitRuleBreach : IRuleBreach
    {
        IHighProfitsRuleParameters Parameters { get; }
        bool HasAbsoluteProfitBreach { get; }
        bool HasRelativeProfitBreach { get; }
        decimal? AbsoluteProfits { get; }
        string AbsoluteProfitCurrency { get; }
        decimal? RelativeProfits { get; }
    }
}