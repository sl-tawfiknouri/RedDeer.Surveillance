namespace DataImport.MessageBusIO.Interfaces
{
    using System.Threading.Tasks;

    using SharedKernel.Contracts.Queues;

    public interface IUploadCoordinatorMessageSender
    {
        Task Send(AutoScheduleMessage message);
    }
}