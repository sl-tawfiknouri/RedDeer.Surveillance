namespace Domain.Surveillance.Judgement.Equity
{
    public class PlacingOrdersWithNoIntentToExecuteJudgement
    {
        public PlacingOrdersWithNoIntentToExecuteJudgement(decimal? sigma)
        {
            Sigma = sigma;
        }

        public decimal? Sigma { get; }
    }
}
