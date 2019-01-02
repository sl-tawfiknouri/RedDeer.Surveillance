using System.Threading.Tasks;
using Surveillance.System.Auditing.Context.Interfaces;

namespace Surveillance.Rules.Spoofing.Interfaces
{
    public interface ISpoofingRuleMessageSender
    {
        Task Send(ISpoofingRuleBreach ruleBreach, ISystemProcessOperationRunRuleContext opCtx);
    }
}