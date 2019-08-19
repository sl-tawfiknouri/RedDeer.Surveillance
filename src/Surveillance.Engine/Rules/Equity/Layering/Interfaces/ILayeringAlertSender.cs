namespace Surveillance.Engine.Rules.Rules.Equity.Layering.Interfaces
{
    using System.Threading.Tasks;

    public interface ILayeringAlertSender
    {
        Task Send(ILayeringRuleBreach breach);
    }
}