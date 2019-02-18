using Surveillance.Engine.Rules.Trades.Interfaces;

namespace Surveillance.Engine.Rules.Rules.HighProfits.Interfaces
{
    public interface IHighProfitRuleCachedMessageSender
    {
        int Flush();
        void Send(IHighProfitRuleBreach ruleBreach);
        void Remove(ITradePosition position);
        void Delete();
    }
}