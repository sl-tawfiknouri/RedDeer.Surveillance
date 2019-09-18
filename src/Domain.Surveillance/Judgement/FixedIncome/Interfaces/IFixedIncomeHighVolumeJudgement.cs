namespace Domain.Surveillance.Judgement.FixedIncome.Interfaces
{
    public interface IFixedIncomeHighVolumeJudgement
    {
        string ClientOrderId { get; set; }

        bool HadMissingMarketData { get; }

        bool NoAnalysis { get; }

        string OrderId { get; set; }

        string Parameters { get; }

        string RuleRunCorrelationId { get; }

        string RuleRunId { get; }
    }
}