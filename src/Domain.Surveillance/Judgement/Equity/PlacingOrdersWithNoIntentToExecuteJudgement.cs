using Domain.Surveillance.Judgement.Equity.Interfaces;

namespace Domain.Surveillance.Judgement.Equity
{
    public class PlacingOrdersWithNoIntentToExecuteJudgement : IPlacingOrdersWithNoIntentToExecuteJudgement
    {
        public PlacingOrdersWithNoIntentToExecuteJudgement(decimal? sigma)
        {
            Sigma = sigma;
        }

        public decimal? Sigma { get; }
    }
}
