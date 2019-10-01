namespace Surveillance.Engine.Rules.Rules.FixedIncome.HighVolume
{
    using System;

    using Domain.Core.Financial.Assets;
    using Domain.Core.Financial.Assets.Interfaces;
    using Domain.Core.Markets;
    using Domain.Core.Trading;
    using Domain.Core.Trading.Interfaces;
    using Domain.Surveillance.Judgement.FixedIncome.Interfaces;

    using Surveillance.Engine.Rules.Judgements.FixedIncome.Interfaces;
    using Surveillance.Engine.Rules.RuleParameters.FixedIncome.Interfaces;
    using Surveillance.Engine.Rules.Rules.Interfaces;

    /// <summary>
    /// Judgement context for fixed income
    /// </summary>
    public class FixedIncomeHighVolumeJudgementContext : IFixedIncomeHighVolumeJudgementContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FixedIncomeHighVolumeJudgementContext"/> class.
        /// </summary>
        /// <param name="judgement">
        /// The judgement.
        /// </param>
        /// <param name="raiseRuleViolation">
        /// The project to alert.
        /// </param>
        /// <param name="tradePosition">
        /// The trade position.
        /// </param>
        /// <param name="venue">
        /// The venue.
        /// </param>
        public FixedIncomeHighVolumeJudgementContext(
            IFixedIncomeHighVolumeJudgement judgement,
            bool raiseRuleViolation,
            ITradePosition tradePosition,
            Market venue)
        {
            this.Judgement = judgement ?? throw new ArgumentNullException(nameof(judgement));
            this.RaiseRuleViolation = raiseRuleViolation;
            this.TradePosition = tradePosition ?? throw new ArgumentNullException(nameof(tradePosition));
            this.Venue = venue;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FixedIncomeHighVolumeJudgementContext"/> class.
        /// </summary>
        /// <param name="judgement">
        /// The judgement.
        /// </param>
        /// <param name="raiseRuleViolation">
        /// The raise rule violation.
        /// </param>
        /// <param name="ruleBreachContext">
        /// The rule breach context.
        /// </param>
        /// <param name="fixedIncomeParameters">
        /// The fixed income parameters.
        /// </param>
        /// <param name="totalOrdersTradedInWindow">
        /// The total orders traded in window.
        /// </param>
        /// <param name="security">
        /// The security.
        /// </param>
        /// <param name="isIssuanceBreach">
        /// The is issuance breach.
        /// </param>
        /// <param name="tradePosition">
        /// The trade Position.
        /// </param>
        /// <param name="venue">
        /// The venue.
        /// </param>
        public FixedIncomeHighVolumeJudgementContext(
            IFixedIncomeHighVolumeJudgement judgement,
            bool raiseRuleViolation,
            IRuleBreachContext ruleBreachContext,
            IHighVolumeIssuanceRuleFixedIncomeParameters fixedIncomeParameters,
            decimal totalOrdersTradedInWindow,
            FinancialInstrument security,
            bool isIssuanceBreach,
            TradePosition tradePosition,
            Market venue)
        {
            this.Judgement = judgement;
            this.RaiseRuleViolation = raiseRuleViolation;
            this.RuleBreachContext = ruleBreachContext;
            this.FixedIncomeParameters = fixedIncomeParameters;
            this.TotalOrdersTradedInWindow = totalOrdersTradedInWindow;
            this.Security = security;
            this.IsIssuanceBreach = isIssuanceBreach;
            this.TradePosition = tradePosition;
            this.Venue = venue;
        }

        /// <summary>
        /// Gets or sets the fixed income parameters.
        /// </summary>
        public IHighVolumeIssuanceRuleFixedIncomeParameters FixedIncomeParameters { get; set; }

        /// <summary>
        /// Gets or sets the judgement.
        /// </summary>
        public IFixedIncomeHighVolumeJudgement Judgement { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether project to alert.
        /// </summary>
        public bool RaiseRuleViolation { get; set; }

        /// <summary>
        /// Gets or sets the rule breach context.
        /// </summary>
        public IRuleBreachContext RuleBreachContext { get; set; }

        /// <summary>
        /// Gets or sets the total orders traded in window.
        /// </summary>
        public decimal TotalOrdersTradedInWindow { get; set; }

        /// <summary>
        /// Gets or sets the security.
        /// </summary>
        public IFinancialInstrument Security { get; set; }
        
        /// <summary>
        /// Gets or sets the trade position.
        /// </summary>
        public ITradePosition TradePosition { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether is issuance breach.
        /// </summary>
        public bool IsIssuanceBreach { get; set; }

        /// <summary>
        /// Gets or sets the venue.
        /// </summary>
        public Market Venue { get; set; }
    }
}