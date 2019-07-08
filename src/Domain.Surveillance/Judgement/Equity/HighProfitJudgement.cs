namespace Domain.Surveillance.Judgement.Equity
{
    public class HighProfitJudgement
    {
        public HighProfitJudgement(
            string dailyHighProfit,
            string windowHighProfit,
            string parameters,
            bool hadMissingMarketData,
            bool noAnalysis)
        {
            DailyHighProfit = dailyHighProfit ?? string.Empty;
            WindowHighProfit = windowHighProfit ?? string.Empty;
            Parameters = parameters ?? string.Empty;
            HadMissingMarketData = hadMissingMarketData;
            NoAnalysis = noAnalysis;
        }

        public string DailyHighProfit { get; }
        public string WindowHighProfit { get; }
        public string Parameters { get; }
        public bool HadMissingMarketData { get; }
        public bool NoAnalysis { get; }
    }
}
