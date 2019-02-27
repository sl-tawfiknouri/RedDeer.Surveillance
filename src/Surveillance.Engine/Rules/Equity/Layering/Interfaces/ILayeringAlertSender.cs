using System.Threading.Tasks;

namespace Surveillance.Engine.Rules.Rules.Equity.Layering.Interfaces
{
    public interface ILayeringAlertSender
    {
        Task Send(ILayeringRuleBreach breach);
    }
}