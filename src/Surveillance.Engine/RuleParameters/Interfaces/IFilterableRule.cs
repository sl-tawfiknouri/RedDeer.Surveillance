namespace Surveillance.Engine.Rules.RuleParameters.Interfaces
{
    using Surveillance.Engine.Rules.RuleParameters.Filter;

    public interface IFilterableRule
    {
        RuleFilter Accounts { get; set; }

        RuleFilter Funds { get; set; }

        RuleFilter Markets { get; set; }

        RuleFilter Strategies { get; set; }

        RuleFilter Traders { get; set; }

        bool HasInternalFilters();
    }
}