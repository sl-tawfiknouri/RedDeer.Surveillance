using Surveillance.Engine.Rules.RuleParameters.Interfaces;
using Surveillance.Engine.Rules.Rules.Interfaces;

namespace Surveillance.Engine.Rules.Rules.Equity.Layering.Interfaces
{
    public interface ILayeringRuleBreach : IRuleBreach
    {
        ILayeringRuleEquitiesParameters EquitiesParameters { get; }
        RuleBreachDescription BidirectionalTradeBreach { get; }
        RuleBreachDescription DailyVolumeTradeBreach { get; }
        RuleBreachDescription WindowVolumeTradeBreach { get; }
        RuleBreachDescription PriceMovementBreach { get; }
    }
}
