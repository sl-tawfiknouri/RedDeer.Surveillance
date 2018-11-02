using Surveillance.Rules.Interfaces;
using Surveillance.Rule_Parameters.Interfaces;

namespace Surveillance.Rules.CancelledOrders.Interfaces
{
    public interface ICancelledOrderRuleBreach : IRuleBreach
    {
        ICancelledOrderRuleParameters Parameters { get; }

        bool HasBreachedRule();
        bool ExceededPercentagePositionCancellations { get; }
        decimal? PercentagePositionCancelled { get; }
        int? AmountOfPositionCancelled { get; }
        int? AmountOfPositionInTotal { get; }
        bool ExceededPercentageTradeCountCancellations { get; }
        decimal? PercentageTradeCountCancelled { get; }
    }
}