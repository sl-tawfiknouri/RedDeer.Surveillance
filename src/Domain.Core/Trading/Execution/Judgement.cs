using Domain.Core.Trading.Execution.Interfaces;

namespace Domain.Core.Trading.Execution
{
    public class Judgement : IJudgement
    {
        public Judgement(PriceSentiment sentiment)
        {
            Sentiment = sentiment;
        }
        public PriceSentiment Sentiment { get; }
    }
}
