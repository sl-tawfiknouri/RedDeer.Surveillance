namespace Surveillance.Engine.Rules.Rules.Equity.Layering.Interfaces
{
    public interface ILayeringCachedMessageSender
    {
        int Flush();
        void Send(ILayeringRuleBreach ruleBreach);
    }
}