using Domain.Surveillance.Judgement.Equity.Interfaces;

namespace Domain.Surveillance.Judgement.Equity
{
    public class HighVolumeJudgement : IHighVolumeJudgement
    {
        public HighVolumeJudgement(
            decimal? dailyHighVolumePercentage,
            decimal? windowHighVolumePercentage,
            decimal? marketCapHighVolumePercentage)
        {
            DailyHighVolumePercentage = dailyHighVolumePercentage;
            WindowHighVolumePercentage = windowHighVolumePercentage;
            MarketCapHighVolumePercentage = marketCapHighVolumePercentage;
        }

        public decimal? DailyHighVolumePercentage { get; }
        public decimal? WindowHighVolumePercentage { get; }
        public decimal? MarketCapHighVolumePercentage { get; }
    }
}
