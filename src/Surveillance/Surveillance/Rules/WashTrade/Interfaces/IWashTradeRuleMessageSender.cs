using System.Threading.Tasks;

namespace Surveillance.Rules.WashTrade.Interfaces
{
    public interface IWashTradeRuleMessageSender
    {
        Task Send(IWashTradeRuleBreach breach);
    }
}
