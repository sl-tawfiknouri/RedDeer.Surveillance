using Surveillance.Auditing.Context.Interfaces;
using Surveillance.Engine.Rules.Analytics.Streams.Interfaces;
using Surveillance.Engine.Rules.Data.Subscribers.Interfaces;
using Surveillance.Engine.Rules.RuleParameters.Equities.Interfaces;
using Surveillance.Engine.Rules.Rules;
using Surveillance.Engine.Rules.Rules.Equity.HighVolume.Interfaces;

namespace Surveillance.Engine.Rules.Factories.Equities.Interfaces
{
    public interface IEquityRuleHighVolumeFactory
    {
        IHighVolumeRule Build(
            IHighVolumeRuleEquitiesParameters equitiesParameters,
            ISystemProcessOperationRunRuleContext operationContext,
            IUniverseAlertStream alertStream,
            IUniverseDataRequestsSubscriber dataRequestSubscriber,
            RuleRunMode runMode);
    }
}