namespace Surveillance.Engine.Rules.RuleParameters.Filter
{
    using System;

    using RedDeer.Contracts.SurveillanceService.Api.RuleParameter;

    using Surveillance.Engine.Rules.RuleParameters.Filter.Interfaces;
    using Range = RedDeer.Contracts.SurveillanceService.Api.RuleParameter.Range;

    /// <summary>
    /// The decimal range rule filter projector.
    /// </summary>
    public class DecimalRangeRuleFilterProjector : IDecimalRangeRuleFilterProjector
    {
        /// <summary>
        /// The project.
        /// </summary>
        /// <param name="filter">
        /// The filter.
        /// </param>
        /// <returns>
        /// The <see cref="DecimalRangeRuleFilter"/>.
        /// </returns>
        public DecimalRangeRuleFilter Project(Range filter)
        {
            if (filter == null)
            {
                return DecimalRangeRuleFilter.None();
            }

            if (filter.LowerBoundary == null && filter.UpperBoundary == null)
            {
                return DecimalRangeRuleFilter.None();
            }

            if (filter.LowerBoundary > filter.UpperBoundary)
            {
                throw new ArgumentException(nameof(filter));
            }

            return new DecimalRangeRuleFilter
               {
                   Min = filter.LowerBoundary,
                   Max = filter.UpperBoundary,
                   Type = RuleFilterType.Include
               };
        }
    }
}