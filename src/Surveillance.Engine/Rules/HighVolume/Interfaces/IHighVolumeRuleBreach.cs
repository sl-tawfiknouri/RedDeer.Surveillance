using Surveillance.Engine.Rules.RuleParameters.Interfaces;
using Surveillance.Engine.Rules.Rules.Interfaces;

namespace Surveillance.Engine.Rules.Rules.HighVolume.Interfaces
{
    public interface IHighVolumeRuleBreach : IRuleBreach
    {
        IHighVolumeRuleParameters Parameters { get; }

        HighVolumeRuleBreach.BreachDetails DailyBreach { get; }
        HighVolumeRuleBreach.BreachDetails WindowBreach { get; }
        HighVolumeRuleBreach.BreachDetails MarketCapBreach { get; }

        long TotalOrdersTradedInWindow { get; }
    }
}
