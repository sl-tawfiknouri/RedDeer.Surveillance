using Surveillance.Engine.Rules.Rules.Shared.HighProfits.Calculators.Interfaces;

namespace Surveillance.Engine.Rules.Judgements.Equities.Interfaces
{
    using Domain.Core.Financial.Money;
    using Domain.Surveillance.Judgement.Equity.Interfaces;

    using Surveillance.Engine.Rules.RuleParameters.Equities.Interfaces;
    using Surveillance.Engine.Rules.Rules.Interfaces;

    public interface IHighProfitJudgementContext
    {
        string AbsoluteProfitCurrency { get; set; }

        Money? AbsoluteProfits { get; set; }

        IHighProfitsRuleEquitiesParameters EquitiesParameters { get; set; }

        IExchangeRateProfitBreakdown ExchangeRateProfits { get; set; }

        bool HasAbsoluteProfitBreach { get; set; }

        bool HasRelativeProfitBreach { get; set; }

        IHighProfitJudgement Judgement { get; set; }

        bool ProjectToAlert { get; set; }

        decimal? RelativeProfits { get; set; }

        IRuleBreachContext RuleBreachContext { get; set; }
    }
}