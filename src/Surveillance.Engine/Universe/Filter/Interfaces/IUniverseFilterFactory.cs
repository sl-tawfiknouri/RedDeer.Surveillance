using Surveillance.Engine.Rules.RuleParameters.Filter;

namespace Surveillance.Engine.Rules.Universe.Filter.Interfaces
{
    public interface IUniverseFilterFactory
    {
        IUniverseFilter Build(
            RuleFilter accounts,
            RuleFilter traders,
            RuleFilter markets);
    }
}