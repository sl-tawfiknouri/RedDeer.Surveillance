using Surveillance.Engine.Rules.RuleParameters.Interfaces;
using Surveillance.Engine.Rules.Rules.Interfaces;

namespace Surveillance.Engine.Rules.Rules.Layering.Interfaces
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
