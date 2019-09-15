namespace Surveillance.Engine.Rules.Judgements.FixedIncome.Interfaces
{
    using Domain.Core.Financial.Money;
    using Domain.Surveillance.Judgement.FixedIncome.Interfaces;

    using Surveillance.Engine.Rules.RuleParameters.FixedIncome.Interfaces;
    using Surveillance.Engine.Rules.Rules.Interfaces;
    using Surveillance.Engine.Rules.Rules.Shared.HighProfits.Calculators.Interfaces;

    /// <summary>
    /// The FixedIncomeHighProfitJudgementContext interface.
    /// </summary>
    public interface IFixedIncomeHighProfitJudgementContext
    {
        /// <summary>
        /// Gets or sets the absolute profit currency.
        /// </summary>
        string AbsoluteProfitCurrency { get; set; }

        /// <summary>
        /// Gets or sets the absolute profits.
        /// </summary>
        Money? AbsoluteProfits { get; set; }

        /// <summary>
        /// Gets or sets the fixed income parameters.
        /// </summary>
        IHighProfitsRuleFixedIncomeParameters FixedIncomeParameters { get; set; }

        /// <summary>
        /// Gets or sets the exchange rate profits.
        /// </summary>
        IExchangeRateProfitBreakdown ExchangeRateProfits { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether has absolute profit breach.
        /// </summary>
        bool HasAbsoluteProfitBreach { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether has relative profit breach.
        /// </summary>
        bool HasRelativeProfitBreach { get; set; }

        /// <summary>
        /// Gets or sets the judgement.
        /// </summary>
        IFixedIncomeHighProfitJudgement Judgement { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether project to alert.
        /// </summary>
        bool RaiseRuleViolation { get; set; }

        /// <summary>
        /// Gets or sets the relative profits.
        /// </summary>
        decimal? RelativeProfits { get; set; }

        /// <summary>
        /// Gets or sets the rule breach context.
        /// </summary>
        IRuleBreachContext RuleBreachContext { get; set; }
    }
}