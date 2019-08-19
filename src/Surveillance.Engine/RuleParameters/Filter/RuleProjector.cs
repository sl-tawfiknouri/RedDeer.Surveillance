namespace Surveillance.Engine.Rules.RuleParameters.Filter
{
    using RedDeer.Contracts.SurveillanceService.Api.RuleParameter;

    using Surveillance.Engine.Rules.RuleParameters.Filter.Interfaces;

    public class RuleProjector : IRuleProjector
    {
        public RuleFilter Project(Filter filter)
        {
            if (filter == null) return RuleFilter.None();

            var type = RuleFilterType.None;
            switch (filter.Type)
            {
                case FilterType.None:
                    type = RuleFilterType.None;
                    break;
                case FilterType.Include:
                    type = RuleFilterType.Include;
                    break;
                case FilterType.Exclude:
                    type = RuleFilterType.Exclude;
                    break;
            }

            return new RuleFilter { Type = type, Ids = filter.Ids ?? new string[0] };
        }
    }
}