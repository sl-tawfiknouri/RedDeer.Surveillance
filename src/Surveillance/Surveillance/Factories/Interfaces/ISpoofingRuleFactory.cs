using Surveillance.Analytics.Streams.Interfaces;
using Surveillance.Rules.Spoofing.Interfaces;
using Surveillance.Rule_Parameters.Interfaces;
using Surveillance.System.Auditing.Context.Interfaces;

namespace Surveillance.Factories.Interfaces
{
    public interface ISpoofingRuleFactory
    {
        ISpoofingRule Build(
            ISpoofingRuleParameters spoofingParameters,
            ISystemProcessOperationRunRuleContext ruleCtx,
            IUniverseAlertStream alertStream);

        string RuleVersion { get; }
    }
}