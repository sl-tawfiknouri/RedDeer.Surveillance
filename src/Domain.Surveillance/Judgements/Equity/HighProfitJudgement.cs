namespace Domain.Surveillance.Judgements.Equity
{
    public class HighProfitJudgement
    {
        public HighProfitJudgement(
            string dailyHighProfit,
            string windowHighProfit,
            string parameters,
            bool projectToAlert)
        {
            DailyHighProfit = dailyHighProfit ?? string.Empty;
            WindowHighProfit = windowHighProfit ?? string.Empty;
            Parameters = parameters ?? string.Empty;
            ProjectToAlert = projectToAlert;
        }

        public string DailyHighProfit { get; }
        public string WindowHighProfit { get; }
        public string Parameters { get; }
        public bool ProjectToAlert { get; }
    }
}
