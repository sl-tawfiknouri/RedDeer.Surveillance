namespace Domain.Surveillance.Judgement.Equity.Interfaces
{
    public interface IHighProfitJudgement
    {
        decimal? AbsoluteHighProfit { get; }

        string AbsoluteHighProfitCurrency { get; }

        string ClientOrderId { get; set; }

        bool HadMissingMarketData { get; }

        bool NoAnalysis { get; }

        string OrderId { get; set; }

        string Parameters { get; }

        decimal? PercentageHighProfit { get; }

        string RuleRunCorrelationId { get; }

        string RuleRunId { get; }
    }
}