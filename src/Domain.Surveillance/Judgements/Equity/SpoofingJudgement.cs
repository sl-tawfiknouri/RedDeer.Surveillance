namespace Domain.Surveillance.Judgements.Equity
{
    public class SpoofingJudgement
    {
        public SpoofingJudgement(decimal? cancellationThreshold, decimal? relativeSizeSpoofExceedingReal)
        {
            CancellationThreshold = cancellationThreshold;
            RelativeSizeSpoofExceedingReal = relativeSizeSpoofExceedingReal;
        }

        public decimal? CancellationThreshold { get; }
        public decimal? RelativeSizeSpoofExceedingReal { get; }
    }
}
