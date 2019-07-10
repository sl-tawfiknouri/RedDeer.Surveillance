namespace Domain.Surveillance.Judgement.Equity
{
    public class HighProfitJudgement
    {
        public HighProfitJudgement(
            string ruleRunId,
            string ruleRunCorrelationId,
            string orderId,
            string clientOrderId,
            decimal? dailyHighProfit,
            decimal? windowHighProfit,
            string parameters,
            bool hadMissingMarketData,
            bool noAnalysis)
        {
            RuleRunId = ruleRunId ?? string.Empty;
            RuleRunCorrelationId = ruleRunCorrelationId ?? string.Empty;
            OrderId = orderId;
            ClientOrderId = clientOrderId;

            DailyHighProfit = dailyHighProfit;
            WindowHighProfit = windowHighProfit;
            Parameters = parameters ?? string.Empty;
            HadMissingMarketData = hadMissingMarketData;
            NoAnalysis = noAnalysis;
        }

        public string RuleRunId { get; }
        public string RuleRunCorrelationId { get; }
        public string OrderId { get; set; }
        public string ClientOrderId { get; set; }

        public decimal? DailyHighProfit { get; }
        public decimal? WindowHighProfit { get; }
        public string Parameters { get; }
        public bool HadMissingMarketData { get; }
        public bool NoAnalysis { get; }
    }
}
