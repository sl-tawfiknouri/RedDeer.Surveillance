namespace Domain.Surveillance.Judgement.FixedIncome.Interfaces
{
    /// <summary>
    /// The FixedIncomeHighVolumeJudgement interface.
    /// </summary>
    public interface IFixedIncomeHighVolumeJudgement
    {
        /// <summary>
        /// Gets or sets the client order id.
        /// </summary>
        string ClientOrderId { get; set; }

        /// <summary>
        /// Gets a value indicating whether had missing market data.
        /// </summary>
        bool HadMissingMarketData { get; }

        /// <summary>
        /// Gets a value indicating whether no analysis.
        /// </summary>
        bool NoAnalysis { get; }

        /// <summary>
        /// Gets or sets the order id.
        /// </summary>
        string OrderId { get; set; }

        /// <summary>
        /// Gets the parameters.
        /// </summary>
        string Parameters { get; }

        /// <summary>
        /// Gets the rule run correlation id.
        /// </summary>
        string RuleRunCorrelationId { get; }

        /// <summary>
        /// Gets the rule run id.
        /// </summary>
        string RuleRunId { get; }

        /// <summary>
        /// Gets or sets the window analysis analysis.
        /// </summary>
        FixedIncomeHighVolumeJudgement.BreachDetails WindowAnalysisAnalysis { get; set; }

        /// <summary>
        /// Gets or sets the daily analysis analysis.
        /// </summary>
        FixedIncomeHighVolumeJudgement.BreachDetails DailyAnalysisAnalysis { get; set; }
    }
}