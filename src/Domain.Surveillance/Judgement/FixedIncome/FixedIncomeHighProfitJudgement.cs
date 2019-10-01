namespace Domain.Surveillance.Judgement.FixedIncome
{
    using Domain.Surveillance.Judgement.FixedIncome.Interfaces;

    /// <summary>
    /// The fixed income high profit judgement.
    /// </summary>
    public class FixedIncomeHighProfitJudgement : IFixedIncomeHighProfitJudgement
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FixedIncomeHighProfitJudgement"/> class.
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
        /// <param name="absoluteHighProfit">
        /// The absolute high profit.
        /// </param>
        /// <param name="absoluteHighProfitCurrency">
        /// The absolute high profit currency.
        /// </param>
        /// <param name="percentageHighProfit">
        /// The percentage high profit.
        /// </param>
        /// <param name="serialisedParameters">
        /// The serialized parameters.
        /// </param>
        /// <param name="hadMissingMarketData">
        /// The had missing market data.
        /// </param>
        /// <param name="noAnalysis">
        /// The no analysis.
        /// </param>
        public FixedIncomeHighProfitJudgement(
            string ruleRunId,
            string ruleRunCorrelationId,
            string orderId,
            string clientOrderId,
            decimal? absoluteHighProfit,
            string absoluteHighProfitCurrency,
            decimal? percentageHighProfit,
            string serialisedParameters,
            bool hadMissingMarketData,
            bool noAnalysis)
        {
            this.RuleRunId = ruleRunId;
            this.RuleRunCorrelationId = ruleRunCorrelationId;
            this.OrderId = orderId;
            this.ClientOrderId = clientOrderId;

            this.AbsoluteHighProfit = absoluteHighProfit;
            this.AbsoluteHighProfitCurrency = absoluteHighProfitCurrency;
            this.PercentageHighProfit = percentageHighProfit;

            this.SerialisedParameters = serialisedParameters;
            this.HadMissingMarketData = hadMissingMarketData;
            this.NoAnalysis = noAnalysis;
        }

        /// <summary>
        /// Gets the absolute high profit.
        /// </summary>
        public decimal? AbsoluteHighProfit { get; }

        /// <summary>
        /// Gets the absolute high profit currency.
        /// </summary>
        public string AbsoluteHighProfitCurrency { get; }

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
        /// Gets the serialized parameters.
        /// </summary>
        public string SerialisedParameters { get; }

        /// <summary>
        /// Gets the percentage high profit.
        /// </summary>
        public decimal? PercentageHighProfit { get; }

        /// <summary>
        /// Gets the rule run correlation id.
        /// </summary>
        public string RuleRunCorrelationId { get; }

        /// <summary>
        /// Gets the rule run id.
        /// </summary>
        public string RuleRunId { get; }
    }
}