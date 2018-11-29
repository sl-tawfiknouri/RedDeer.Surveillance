using Surveillance.RuleParameters.Filter;

namespace Surveillance.RuleParameters.Interfaces
{
    public interface IFilterableRule
    {
        RuleFilter Accounts { get; set; }
        RuleFilter Traders { get; set; }
        RuleFilter Markets { get; set; }

        bool HasFilters();
    }
}
