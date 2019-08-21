namespace Domain.Surveillance.Judgement.Equity
{
    using Domain.Surveillance.Judgement.Equity.Interfaces;

    public class LayeringJudgement : ILayeringJudgement
    {
        public LayeringJudgement(decimal? dailyPercentageMarketVolume, decimal? windowPercentageMarketVolume)
        {
            this.DailyPercentageMarketVolume = dailyPercentageMarketVolume;
            this.WindowPercentageMarketVolume = windowPercentageMarketVolume;
        }

        public decimal? DailyPercentageMarketVolume { get; }

        public decimal? WindowPercentageMarketVolume { get; }
    }
}