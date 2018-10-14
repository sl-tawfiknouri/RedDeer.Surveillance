namespace Surveillance.Rules.Layering.Interfaces
{
    public interface ILayeringCachedMessageSender
    {
        int Flush();
        void Send(ILayeringRuleBreach ruleBreach);
    }
}