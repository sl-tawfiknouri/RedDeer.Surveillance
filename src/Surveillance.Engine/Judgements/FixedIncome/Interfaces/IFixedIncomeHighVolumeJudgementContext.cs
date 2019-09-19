namespace Surveillance.Engine.Rules.Judgements.FixedIncome.Interfaces
{
    using Domain.Core.Financial.Assets;
    using Domain.Core.Trading;
    using Domain.Surveillance.Judgement.FixedIncome.Interfaces;

    using Surveillance.Engine.Rules.RuleParameters.FixedIncome.Interfaces;
    using Surveillance.Engine.Rules.Rules.Equity.HighVolume;
    using Surveillance.Engine.Rules.Rules.Interfaces;

    /// <summary>
    /// The FixedIncomeHighProfitJudgementContext interface.
    /// </summary>
    public interface IFixedIncomeHighVolumeJudgementContext
    {
        /// <summary>
        /// Gets or sets the fixed income parameters.
        /// </summary>
        IHighVolumeIssuanceRuleFixedIncomeParameters FixedIncomeParameters { get; set; }

        /// <summary>
        /// Gets or sets the judgement.
        /// </summary>
        IFixedIncomeHighVolumeJudgement Judgement { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether project to alert.
        /// </summary>
        bool RaiseRuleViolation { get; set; }

        /// <summary>
        /// Gets or sets the rule breach context.
        /// </summary>
        IRuleBreachContext RuleBreachContext { get; set; }

        /// <summary>
        /// Gets or sets the total orders traded in window.
        /// </summary>
        decimal TotalOrdersTradedInWindow { get; set; }

        /// <summary>
        /// Gets or sets the security.
        /// </summary>
        FinancialInstrument Security { get; set; }

        /// <summary>
        /// Gets or sets the window breach.
        /// </summary>
        HighVolumeRuleBreach.BreachDetails WindowBreach { get; set; }

        /// <summary>
        /// Gets or sets the daily breach.
        /// </summary>
        HighVolumeRuleBreach.BreachDetails DailyBreach { get; set; }

        /// <summary>
        /// Gets or sets the trade position.
        /// </summary>
        TradePosition TradePosition { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether is issuance breach.
        /// </summary>
        bool IsIssuanceBreach { get; set; }
    }
}