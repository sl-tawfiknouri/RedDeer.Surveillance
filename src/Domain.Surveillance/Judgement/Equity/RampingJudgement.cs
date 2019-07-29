using Domain.Surveillance.Judgement.Equity.Interfaces;

namespace Domain.Surveillance.Judgement.Equity
{
    public class RampingJudgement : IRampingJudgement
    {
        public RampingJudgement(decimal? autoCorrelationCoefficient)
        {
            AutoCorrelationCoefficient = autoCorrelationCoefficient;
        }

        public decimal? AutoCorrelationCoefficient { get; }
    }
}
