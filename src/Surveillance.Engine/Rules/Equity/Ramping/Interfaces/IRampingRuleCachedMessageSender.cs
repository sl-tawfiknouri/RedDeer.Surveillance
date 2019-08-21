namespace Surveillance.Engine.Rules.Rules.Equity.Ramping.Interfaces
{
    public interface IRampingRuleCachedMessageSender
    {
        int Flush();

        void Send(IRampingRuleBreach ruleBreach);
    }
}