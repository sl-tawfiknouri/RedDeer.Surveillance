using System.Collections.Generic;
using System.Threading.Tasks;
using Surveillance.Systems.DataLayer.Processes.Interfaces;

namespace Surveillance.Systems.DataLayer.Repositories.Interfaces
{
    public interface ISystemProcessOperationUploadFileRepository
    {
        Task Create(ISystemProcessOperationUploadFile entity);
        Task<IReadOnlyCollection<ISystemProcessOperationUploadFile>> GetDashboard();
    }
}