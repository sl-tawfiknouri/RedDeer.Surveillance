namespace Domain.Surveillance.Judgement.Equity.Interfaces
{
    public interface IMarkingTheCloseJudgement
    {
        decimal? DailyPercentageMarketVolume { get; }
        decimal? ThresholdOffTouchPercentage { get; }
        decimal? WindowPercentageMarketVolume { get; }
    }
}