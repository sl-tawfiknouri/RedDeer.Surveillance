using System.Threading.Tasks;
using Contracts.SurveillanceService;

namespace Surveillance.Engine.Rules.MessageBusIO.Interfaces
{
    public interface ICaseMessageSender
    {
        Task Send(CaseMessage message);
    }
}