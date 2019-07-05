namespace Domain.Surveillance.Judgements.Equity
{
    public class RampingJudgement
    {
        public RampingJudgement(decimal? autoCorrelationCoefficient)
        {
            AutoCorrelationCoefficient = autoCorrelationCoefficient;
        }

        public decimal? AutoCorrelationCoefficient { get; }
    }
}
