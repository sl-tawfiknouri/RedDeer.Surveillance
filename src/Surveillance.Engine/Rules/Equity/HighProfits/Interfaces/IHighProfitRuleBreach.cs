using Surveillance.Engine.Rules.RuleParameters.Equities.Interfaces;
using Surveillance.Engine.Rules.RuleParameters.Interfaces;
using Surveillance.Engine.Rules.Rules.Equity.HighProfits.Calculators.Interfaces;
using Surveillance.Engine.Rules.Rules.Interfaces;

namespace Surveillance.Engine.Rules.Rules.Equity.HighProfits.Interfaces
{
    public interface IHighProfitRuleBreach : IRuleBreach
    {
        IHighProfitsRuleEquitiesParameters EquitiesParameters { get; }
        bool HasAbsoluteProfitBreach { get; }
        bool HasRelativeProfitBreach { get; }
        decimal? AbsoluteProfits { get; }
        string AbsoluteProfitCurrency { get; }
        decimal? RelativeProfits { get; }
        bool MarketClosureVirtualProfitComponent { get; }
        IExchangeRateProfitBreakdown ExchangeRateProfits { get; }
    }
}