using System.Threading.Tasks;
using Contracts.SurveillanceService;

namespace Surveillance.MessageBus_IO.Interfaces
{
    public interface ICaseMessageSender
    {
        Task Send(CaseMessage message);
    }
}