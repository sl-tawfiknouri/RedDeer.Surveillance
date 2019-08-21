namespace Domain.Surveillance.Judgement.Equity
{
    using Domain.Surveillance.Judgement.Equity.Interfaces;

    public class MarkingTheCloseJudgement : IMarkingTheCloseJudgement
    {
        public MarkingTheCloseJudgement(
            decimal? dailyPercentageMarketVolume,
            decimal? windowPercentageMarketVolume,
            decimal? thresholdOffTouchPercentage)
        {
            this.DailyPercentageMarketVolume = dailyPercentageMarketVolume;
            this.WindowPercentageMarketVolume = windowPercentageMarketVolume;
            this.ThresholdOffTouchPercentage = thresholdOffTouchPercentage;
        }

        public decimal? DailyPercentageMarketVolume { get; }

        public decimal? ThresholdOffTouchPercentage { get; }

        public decimal? WindowPercentageMarketVolume { get; }
    }
}