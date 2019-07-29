using Domain.Surveillance.Judgement.Equity.Interfaces;

namespace Domain.Surveillance.Judgement.Equity
{
    public class CancelledOrderJudgement : ICancelledOrderJudgement
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
