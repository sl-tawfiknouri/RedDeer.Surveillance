using Surveillance.Auditing.Context.Interfaces;
using Surveillance.Engine.Rules.Analytics.Streams.Interfaces;
using Surveillance.Engine.Rules.Rules;
using Surveillance.Engine.Rules.Rules.MarkingTheClose.Interfaces;

namespace Surveillance.Engine.Rules.Factories.Interfaces
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