namespace Surveillance.Engine.Rules.Judgements.FixedIncome.Interfaces
{
    using Domain.Core.Financial.Assets;
    using Domain.Core.Financial.Assets.Interfaces;
    using Domain.Core.Markets;
    using Domain.Core.Trading;
    using Domain.Core.Trading.Interfaces;
    using Domain.Surveillance.Judgement.FixedIncome.Interfaces;

    using Surveillance.Engine.Rules.RuleParameters.FixedIncome.Interfaces;
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
        IFinancialInstrument Security { get; set; }

        /// <summary>
        /// Gets or sets the trade position.
        /// </summary>
        ITradePosition TradePosition { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether is issuance breach.
        /// </summary>
        bool IsIssuanceBreach { get; set; }

        /// <summary>
        /// Gets or sets the venue.
        /// </summary>
        Market Venue { get; set; }
    }
}