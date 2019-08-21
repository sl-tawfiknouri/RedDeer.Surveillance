namespace Domain.Surveillance.Judgement.Equity
{
    using Domain.Surveillance.Judgement.Equity.Interfaces;

    public class CancelledOrderJudgement : ICancelledOrderJudgement
    {
        public CancelledOrderJudgement(
            decimal? cancelledOrderPercentagePositionThreshold,
            decimal? cancelledOrderCountPercentageThreshold)
        {
            this.CancelledOrderPercentagePositionThreshold = cancelledOrderPercentagePositionThreshold;
            this.CancelledOrderCountPercentageThreshold = cancelledOrderCountPercentageThreshold;
        }

        public decimal? CancelledOrderCountPercentageThreshold { get; }

        public decimal? CancelledOrderPercentagePositionThreshold { get; }
    }
}