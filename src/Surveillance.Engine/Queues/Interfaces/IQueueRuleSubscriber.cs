using System.Threading.Tasks;

namespace Surveillance.Engine.Rules.Queues.Interfaces
{
    public interface IQueueRuleSubscriber
    {
        void Initiate();

        void Terminate();

        Task ExecuteDistributedMessage(string messageId, string messageBody);
    }
}