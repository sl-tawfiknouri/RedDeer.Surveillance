using Surveillance.RuleParameters.Interfaces;
using Surveillance.Rules.Interfaces;

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
