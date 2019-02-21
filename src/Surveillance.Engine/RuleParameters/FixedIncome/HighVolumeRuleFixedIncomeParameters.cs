using Surveillance.Engine.Rules.RuleParameters.Filter;
using Surveillance.Engine.Rules.RuleParameters.FixedIncome.Interfaces;

namespace Surveillance.Engine.Rules.RuleParameters.FixedIncome
{
    public class HighVolumeRuleFixedIncomeParameters : IHighVolumeRuleFixedIncomeParameters
    {
        public HighVolumeRuleFixedIncomeParameters(
            string id,
            RuleFilter accounts,
            RuleFilter traders,
            RuleFilter markets)
        {
            Id = id ?? string.Empty;
            Accounts = accounts ?? RuleFilter.None();
            Traders = traders ?? RuleFilter.None();
            Markets = markets ?? RuleFilter.None();
        }

        public string Id { get; }
        public RuleFilter Accounts { get; set; }
        public RuleFilter Traders { get; set; }
        public RuleFilter Markets { get; set; }

        public bool HasFilters()
        {
            return
                Accounts?.Type != RuleFilterType.None
                || Traders?.Type != RuleFilterType.None
                || Markets?.Type != RuleFilterType.None;
        }
    }
}
