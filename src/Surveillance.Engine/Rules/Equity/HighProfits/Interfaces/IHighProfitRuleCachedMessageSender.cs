using Surveillance.Engine.Rules.Judgements.Equities.Interfaces;
using Surveillance.Engine.Rules.Trades.Interfaces;

namespace Surveillance.Engine.Rules.Rules.Equity.HighProfits.Interfaces
{
    public interface IHighProfitRuleCachedMessageSender
    {
        int Flush();
        void Send(IHighProfitJudgementContext judgementContext);
        void Remove(ITradePosition position);
        void Delete();
    }
}