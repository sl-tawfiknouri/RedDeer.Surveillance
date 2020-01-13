using System.Threading.Tasks;

namespace DataSynchroniser.Queues.Interfaces
{
    public interface IDataRequestSubscriber
    {
        void Initiate();

        void Terminate();

        Task Execute(string messageId, string messageBody);
    }
}