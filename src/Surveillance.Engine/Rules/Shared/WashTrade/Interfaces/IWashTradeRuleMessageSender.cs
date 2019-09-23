namespace Surveillance.Engine.Rules.Rules.Shared.WashTrade.Interfaces
{
    using System.Threading.Tasks;

    public interface IWashTradeRuleMessageSender
    {
        Task Send(IWashTradeRuleBreach breach);
    }
}