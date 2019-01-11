using Surveillance.Analytics.Streams.Interfaces;
using Surveillance.RuleParameters.Interfaces;
using Surveillance.Rules;
using Surveillance.Rules.Spoofing.Interfaces;
using Surveillance.System.Auditing.Context.Interfaces;

namespace Surveillance.Factories.Interfaces
{
    public interface ISpoofingRuleFactory
    {
        ISpoofingRule Build(
            ISpoofingRuleParameters spoofingParameters,
            ISystemProcessOperationRunRuleContext ruleCtx,
            IUniverseAlertStream alertStream,
            RuleRunMode runMode);
    }
}