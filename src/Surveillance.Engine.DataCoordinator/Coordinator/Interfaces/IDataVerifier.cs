using System.Threading.Tasks;

namespace Surveillance.Engine.DataCoordinator.Coordinator.Interfaces
{
    public interface IDataVerifier
    {
        Task Scan();
    }
}