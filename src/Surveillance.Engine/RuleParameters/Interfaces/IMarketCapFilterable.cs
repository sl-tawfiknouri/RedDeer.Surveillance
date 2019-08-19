namespace Surveillance.Engine.Rules.RuleParameters.Interfaces
{
    using Surveillance.Engine.Rules.RuleParameters.Filter;

    public interface IMarketCapFilterable
    {
        DecimalRangeRuleFilter MarketCapFilter { get; }

        bool HasMarketCapFilters();
    }
}