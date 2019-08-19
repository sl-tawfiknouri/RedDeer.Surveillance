namespace Surveillance.Engine.Rules.Rules.Equity.Layering.Interfaces
{
    using Surveillance.Engine.Rules.RuleParameters.Equities.Interfaces;
    using Surveillance.Engine.Rules.Rules.Interfaces;

    public interface ILayeringRuleBreach : IRuleBreach
    {
        RuleBreachDescription BidirectionalTradeBreach { get; }

        RuleBreachDescription DailyVolumeTradeBreach { get; }

        ILayeringRuleEquitiesParameters EquitiesParameters { get; }

        RuleBreachDescription PriceMovementBreach { get; }

        RuleBreachDescription WindowVolumeTradeBreach { get; }
    }
}