namespace Surveillance.Engine.Rules.Rules.WashTrade.Interfaces
{
    public interface IWashTradeCachedMessageSender
    {
        int Flush();
        void Send(IWashTradeRuleBreach ruleBreach);
    }
}