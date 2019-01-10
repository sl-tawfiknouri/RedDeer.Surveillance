using System.Threading.Tasks;

namespace Surveillance.Rules.Layering.Interfaces
{
    public interface ILayeringAlertSender
    {
        Task Send(ILayeringRuleBreach breach);
    }
}