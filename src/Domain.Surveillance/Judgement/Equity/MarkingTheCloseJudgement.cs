namespace Domain.Surveillance.Judgement.Equity
{
    public class MarkingTheCloseJudgement
    {
        public MarkingTheCloseJudgement(
            decimal? dailyPercentageMarketVolume,
            decimal? windowPercentageMarketVolume,
            decimal? thresholdOffTouchPercentage)
        {
            DailyPercentageMarketVolume = dailyPercentageMarketVolume;
            WindowPercentageMarketVolume = windowPercentageMarketVolume;
            ThresholdOffTouchPercentage = thresholdOffTouchPercentage;
        }

        public decimal? DailyPercentageMarketVolume { get; }
        public decimal? WindowPercentageMarketVolume { get; }
        public decimal? ThresholdOffTouchPercentage { get; }
    }
}
