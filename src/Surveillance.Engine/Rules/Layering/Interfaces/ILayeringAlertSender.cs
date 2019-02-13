using System.Threading.Tasks;

namespace Surveillance.Engine.Rules.Rules.Layering.Interfaces
{
    public interface ILayeringAlertSender
    {
        Task Send(ILayeringRuleBreach breach);
    }
}