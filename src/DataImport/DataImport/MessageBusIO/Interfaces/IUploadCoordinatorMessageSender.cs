using System.Threading.Tasks;
using SharedKernel.Contracts.Queues;

namespace DataImport.MessageBusIO.Interfaces
{
    public interface IUploadCoordinatorMessageSender
    {
        Task Send(AutoScheduleMessage message);
    }
}