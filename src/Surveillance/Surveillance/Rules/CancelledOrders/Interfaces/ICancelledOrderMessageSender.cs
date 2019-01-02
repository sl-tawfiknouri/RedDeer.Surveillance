using System.Threading.Tasks;
using Surveillance.System.Auditing.Context.Interfaces;

namespace Surveillance.Rules.CancelledOrders.Interfaces
{
    public interface ICancelledOrderMessageSender
    {
        Task Send(ICancelledOrderRuleBreach ruleBreach, ISystemProcessOperationRunRuleContext opCtx);
    }
}