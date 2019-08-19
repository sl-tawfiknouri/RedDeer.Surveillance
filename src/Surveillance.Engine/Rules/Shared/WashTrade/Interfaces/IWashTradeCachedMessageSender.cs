namespace Surveillance.Engine.Rules.Rules.Shared.WashTrade.Interfaces
{
    public interface IWashTradeCachedMessageSender
    {
        int Flush();

        void Send(IWashTradeRuleBreach ruleBreach);
    }
}