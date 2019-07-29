using Domain.Surveillance.Judgement.Equity.Interfaces;

namespace Domain.Surveillance.Judgement.Equity
{
    public class SpoofingJudgement : ISpoofingJudgement
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
