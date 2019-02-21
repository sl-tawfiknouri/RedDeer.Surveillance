using System.Threading.Tasks;
using Contracts.SurveillanceService;

namespace Surveillance.Engine.Rules.Queues.Interfaces
{
    public interface IQueueCasePublisher
    {
        Task Send(CaseMessage message);
    }
}