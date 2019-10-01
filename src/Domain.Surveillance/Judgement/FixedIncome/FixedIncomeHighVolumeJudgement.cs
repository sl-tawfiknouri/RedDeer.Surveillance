namespace Domain.Surveillance.Judgement.FixedIncome
{
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
        /// <param name="market">
        /// Venue that the trade executed on
        /// </param>
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
        /// <param name="windowAnalysis">
        /// The window analysis
        /// </param>
        /// <param name="dailyAnalysis">
        /// The daily analysis
        /// </param>
        public FixedIncomeHighVolumeJudgement(
            Market market,
            string ruleRunId,
            string ruleRunCorrelationId,
            string orderId,
            string clientOrderId,
            string parameters,
            bool hadMissingMarketData,
            bool noAnalysis,
            BreachDetails windowAnalysis,
            BreachDetails dailyAnalysis)
        {
            this.Market = market;
            this.RuleRunId = ruleRunId;
            this.RuleRunCorrelationId = ruleRunCorrelationId;
            this.OrderId = orderId;
            this.ClientOrderId = clientOrderId;

            this.Parameters = parameters;
            this.HadMissingMarketData = hadMissingMarketData;
            this.NoAnalysis = noAnalysis;

            this.WindowAnalysisAnalysis = windowAnalysis;
            this.DailyAnalysisAnalysis = dailyAnalysis;
        }

        /// <summary>
        /// Gets or sets the market.
        /// </summary>
        public Market Market { get; set; }

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
        /// Gets or sets the window analysis analysis.
        /// </summary>
        public BreachDetails WindowAnalysisAnalysis { get; set; }

        /// <summary>
        /// Gets or sets the daily analysis analysis.
        /// </summary>
        public BreachDetails DailyAnalysisAnalysis { get; set; }

        /// <summary>
        /// The breach details.
        /// </summary>
        public class BreachDetails
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="BreachDetails"/> class.
            /// </summary>
            /// <param name="volumeThresholdAmount">
            /// The volume threshold amount.
            /// </param>
            /// <param name="volumeThresholdPercentage">
            /// The volume threshold percentage.
            /// </param>
            /// <param name="volumeTradedAmount">
            /// The volume traded amount.
            /// </param>
            /// <param name="volumeTradedPercentage">
            /// The volume traded percentage.
            /// </param>
            /// <param name="volumeBreach">
            /// The volume breach.
            /// </param>
            public BreachDetails(
                decimal? volumeThresholdAmount,
                decimal? volumeThresholdPercentage,
                decimal? volumeTradedAmount,
                decimal? volumeTradedPercentage,
                bool volumeBreach)
            {
                this.VolumeThresholdAmount = volumeThresholdAmount;
                this.VolumeThresholdPercentage = volumeThresholdPercentage;
                this.VolumeTradedAmount = volumeTradedAmount;
                this.VolumeTradedPercentage = volumeTradedPercentage;
                this.VolumeBreach = volumeBreach;
            }

            /// <summary>
            /// Prevents a default instance of the <see cref="BreachDetails"/> class from being created.
            /// </summary>
            private BreachDetails()
            {
                // used by None
            }

            /// <summary>
            /// Gets or sets the volume threshold amount.
            /// </summary>
            public decimal? VolumeThresholdAmount { get; set; }

            /// <summary>
            /// Gets or sets the volume threshold percentage.
            /// </summary>
            public decimal? VolumeThresholdPercentage { get; set; }

            /// <summary>
            /// Gets or sets the volume traded amount.
            /// </summary>
            public decimal? VolumeTradedAmount { get; set; }

            /// <summary>
            /// Gets or sets the volume traded percentage.
            /// </summary>
            public decimal? VolumeTradedPercentage { get; set; }

            /// <summary>
            /// Gets or sets a value indicating whether volume breach.
            /// </summary>
            public bool VolumeBreach { get; set; }

            /// <summary>
            /// The none.
            /// </summary>
            /// <returns>
            /// The <see cref="BreachDetails"/>.
            /// </returns>
            public static BreachDetails None()
            {
                return new BreachDetails();
            }
        }
    }
}