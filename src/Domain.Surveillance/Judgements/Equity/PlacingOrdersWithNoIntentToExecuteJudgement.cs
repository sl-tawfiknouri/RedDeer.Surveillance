namespace Domain.Surveillance.Judgements.Equity
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
