using System.Threading.Tasks;

namespace Surveillance.Engine.Rules.Rules.Equity.WashTrade.Interfaces
{
    public interface IWashTradeRuleMessageSender
    {
        Task Send(IWashTradeRuleBreach breach);
    }
}
