using Surveillance.Rules.Interfaces;
using Surveillance.Rule_Parameters.Interfaces;

namespace Surveillance.Rules.Layering.Interfaces
{
    public interface ILayeringRuleBreach : IRuleBreach
    {
        ILayeringRuleParameters Parameters { get; }
        RuleBreachDescription BidirectionalTradeBreach { get; }
        RuleBreachDescription DailyVolumeTradeBreach { get; }
        RuleBreachDescription WindowVolumeTradeBreach { get; }
        RuleBreachDescription PriceMovementBreach { get; }
    }
}
