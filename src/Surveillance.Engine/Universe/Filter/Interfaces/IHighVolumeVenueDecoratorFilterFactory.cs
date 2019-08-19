namespace Surveillance.Engine.Rules.Universe.Filter.Interfaces
{
    using SharedKernel.Contracts.Markets;

    using Surveillance.Auditing.Context.Interfaces;
    using Surveillance.Engine.Rules.Data.Subscribers.Interfaces;
    using Surveillance.Engine.Rules.RuleParameters;
    using Surveillance.Engine.Rules.RuleParameters.Filter;
    using Surveillance.Engine.Rules.Rules;

    public interface IHighVolumeVenueDecoratorFilterFactory
    {
        IHighVolumeVenueDecoratorFilter Build(
            TimeWindows timeWindows,
            IUniverseFilterService baseService,
            DecimalRangeRuleFilter venueVolumeFilterSetting,
            ISystemProcessOperationRunRuleContext ruleRunContext,
            IUniverseDataRequestsSubscriber dataRequestSubscriber,
            DataSource dataSource,
            RuleRunMode ruleRunMode);
    }
}