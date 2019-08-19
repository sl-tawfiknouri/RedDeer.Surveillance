namespace Domain.Surveillance.Judgement.Equity.Interfaces
{
    public interface IHighVolumeJudgement
    {
        decimal? DailyHighVolumePercentage { get; }

        decimal? MarketCapHighVolumePercentage { get; }

        decimal? WindowHighVolumePercentage { get; }
    }
}