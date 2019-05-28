using Surveillance.Engine.Rules.RuleParameters.Equities.Interfaces;
using Surveillance.Engine.Rules.Rules.Interfaces;

namespace Surveillance.Engine.Rules.Rules.Equity.HighVolume.Interfaces
{
    public interface IHighVolumeRuleBreach : IRuleBreach
    {
        IHighVolumeRuleEquitiesParameters EquitiesParameters { get; }

        HighVolumeRuleBreach.BreachDetails DailyBreach { get; }
        HighVolumeRuleBreach.BreachDetails WindowBreach { get; }
        HighVolumeRuleBreach.BreachDetails MarketCapBreach { get; }

        decimal TotalOrdersTradedInWindow { get; }
    }
}
