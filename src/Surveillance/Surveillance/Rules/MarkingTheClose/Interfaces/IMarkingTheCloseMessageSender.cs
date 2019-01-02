using System.Threading.Tasks;
using Surveillance.System.Auditing.Context.Interfaces;

namespace Surveillance.Rules.MarkingTheClose.Interfaces
{
    public interface IMarkingTheCloseMessageSender
    {
        Task Send(IMarkingTheCloseBreach breach, ISystemProcessOperationRunRuleContext ruleCtx);
    }
}