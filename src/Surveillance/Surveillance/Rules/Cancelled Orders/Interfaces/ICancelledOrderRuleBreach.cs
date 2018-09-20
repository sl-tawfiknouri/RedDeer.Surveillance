namespace Surveillance.Rules.Cancelled_Orders.Interfaces
{
    public interface ICancelledOrderRuleBreach
    {
        bool HasBreachedRule();
        bool ExceededPercentagePositionCancellations { get; set; }
        decimal? PercentagePositionCancelled { get; set; }
        bool ExceededPercentageTradeCountCancellations { get; set; }
        decimal? PercentageTradeCountCancelled { get; set; }
    }
}