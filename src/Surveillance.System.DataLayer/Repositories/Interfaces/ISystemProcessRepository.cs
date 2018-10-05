using System.Threading.Tasks;
using Surveillance.System.DataLayer.Processes.Interfaces;

namespace Surveillance.System.DataLayer.Repositories.Interfaces
{
    public interface ISystemProcessRepository
    {
        Task Create(ISystemProcess entity);
        Task Update(ISystemProcess entity);
    }
}