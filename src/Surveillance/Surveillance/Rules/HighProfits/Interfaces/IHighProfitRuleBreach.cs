using Surveillance.RuleParameters.Interfaces;
using Surveillance.Rules.HighProfits.Calculators.Interfaces;
using Surveillance.Rules.Interfaces;

namespace Surveillance.Rules.HighProfits.Interfaces
{
    public interface IHighProfitRuleBreach : IRuleBreach
    {
        IHighProfitsRuleParameters Parameters { get; }
        bool HasAbsoluteProfitBreach { get; }
        bool HasRelativeProfitBreach { get; }
        decimal? AbsoluteProfits { get; }
        string AbsoluteProfitCurrency { get; }
        decimal? RelativeProfits { get; }
        bool MarketClosureVirtualProfitComponent { get; }
        IExchangeRateProfitBreakdown ExchangeRateProfits { get; }
    }
}