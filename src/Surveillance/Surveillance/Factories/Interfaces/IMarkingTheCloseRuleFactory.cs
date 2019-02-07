using Surveillance.Analytics.Streams.Interfaces;
using Surveillance.Rules;
using Surveillance.Rules.MarkingTheClose.Interfaces;
using Surveillance.Systems.Auditing.Context.Interfaces;

namespace Surveillance.Factories.Interfaces
{
    public interface IMarkingTheCloseRuleFactory
    {
        IMarkingTheCloseRule Build(
            IMarkingTheCloseParameters parameters,
            ISystemProcessOperationRunRuleContext ruleCtx,
            IUniverseAlertStream alertStream,
            RuleRunMode runMode);
    }
}