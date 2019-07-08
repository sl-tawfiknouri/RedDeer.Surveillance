namespace Domain.Surveillance.Judgement.Equity
{
    public class HighVolumeJudgement
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
