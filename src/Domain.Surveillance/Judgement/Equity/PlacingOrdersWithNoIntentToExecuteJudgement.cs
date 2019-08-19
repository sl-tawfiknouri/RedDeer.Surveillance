namespace Domain.Surveillance.Judgement.Equity
{
    using Domain.Surveillance.Judgement.Equity.Interfaces;

    public class PlacingOrdersWithNoIntentToExecuteJudgement : IPlacingOrdersWithNoIntentToExecuteJudgement
    {
        public PlacingOrdersWithNoIntentToExecuteJudgement(decimal? sigma)
        {
            this.Sigma = sigma;
        }

        public decimal? Sigma { get; }
    }
}