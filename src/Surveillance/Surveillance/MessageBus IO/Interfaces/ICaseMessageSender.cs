using System.Threading.Tasks;
using MessageBusDtos.Surveillance;

namespace Surveillance.MessageBus_IO.Interfaces
{
    public interface ICaseMessageSender
    {
        Task Send(CaseMessage message);
    }
}