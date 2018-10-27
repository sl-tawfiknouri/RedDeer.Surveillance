using System.Threading.Tasks;
using Surveillance.System.DataLayer.Processes.Interfaces;

namespace Surveillance.System.DataLayer.Repositories.Interfaces
{
    public interface ISystemProcessOperationUploadFileRepository
    {
        Task Create(ISystemProcessOperationUploadFile entity);
    }
}