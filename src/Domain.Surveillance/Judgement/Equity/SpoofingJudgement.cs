namespace Domain.Surveillance.Judgement.Equity
{
    using Domain.Surveillance.Judgement.Equity.Interfaces;

    public class SpoofingJudgement : ISpoofingJudgement
    {
        public SpoofingJudgement(decimal? cancellationThreshold, decimal? relativeSizeSpoofExceedingReal)
        {
            this.CancellationThreshold = cancellationThreshold;
            this.RelativeSizeSpoofExceedingReal = relativeSizeSpoofExceedingReal;
        }

        public decimal? CancellationThreshold { get; }

        public decimal? RelativeSizeSpoofExceedingReal { get; }
    }
}