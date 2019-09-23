namespace Domain.Surveillance.Judgement.Equity
{
    using Domain.Surveillance.Judgement.Equity.Interfaces;

    public class RampingJudgement : IRampingJudgement
    {
        public RampingJudgement(decimal? autoCorrelationCoefficient)
        {
            this.AutoCorrelationCoefficient = autoCorrelationCoefficient;
        }

        public decimal? AutoCorrelationCoefficient { get; }
    }
}