using System.Threading.Tasks;

namespace Surveillance.Engine.Rules.MessageBusIO.Interfaces
{
    public interface IDataRequestMessageSender
    {
        Task Send(string ruleRunId);
    }
}