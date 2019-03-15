namespace Domain.Core.Trading.Execution.Interfaces
{
    public interface IJudgement
    {
        PriceSentiment Sentiment { get; }
    }
}