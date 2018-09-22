namespace Surveillance.Rules.High_Profits.Interfaces
{
    public interface IHighProfitMessageSender
    {
        void Send(IHighProfitRuleBreach ruleBreach);
    }
}