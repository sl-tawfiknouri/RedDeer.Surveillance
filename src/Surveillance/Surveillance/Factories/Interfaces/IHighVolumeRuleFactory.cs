using Surveillance.Analytics.Streams.Interfaces;
using Surveillance.Rule_Parameters.Interfaces;
using Surveillance.Rules.HighVolume.Interfaces;
using Surveillance.System.Auditing.Context.Interfaces;

namespace Surveillance.Factories.Interfaces
{
    public interface IHighVolumeRuleFactory
    {
        IHighVolumeRule Build(IHighVolumeRuleParameters parameters, ISystemProcessOperationRunRuleContext opCtx, IUniverseAlertStream alertStream);
    }
}