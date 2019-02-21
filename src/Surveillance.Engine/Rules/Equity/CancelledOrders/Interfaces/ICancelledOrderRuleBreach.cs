using Surveillance.Engine.Rules.RuleParameters.Interfaces;
using Surveillance.Engine.Rules.Rules.Interfaces;

namespace Surveillance.Engine.Rules.Rules.Equity.CancelledOrders.Interfaces
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