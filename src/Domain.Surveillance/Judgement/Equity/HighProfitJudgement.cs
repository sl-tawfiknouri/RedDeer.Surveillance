namespace Domain.Surveillance.Judgement.Equity
{
    public class HighProfitJudgement
    {
        public HighProfitJudgement(
            string dailyHighProfit,
            string windowHighProfit,
            string parameters)
        {
            DailyHighProfit = dailyHighProfit ?? string.Empty;
            WindowHighProfit = windowHighProfit ?? string.Empty;
            Parameters = parameters ?? string.Empty;
        }

        public string DailyHighProfit { get; }
        public string WindowHighProfit { get; }
        public string Parameters { get; }
    }
}
