using Surveillance.Rules.Marking_The_Close.Interfaces;
using Surveillance.System.Auditing.Context.Interfaces;

namespace Surveillance.Factories.Interfaces
{
    public interface IMarkingTheCloseRuleFactory
    {
        IMarkingTheCloseRule Build(IMarkingTheCloseParameters parameters, ISystemProcessOperationRunRuleContext ruleCtx);

        string RuleVersion { get; }
    }
}