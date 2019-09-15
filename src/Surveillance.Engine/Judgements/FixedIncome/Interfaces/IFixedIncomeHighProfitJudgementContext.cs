namespace Surveillance.Engine.Rules.Judgements.FixedIncome.Interfaces
{
    using Domain.Core.Financial.Money;
    using Domain.Surveillance.Judgement.FixedIncome.Interfaces;

    using Surveillance.Engine.Rules.RuleParameters.FixedIncome.Interfaces;
    using Surveillance.Engine.Rules.Rules.Interfaces;
    using Surveillance.Engine.Rules.Rules.Shared.HighProfits.Calculators.Interfaces;

    public interface IFixedIncomeHighProfitJudgementContext
    {
        string AbsoluteProfitCurrency { get; set; }

        Money? AbsoluteProfits { get; set; }

        IHighProfitsRuleFixedIncomeParameters FixedIncomeParameters { get; set; }

        IExchangeRateProfitBreakdown ExchangeRateProfits { get; set; }

        bool HasAbsoluteProfitBreach { get; set; }

        bool HasRelativeProfitBreach { get; set; }

        IFixedIncomeHighProfitJudgement Judgement { get; set; }

        bool ProjectToAlert { get; set; }

        decimal? RelativeProfits { get; set; }

        IRuleBreachContext RuleBreachContext { get; set; }
    }
}