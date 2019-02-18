using System.Threading.Tasks;

namespace Surveillance.Engine.Rules.Queues.Interfaces
{
    public interface IQueueRuleUpdatePublisher
    {
        Task Send(string ruleRunId);
    }
}