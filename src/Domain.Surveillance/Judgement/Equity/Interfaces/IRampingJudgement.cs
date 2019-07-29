namespace Domain.Surveillance.Judgement.Equity.Interfaces
{
    public interface IRampingJudgement
    {
        decimal? AutoCorrelationCoefficient { get; }
    }
}