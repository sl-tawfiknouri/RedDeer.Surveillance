using System.Threading.Tasks;

namespace Surveillance.Engine.Rules.MessageBusIO.Interfaces
{
    public interface IRuleRunUpdateMessageSender
    {
        Task Send(string ruleRunId);
    }
}