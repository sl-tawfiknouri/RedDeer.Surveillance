using RedDeer.Contracts.SurveillanceService.Api.RuleParameter;
using Surveillance.Engine.Rules.RuleParameters.Filter.Interfaces;

namespace Surveillance.Engine.Rules.RuleParameters.Filter
{
    public class DecimalRangeRuleFilterProjector : IDecimalRangeRuleFilterProjector
    {
        public DecimalRangeRuleFilter Project(Range filter)
        {
            if (filter == null)
            {
                return DecimalRangeRuleFilter.None();
            }

            if(filter.LowerBoundary == null && filter.UpperBoundary == null)
            {
                return DecimalRangeRuleFilter.None();
            }

            return new DecimalRangeRuleFilter()
            {
                Min = filter.LowerBoundary,
                Max = filter.UpperBoundary,
                Type = RuleFilterType.Include
            };
        }
    }
}
