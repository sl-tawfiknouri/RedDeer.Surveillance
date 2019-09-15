namespace Domain.Surveillance.Judgement.FixedIncome
{
    using Domain.Surveillance.Judgement.FixedIncome.Interfaces;

    public class FixedIncomeHighProfitJudgement : IFixedIncomeHighProfitJudgement
    {
        public FixedIncomeHighProfitJudgement(
            string ruleRunId,
            string ruleRunCorrelationId,
            string orderId,
            string clientOrderId,
            decimal? absoluteHighProfit,
            string absoluteHighProfitCurrency,
            decimal? percentageHighProfit,
            string parameters,
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

            this.Parameters = parameters;
            this.HadMissingMarketData = hadMissingMarketData;
            this.NoAnalysis = noAnalysis;
        }

        public decimal? AbsoluteHighProfit { get; }

        public string AbsoluteHighProfitCurrency { get; }

        public string ClientOrderId { get; set; }

        public bool HadMissingMarketData { get; }

        public bool NoAnalysis { get; }

        public string OrderId { get; set; }

        public string Parameters { get; }

        public decimal? PercentageHighProfit { get; }

        public string RuleRunCorrelationId { get; }

        public string RuleRunId { get; }
    }
}