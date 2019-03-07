namespace Surveillance.Engine.Rules.Rules.Equity.WashTrade.Interfaces
{
    public interface IWashTradeCachedMessageSender
    {
        int Flush();
        void Send(IWashTradeRuleBreach ruleBreach);
    }
}