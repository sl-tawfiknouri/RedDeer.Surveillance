using System.Threading.Tasks;
using Surveillance.System.Auditing.Context.Interfaces;

namespace Surveillance.Rules.HighProfits.Interfaces
{
    public interface IHighProfitMessageSender
    {
        Task Send(IHighProfitRuleBreach ruleBreach, ISystemProcessOperationRunRuleContext ruleCtx);
    }
}