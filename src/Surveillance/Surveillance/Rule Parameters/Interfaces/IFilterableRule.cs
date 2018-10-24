using Surveillance.Rule_Parameters.Filter;

namespace Surveillance.Rule_Parameters.Interfaces
{
    public interface IFilterableRule
    {
        RuleFilter Accounts { get; set; }
        RuleFilter Traders { get; set; }
        RuleFilter Markets { get; set; }

        bool HasFilters();
    }
}
