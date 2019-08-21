namespace Surveillance.Engine.Rules.Rules.Equity.HighVolume.Interfaces
{
    using Surveillance.Engine.Rules.RuleParameters.Equities.Interfaces;
    using Surveillance.Engine.Rules.Rules.Interfaces;

    public interface IHighVolumeRuleBreach : IRuleBreach
    {
        HighVolumeRuleBreach.BreachDetails DailyBreach { get; }

        IHighVolumeRuleEquitiesParameters EquitiesParameters { get; }

        HighVolumeRuleBreach.BreachDetails MarketCapBreach { get; }

        decimal TotalOrdersTradedInWindow { get; }

        HighVolumeRuleBreach.BreachDetails WindowBreach { get; }
    }
}