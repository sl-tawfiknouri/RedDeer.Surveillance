namespace Domain.Surveillance.Judgements.Equity
{
    public class LayeringJudgement
    {
        public LayeringJudgement(decimal? dailyPercentageMarketVolume, decimal? windowPercentageMarketVolume)
        {
            DailyPercentageMarketVolume = dailyPercentageMarketVolume;
            WindowPercentageMarketVolume = windowPercentageMarketVolume;
        }
        
        public decimal? DailyPercentageMarketVolume { get; }
        public decimal? WindowPercentageMarketVolume { get; }
    }
}
