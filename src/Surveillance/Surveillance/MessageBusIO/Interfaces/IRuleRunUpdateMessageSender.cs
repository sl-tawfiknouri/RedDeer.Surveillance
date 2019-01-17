using System.Threading.Tasks;

namespace Surveillance.MessageBusIO.Interfaces
{
    public interface IRuleRunUpdateMessageSender
    {
        Task Send(string ruleRunId);
    }
}