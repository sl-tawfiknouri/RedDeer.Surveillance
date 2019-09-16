namespace Surveillance.Engine.Rules.Rules.Equity.HighProfits
{
    using System;

    using Domain.Core.Financial.Money;
    using Domain.Surveillance.Judgement.FixedIncome;
    using Domain.Surveillance.Judgement.FixedIncome.Interfaces;

    using Surveillance.Engine.Rules.Judgements.FixedIncome.Interfaces;
    using Surveillance.Engine.Rules.RuleParameters.FixedIncome.Interfaces;
    using Surveillance.Engine.Rules.Rules.Interfaces;

    using Surveillance.Engine.Rules.Rules.Shared.HighProfits.Calculators.Interfaces;

    /// <summary>
    /// Judgement context for fixed income
    /// </summary>
    public class FixedIncomeHighProfitJudgementContext : IFixedIncomeHighProfitJudgementContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FixedIncomeHighProfitJudgementContext"/> class.
        /// </summary>
        /// <param name="judgement">
        /// The judgement.
        /// </param>
        /// <param name="projectToAlert">
        /// The project to alert.
        /// </param>
        public FixedIncomeHighProfitJudgementContext(IFixedIncomeHighProfitJudgement judgement, bool projectToAlert)
        {
            this.Judgement = judgement ?? throw new ArgumentNullException(nameof(judgement));
            this.RaiseRuleViolation = projectToAlert;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FixedIncomeHighProfitJudgementContext"/> class.
        /// </summary>
        /// <param name="judgement">
        /// The judgement.
        /// </param>
        /// <param name="projectToAlert">
        /// The project to alert.
        /// </param>
        /// <param name="ruleBreachContext">
        /// The rule breach context.
        /// </param>
        /// <param name="fixedIncomeParameters">
        /// The fixed income parameters.
        /// </param>
        /// <param name="absoluteProfits">
        /// The absolute profits.
        /// </param>
        /// <param name="absoluteProfitCurrency">
        /// The absolute profit currency.
        /// </param>
        /// <param name="relativeProfits">
        /// The relative profits.
        /// </param>
        /// <param name="hasAbsoluteProfitBreach">
        /// The has absolute profit breach.
        /// </param>
        /// <param name="hasRelativeProfitBreach">
        /// The has relative profit breach.
        /// </param>
        /// <param name="profitBreakdown">
        /// The profit breakdown.
        /// </param>
        public FixedIncomeHighProfitJudgementContext(
            IFixedIncomeHighProfitJudgement judgement,
            bool projectToAlert,
            IRuleBreachContext ruleBreachContext,
            IHighProfitsRuleFixedIncomeParameters fixedIncomeParameters,
            Money? absoluteProfits,
            string absoluteProfitCurrency,
            decimal? relativeProfits,
            bool hasAbsoluteProfitBreach,
            bool hasRelativeProfitBreach,
            IExchangeRateProfitBreakdown profitBreakdown)
        {
            this.Judgement = judgement;
            this.RaiseRuleViolation = projectToAlert;
            this.RuleBreachContext = ruleBreachContext;
            this.FixedIncomeParameters = fixedIncomeParameters;
            this.AbsoluteProfits = absoluteProfits;
            this.AbsoluteProfitCurrency = absoluteProfitCurrency;
            this.RelativeProfits = relativeProfits;
            this.HasAbsoluteProfitBreach = hasAbsoluteProfitBreach;
            this.HasRelativeProfitBreach = hasRelativeProfitBreach;
            this.ExchangeRateProfits = profitBreakdown;
        }

        /// <summary>
        /// Gets or sets the absolute profit currency.
        /// </summary>
        public string AbsoluteProfitCurrency { get; set; }

        /// <summary>
        /// Gets or sets the absolute profits.
        /// </summary>
        public Money? AbsoluteProfits { get; set; }

        /// <summary>
        /// Gets or sets the fixed income parameters.
        /// </summary>
        public IHighProfitsRuleFixedIncomeParameters FixedIncomeParameters { get; set; }

        /// <summary>
        /// Gets or sets the exchange rate profits.
        /// </summary>
        public IExchangeRateProfitBreakdown ExchangeRateProfits { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether has absolute profit breach.
        /// </summary>
        public bool HasAbsoluteProfitBreach { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether has relative profit breach.
        /// </summary>
        public bool HasRelativeProfitBreach { get; set; }

        /// <summary>
        /// Gets or sets the judgement.
        /// </summary>
        public IFixedIncomeHighProfitJudgement Judgement { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether project to alert.
        /// </summary>
        public bool RaiseRuleViolation { get; set; }

        /// <summary>
        /// Gets or sets the relative profits.
        /// </summary>
        public decimal? RelativeProfits { get; set; }

        /// <summary>
        /// Gets or sets the rule breach context.
        /// </summary>
        public IRuleBreachContext RuleBreachContext { get; set; }
    }
}