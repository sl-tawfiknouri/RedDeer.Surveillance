namespace Surveillance.Engine.DataCoordinator.Queues.Interfaces
{
    using System.Threading.Tasks;

    public interface IQueueSubscriber
    {
        Task ExecuteCoordinationMessage(string messageId, string messageBody);

        void Initiate();

        void Terminate();
    }
}