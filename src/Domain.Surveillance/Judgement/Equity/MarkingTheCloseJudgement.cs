using Domain.Surveillance.Judgement.Equity.Interfaces;

namespace Domain.Surveillance.Judgement.Equity
{
    public class MarkingTheCloseJudgement : IMarkingTheCloseJudgement
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
