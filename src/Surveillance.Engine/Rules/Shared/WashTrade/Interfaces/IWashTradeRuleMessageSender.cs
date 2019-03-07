using System.Threading.Tasks;

namespace Surveillance.Engine.Rules.Rules.Shared.WashTrade.Interfaces
{
    public interface IWashTradeRuleMessageSender
    {
        Task Send(IWashTradeRuleBreach breach);
    }
}
