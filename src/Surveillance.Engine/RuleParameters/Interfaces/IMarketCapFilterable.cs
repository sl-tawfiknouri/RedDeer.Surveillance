using Surveillance.Engine.Rules.RuleParameters.Filter;

namespace Surveillance.Engine.Rules.RuleParameters.Interfaces
{
    public interface IMarketCapFilterable
    {
        DecimalRangeRuleFilter MarketCapFilter { get; }

        bool HasMarketCapFilters();
    }
}
