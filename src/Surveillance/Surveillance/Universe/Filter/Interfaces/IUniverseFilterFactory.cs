using Surveillance.RuleParameters.Filter;

namespace Surveillance.Universe.Filter.Interfaces
{
    public interface IUniverseFilterFactory
    {
        IUniverseFilter Build(
            RuleFilter accounts,
            RuleFilter traders,
            RuleFilter markets);
    }
}