using System.Threading.Tasks;

namespace Surveillance.Scheduler.Interfaces
{
    public interface IReddeerDistributedRuleScheduler
    {
        Task ExecuteNonDistributedMessage(string messageId, string messageBody);
        void Initiate();
        void Terminate();
    }
}