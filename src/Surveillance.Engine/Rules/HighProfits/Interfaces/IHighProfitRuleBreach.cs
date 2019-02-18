using Surveillance.Engine.Rules.RuleParameters.Interfaces;
using Surveillance.Engine.Rules.Rules.HighProfits.Calculators.Interfaces;
using Surveillance.Engine.Rules.Rules.Interfaces;

namespace Surveillance.Engine.Rules.Rules.HighProfits.Interfaces
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