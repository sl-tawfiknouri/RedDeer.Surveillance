using Surveillance.Auditing.Context.Interfaces;
using Surveillance.Engine.Rules.Data.Subscribers.Interfaces;
using Surveillance.Engine.Rules.RuleParameters;
using Surveillance.Engine.Rules.RuleParameters.Filter;
using Surveillance.Engine.Rules.Rules;

namespace Surveillance.Engine.Rules.Universe.Filter.Interfaces
{
    public interface IHighVolumeVenueDecoratorFilterFactory
    {
        IHighVolumeVenueDecoratorFilter Build(
            TimeWindows timeWindows,
            IUniverseFilterService baseService,
            DecimalRangeRuleFilter venueVolumeFilterSetting,
            ISystemProcessOperationRunRuleContext ruleRunContext,
            IUniverseDataRequestsSubscriber dataRequestSubscriber,
            RuleRunMode ruleRunMode);
    }
}