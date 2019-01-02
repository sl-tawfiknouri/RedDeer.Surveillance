using System.Threading.Tasks;
using Surveillance.System.Auditing.Context.Interfaces;

namespace Surveillance.Rules.WashTrade.Interfaces
{
    public interface IWashTradeRuleMessageSender
    {
        Task Send(IWashTradeRuleBreach breach, ISystemProcessOperationRunRuleContext opCtx);
    }
}
