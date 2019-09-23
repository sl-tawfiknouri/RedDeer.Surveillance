namespace Domain.Surveillance.Judgement.Equity
{
    using Domain.Surveillance.Judgement.Equity.Interfaces;

    public class HighVolumeJudgement : IHighVolumeJudgement
    {
        public HighVolumeJudgement(
            decimal? dailyHighVolumePercentage,
            decimal? windowHighVolumePercentage,
            decimal? marketCapHighVolumePercentage)
        {
            this.DailyHighVolumePercentage = dailyHighVolumePercentage;
            this.WindowHighVolumePercentage = windowHighVolumePercentage;
            this.MarketCapHighVolumePercentage = marketCapHighVolumePercentage;
        }

        public decimal? DailyHighVolumePercentage { get; }

        public decimal? MarketCapHighVolumePercentage { get; }

        public decimal? WindowHighVolumePercentage { get; }
    }
}