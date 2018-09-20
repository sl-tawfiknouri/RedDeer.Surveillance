using Surveillance.Rules.Cancelled_Orders.Interfaces;

namespace Surveillance.Rules.Cancelled_Orders
{
    public class CancelledOrderRuleBreach : ICancelledOrderRuleBreach
    {
        public bool HasBreachedRule()
        {
            return ExceededPercentageTradeCountCancellations
                || ExceededPercentagePositionCancellations;
        }

        public bool ExceededPercentagePositionCancellations { get; set; }
        public decimal? PercentagePositionCancelled { get; set; }
        public bool ExceededPercentageTradeCountCancellations { get; set; }
        public decimal? PercentageTradeCountCancelled { get; set; }
    }
}
