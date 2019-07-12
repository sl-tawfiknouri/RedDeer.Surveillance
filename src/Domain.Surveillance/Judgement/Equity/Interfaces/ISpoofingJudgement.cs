namespace Domain.Surveillance.Judgement.Equity.Interfaces
{
    public interface ISpoofingJudgement
    {
        decimal? CancellationThreshold { get; }
        decimal? RelativeSizeSpoofExceedingReal { get; }
    }
}