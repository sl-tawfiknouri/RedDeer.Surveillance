using Domain.Core.Financial.Money;
using Domain.Surveillance.Judgement.Equity;
using Surveillance.Engine.Rules.RuleParameters.Equities.Interfaces;
using Surveillance.Engine.Rules.Rules.Equity.HighProfits.Calculators.Interfaces;
using Surveillance.Engine.Rules.Rules.Interfaces;

namespace Surveillance.Engine.Rules.Judgements.Equities.Interfaces
{
    public interface IHighProfitJudgementContext
    {
        HighProfitJudgement Judgement { get; }

        bool ProjectToAlert { get; }
        IRuleBreachContext RuleBreachContext { get; }
        IHighProfitsRuleEquitiesParameters EquitiesParameters { get; }
        bool HasRelativeProfitBreach { get; }
        bool HasAbsoluteProfitBreach { get; }
        string AbsoluteProfitCurrency { get; }
        Money? AbsoluteProfits { get; }
        decimal? RelativeProfits { get; }
        IExchangeRateProfitBreakdown ExchangeRateProfits { get; }
    }
}