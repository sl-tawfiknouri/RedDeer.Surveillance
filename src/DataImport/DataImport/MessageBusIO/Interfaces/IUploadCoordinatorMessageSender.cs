using System.Threading.Tasks;
using DomainV2.Contracts;

namespace DataImport.MessageBusIO.Interfaces
{
    public interface IUploadCoordinatorMessageSender
    {
        Task Send(UploadCoordinatorMessage message);
    }
}