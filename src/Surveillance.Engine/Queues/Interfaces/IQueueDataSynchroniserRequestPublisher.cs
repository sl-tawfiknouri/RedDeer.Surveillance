namespace Surveillance.Engine.Rules.Queues.Interfaces
{
    using System.Threading.Tasks;

    public interface IQueueDataSynchroniserRequestPublisher
    {
        Task Send(string ruleRunId);
    }
}