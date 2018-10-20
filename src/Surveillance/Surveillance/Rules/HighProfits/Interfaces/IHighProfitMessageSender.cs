namespace Surveillance.Rules.HighProfits.Interfaces
{
    public interface IHighProfitMessageSender
    {
        void Send(IHighProfitRuleBreach ruleBreach);
    }
}