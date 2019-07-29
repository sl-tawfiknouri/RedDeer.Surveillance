namespace Domain.Surveillance.Judgement.Equity.Interfaces
{
    public interface ICancelledOrderJudgement
    {
        decimal? CancelledOrderCountPercentageThreshold { get; }
        decimal? CancelledOrderPercentagePositionThreshold { get; }
    }
}