using System.Threading.Tasks;
using Surveillance.System.Auditing.Context.Interfaces;

namespace Surveillance.Rules.Layering.Interfaces
{
    public interface ILayeringAlertSender
    {
        Task Send(ILayeringRuleBreach breach, ISystemProcessOperationRunRuleContext opCtx);
    }
}