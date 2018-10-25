using Surveillance.System.Auditing.Context.Interfaces;

namespace Surveillance.Rules.MarkingTheClose.Interfaces
{
    public interface IMarkingTheCloseMessageSender
    {
        void Send(IMarkingTheCloseBreach breach, ISystemProcessOperationRunRuleContext ruleCtx);
    }
}