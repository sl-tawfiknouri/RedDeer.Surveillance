using System.Threading.Tasks;
using Domain.Contracts;

namespace DataImport.MessageBusIO.Interfaces
{
    public interface IUploadCoordinatorMessageSender
    {
        Task Send(AutoScheduleMessage message);
    }
}