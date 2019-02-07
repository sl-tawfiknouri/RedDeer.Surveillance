using Surveillance.Trades.Interfaces;

namespace Surveillance.Rules.HighProfits.Interfaces
{
    public interface IHighProfitRuleCachedMessageSender
    {
        int Flush();
        void Send(IHighProfitRuleBreach ruleBreach);
        void Remove(ITradePosition position);
        void Delete();
    }
}