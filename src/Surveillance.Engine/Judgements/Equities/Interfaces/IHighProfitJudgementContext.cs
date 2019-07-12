using Domain.Core.Financial.Money;
using Domain.Surveillance.Judgement.Equity.Interfaces;
using Surveillance.Engine.Rules.RuleParameters.Equities.Interfaces;
using Surveillance.Engine.Rules.Rules.Equity.HighProfits.Calculators.Interfaces;
using Surveillance.Engine.Rules.Rules.Interfaces;

namespace Surveillance.Engine.Rules.Judgements.Equities.Interfaces
{
    public interface IHighProfitJudgementContext
    {
        IHighProfitJudgement Judgement { get; set; }

        bool ProjectToAlert { get; set; }
        IRuleBreachContext RuleBreachContext { get; set; }
        IHighProfitsRuleEquitiesParameters EquitiesParameters { get; set; }
        bool HasRelativeProfitBreach { get; set; }
        bool HasAbsoluteProfitBreach { get; set; }
        string AbsoluteProfitCurrency { get; set; }
        Money? AbsoluteProfits { get; set; }
        decimal? RelativeProfits { get; set; }
        IExchangeRateProfitBreakdown ExchangeRateProfits { get; set; }
    }
}