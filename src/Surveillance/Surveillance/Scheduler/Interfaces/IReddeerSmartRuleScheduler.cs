using System.Threading.Tasks;

namespace Surveillance.Scheduler.Interfaces
{
    public interface IReddeerSmartRuleScheduler
    {
        Task ExecuteNonDistributedMessage(string messageId, string messageBody);
        void Initiate();
        void Terminate();
    }
}