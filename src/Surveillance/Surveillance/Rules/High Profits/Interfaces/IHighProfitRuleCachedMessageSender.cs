namespace Surveillance.Rules.High_Profits.Interfaces
{
    public interface IHighProfitRuleCachedMessageSender
    {
        int Flush();
        void Send(IHighProfitRuleBreach ruleBreach);
    }
}