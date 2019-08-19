namespace Surveillance.Engine.Rules.Queues.Interfaces
{
    using System.Threading.Tasks;

    using RedDeer.Contracts.SurveillanceService;

    public interface IQueueCasePublisher
    {
        Task Send(CaseMessage message);
    }
}