using Surveillance.Engine.Rules.RuleParameters.Filter;

namespace Surveillance.Engine.Rules.Universe.Filter.Interfaces
{
    public interface IUniverseFilterFactory
    {
        IUniverseFilterService Build(
            RuleFilter accounts,
            RuleFilter traders,
            RuleFilter markets,
            RuleFilter funds,
            RuleFilter strategies,
            RuleFilter sectors,
            RuleFilter industries,
            RuleFilter regions,
            RuleFilter countries);
    }
}