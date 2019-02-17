using System.Threading.Tasks;

namespace Surveillance.Engine.Rules.Queues.Interfaces
{
    public interface IQueueDataSynchroniserRequestPublisher
    {
        Task Send(string ruleRunId);
    }
}