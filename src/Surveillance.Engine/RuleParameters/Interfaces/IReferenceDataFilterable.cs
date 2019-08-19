namespace Surveillance.Engine.Rules.RuleParameters.Interfaces
{
    using Surveillance.Engine.Rules.RuleParameters.Filter;

    public interface IReferenceDataFilterable
    {
        RuleFilter Countries { get; set; }

        RuleFilter Industries { get; set; }

        RuleFilter Regions { get; set; }

        RuleFilter Sectors { get; set; }

        bool HasReferenceDataFilters();
    }
}