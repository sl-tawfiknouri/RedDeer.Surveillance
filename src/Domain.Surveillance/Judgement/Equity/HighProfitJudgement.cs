using Domain.Surveillance.Judgement.Equity.Interfaces;

namespace Domain.Surveillance.Judgement.Equity
{
    public class HighProfitJudgement : IHighProfitJudgement
    {
        public HighProfitJudgement(
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
            RuleRunId = ruleRunId;
            RuleRunCorrelationId = ruleRunCorrelationId;
            OrderId = orderId;
            ClientOrderId = clientOrderId;

            AbsoluteHighProfit = absoluteHighProfit;
            AbsoluteHighProfitCurrency = absoluteHighProfitCurrency;
            PercentageHighProfit = percentageHighProfit;

            Parameters = parameters;
            HadMissingMarketData = hadMissingMarketData;
            NoAnalysis = noAnalysis;
        }

        public string RuleRunId { get; }
        public string RuleRunCorrelationId { get; }
        public string OrderId { get; set; }
        public string ClientOrderId { get; set; }

        public decimal? AbsoluteHighProfit { get; }
        public string AbsoluteHighProfitCurrency { get; }
        public decimal? PercentageHighProfit { get; }
        public string Parameters { get; }
        public bool HadMissingMarketData { get; }
        public bool NoAnalysis { get; }
    }
}
