using Surveillance.Analytics.Streams.Interfaces;
using Surveillance.RuleParameters.Interfaces;
using Surveillance.Rules;
using Surveillance.Rules.Layering.Interfaces;
using Surveillance.Systems.Auditing.Context.Interfaces;

namespace Surveillance.Factories.Interfaces
{
    public interface ILayeringRuleFactory
    {
        ILayeringRule Build(
            ILayeringRuleParameters parameters,
            ISystemProcessOperationRunRuleContext ruleCtx,
            IUniverseAlertStream alertStream,
            RuleRunMode runMode);
    }
}