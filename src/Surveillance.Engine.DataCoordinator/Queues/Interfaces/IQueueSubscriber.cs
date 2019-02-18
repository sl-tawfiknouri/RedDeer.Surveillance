using System.Threading.Tasks;

namespace Surveillance.Engine.DataCoordinator.Queues.Interfaces
{
    public interface IQueueSubscriber
    {
        void Initiate();
        void Terminate();
        Task ExecuteCoordinationMessage(string messageId, string messageBody);
    }
}