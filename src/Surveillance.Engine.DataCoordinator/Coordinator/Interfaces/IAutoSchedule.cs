using System.Threading.Tasks;

namespace Surveillance.Engine.DataCoordinator.Coordinator.Interfaces
{
    public interface IAutoSchedule
    {
        Task ScheduleExecution(string fileId);
    }
}