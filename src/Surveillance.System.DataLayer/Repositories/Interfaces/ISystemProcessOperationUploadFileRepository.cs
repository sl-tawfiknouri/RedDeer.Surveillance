using System.Collections.Generic;
using System.Threading.Tasks;
using Surveillance.Auditing.DataLayer.Processes.Interfaces;

namespace Surveillance.Auditing.DataLayer.Repositories.Interfaces
{
    public interface ISystemProcessOperationUploadFileRepository
    {
        Task Create(ISystemProcessOperationUploadFile entity);
        Task<IReadOnlyCollection<ISystemProcessOperationUploadFile>> GetDashboard();
    }
}