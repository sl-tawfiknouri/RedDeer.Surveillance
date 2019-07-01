using Surveillance.Engine.Rules.RuleParameters.Filter;

namespace Surveillance.Engine.Rules.RuleParameters.Interfaces
{
    public interface IFilterableRule
    {
        RuleFilter Accounts { get; set; }
        RuleFilter Traders { get; set; }
        RuleFilter Markets { get; set; }
        RuleFilter Funds { get; set; }
        RuleFilter Strategies { get; set; }

        bool HasInternalFilters();
    }
}
