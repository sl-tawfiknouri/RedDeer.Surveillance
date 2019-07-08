namespace Domain.Surveillance.Judgement.Equity
{
    public class CancelledOrderJudgement
    {
        public CancelledOrderJudgement(
            decimal? cancelledOrderPercentagePositionThreshold,
            decimal? cancelledOrderCountPercentageThreshold)
        {
            CancelledOrderPercentagePositionThreshold = cancelledOrderPercentagePositionThreshold;
            CancelledOrderCountPercentageThreshold = cancelledOrderCountPercentageThreshold;
        }

        public decimal? CancelledOrderPercentagePositionThreshold { get; }
        public decimal? CancelledOrderCountPercentageThreshold { get; }
    }
}
