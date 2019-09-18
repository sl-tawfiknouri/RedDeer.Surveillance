namespace Domain.Surveillance.Judgement.FixedIncome
{
    using Domain.Core.Financial.Money;
    using Domain.Core.Markets;
    using Domain.Surveillance.Judgement.FixedIncome.Interfaces;

    /// <summary>
    /// The fixed income high volume judgement.
    /// </summary>
    public class FixedIncomeHighVolumeJudgement : IFixedIncomeHighVolumeJudgement
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FixedIncomeHighVolumeJudgement"/> class.
        /// </summary>
        /// <param name="ruleRunId">
        /// The rule run id.
        /// </param>
        /// <param name="ruleRunCorrelationId">
        /// The rule run correlation id.
        /// </param>
        /// <param name="orderId">
        /// The order id.
        /// </param>
        /// <param name="clientOrderId">
        /// The client order id.
        /// </param>
        /// <param name="parameters">
        /// The parameters.
        /// </param>
        /// <param name="hadMissingMarketData">
        /// The had missing market data.
        /// </param>
        /// <param name="noAnalysis">
        /// The no analysis.
        /// </param>
        /// <param name="windowBreach">
        /// The window Breach.
        /// </param>
        /// <param name="dailyBreach">
        /// The daily Breach.
        /// </param>
        public FixedIncomeHighVolumeJudgement(
            string ruleRunId,
            string ruleRunCorrelationId,
            string orderId,
            string clientOrderId,
            string parameters,
            bool hadMissingMarketData,
            bool noAnalysis,
            BreachDetails windowBreach,
            BreachDetails dailyBreach)
        {
            this.RuleRunId = ruleRunId;
            this.RuleRunCorrelationId = ruleRunCorrelationId;
            this.OrderId = orderId;
            this.ClientOrderId = clientOrderId;

            this.Parameters = parameters;
            this.HadMissingMarketData = hadMissingMarketData;
            this.NoAnalysis = noAnalysis;
            this.WindowBreach = windowBreach;
            this.DailyBreach = dailyBreach;
        }

        /// <summary>
        /// Gets or sets the client order id.
        /// </summary>
        public string ClientOrderId { get; set; }

        /// <summary>
        /// Gets a value indicating whether had missing market data.
        /// </summary>
        public bool HadMissingMarketData { get; }

        /// <summary>
        /// Gets a value indicating whether no analysis.
        /// </summary>
        public bool NoAnalysis { get; }

        /// <summary>
        /// Gets or sets the order id.
        /// </summary>
        public string OrderId { get; set; }

        /// <summary>
        /// Gets the parameters.
        /// </summary>
        public string Parameters { get; }

        /// <summary>
        /// Gets the rule run correlation id.
        /// </summary>
        public string RuleRunCorrelationId { get; }

        /// <summary>
        /// Gets the rule run id.
        /// </summary>
        public string RuleRunId { get; }

        /// <summary>
        /// Gets the daily breach.
        /// </summary>
        public BreachDetails DailyBreach { get; }

        /// <summary>
        /// Gets the window breach.
        /// </summary>
        public BreachDetails WindowBreach { get; }

        /// <summary>
        /// The breach details.
        /// </summary>
        public class BreachDetails
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="BreachDetails"/> class.
            /// </summary>
            /// <param name="hasBreach">
            /// The has breach.
            /// </param>
            /// <param name="breachPercentage">
            /// The breach percentage.
            /// </param>
            /// <param name="breachThresholdAmount">
            /// The breach threshold amount.
            /// </param>
            /// <param name="venue">
            /// The venue.
            /// </param>
            public BreachDetails(
                bool hasBreach,
                decimal? breachPercentage,
                decimal breachThresholdAmount,
                Market venue)
            {
                this.HasBreach = hasBreach;
                this.BreachPercentage = breachPercentage;
                this.BreachThresholdAmount = breachThresholdAmount;
                this.Venue = venue;
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="BreachDetails"/> class.
            /// </summary>
            /// <param name="hasBreach">
            /// The has breach.
            /// </param>
            /// <param name="breachPercentage">
            /// The breach percentage.
            /// </param>
            /// <param name="breachThresholdMoney">
            /// The breach threshold money.
            /// </param>
            /// <param name="breachTradedMoney">
            /// The breach traded money.
            /// </param>
            /// <param name="venue">
            /// The venue.
            /// </param>
            public BreachDetails(
                bool hasBreach,
                decimal? breachPercentage,
                Money breachThresholdMoney,
                Money breachTradedMoney,
                Market venue)
            {
                this.HasBreach = hasBreach;
                this.BreachPercentage = breachPercentage;
                this.BreachThresholdMoney = breachThresholdMoney;
                this.BreachTradedMoney = breachTradedMoney;
                this.Venue = venue;
            }

            /// <summary>
            /// Gets the breach percentage.
            /// </summary>
            public decimal? BreachPercentage { get; }

            /// <summary>
            /// Gets the breach threshold amount.
            /// </summary>
            public decimal BreachThresholdAmount { get; }

            /// <summary>
            /// Gets the breach threshold money.
            /// </summary>
            public Money BreachThresholdMoney { get; }

            /// <summary>
            /// Gets the breach traded money.
            /// </summary>
            public Money BreachTradedMoney { get; }

            /// <summary>
            /// Gets a value indicating whether has breach.
            /// </summary>
            public bool HasBreach { get; }

            /// <summary>
            /// Gets the venue.
            /// </summary>
            public Market Venue { get; }

            /// <summary>
            /// The none.
            /// </summary>
            /// <returns>
            /// The <see cref="BreachDetails"/>.
            /// </returns>
            public static BreachDetails None()
            {
                return new BreachDetails(false, null, 0, null);
            }
        }
    }
}