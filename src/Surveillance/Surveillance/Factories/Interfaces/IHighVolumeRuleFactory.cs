using Surveillance.Analytics.Streams.Interfaces;
using Surveillance.Rule_Parameters.Interfaces;
using Surveillance.Rules.HighVolume.Interfaces;
using Surveillance.System.Auditing.Context.Interfaces;

namespace Surveillance.Factories.Interfaces
{
    public interface IHighVolumeRuleFactory
    {
        string RuleVersion { get; }

        IHighVolumeRule Build(IHighVolumeRuleParameters parameters, ISystemProcessOperationRunRuleContext opCtx, IUniverseAlertStream alertStream);
    }
}