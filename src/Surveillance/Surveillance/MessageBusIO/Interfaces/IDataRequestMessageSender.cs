using System.Threading.Tasks;

namespace Surveillance.MessageBusIO.Interfaces
{
    public interface IDataRequestMessageSender
    {
        Task Send(string ruleRunId);
    }
}