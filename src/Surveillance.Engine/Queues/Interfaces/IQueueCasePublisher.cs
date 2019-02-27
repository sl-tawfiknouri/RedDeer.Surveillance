using System.Threading.Tasks;
using RedDeer.Contracts.SurveillanceService;

namespace Surveillance.Engine.Rules.Queues.Interfaces
{
    public interface IQueueCasePublisher
    {
        Task Send(CaseMessage message);
    }
}