namespace Surveillance.Rules.Layering.Interfaces
{
    public interface ILayeringAlertSender
    {
        void Send(ILayeringRuleBreach breach);
    }
}