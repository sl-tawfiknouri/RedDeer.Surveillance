namespace Domain.Surveillance.Judgement.Equity.Interfaces
{
    public interface ILayeringJudgement
    {
        decimal? DailyPercentageMarketVolume { get; }
        decimal? WindowPercentageMarketVolume { get; }
    }
}