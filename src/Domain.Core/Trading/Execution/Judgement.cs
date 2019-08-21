namespace Domain.Core.Trading.Execution
{
    using Domain.Core.Trading.Execution.Interfaces;

    public class Judgement : IJudgement
    {
        public Judgement(PriceSentiment sentiment)
        {
            this.Sentiment = sentiment;
        }

        public PriceSentiment Sentiment { get; }
    }
}