using System.Threading.Tasks;

namespace Surveillance.Engine.Rules.Queues.Interfaces
{
    public interface IQueueRuleCancellationInfrastructureBuilder
    {
        Task Setup();
    }
}