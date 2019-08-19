namespace Surveillance.Engine.Rules.Queues.Interfaces
{
    using System.Threading.Tasks;

    public interface IQueueRuleUpdatePublisher
    {
        Task Send(string ruleRunId);
    }
}