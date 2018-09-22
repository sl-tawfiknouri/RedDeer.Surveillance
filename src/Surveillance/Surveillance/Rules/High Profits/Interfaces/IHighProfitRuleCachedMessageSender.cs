namespace Surveillance.Rules.High_Profits.Interfaces
{
    public interface IHighProfitRuleCachedMessageSender
    {
        void Flush();
        void Send(IHighProfitRuleBreach ruleBreach);
    }
}