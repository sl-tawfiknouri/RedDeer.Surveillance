using System.Threading.Tasks;
using Contracts.SurveillanceService;

namespace Surveillance.MessageBusIO.Interfaces
{
    public interface ICaseMessageSender
    {
        Task Send(CaseMessage message);
    }
}