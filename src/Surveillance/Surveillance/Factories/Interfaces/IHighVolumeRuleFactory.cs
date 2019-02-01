using Surveillance.Analytics.Streams.Interfaces;
using Surveillance.Data.Subscribers.Interfaces;
using Surveillance.RuleParameters.Interfaces;
using Surveillance.Rules;
using Surveillance.Rules.HighVolume.Interfaces;
using Surveillance.Systems.Auditing.Context.Interfaces;

namespace Surveillance.Factories.Interfaces
{
    public interface IHighVolumeRuleFactory
    {
        IHighVolumeRule Build(
            IHighVolumeRuleParameters parameters,
            ISystemProcessOperationRunRuleContext opCtx,
            IUniverseAlertStream alertStream,
            IUniverseDataRequestsSubscriber dataRequestSubscriber,
            RuleRunMode runMode);
    }
}