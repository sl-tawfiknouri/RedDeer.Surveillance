namespace Surveillance.Engine.DataCoordinator.Coordinator.Interfaces
{
    using System.Threading.Tasks;

    public interface IAutoSchedule
    {
        Task Scan();
    }
}