using Surveillance.System.Auditing.Processes;

namespace Surveillance.System.DataLayer.Repositories.Interfaces
{
    public interface ISystemProcessOperationFileRepository
    {
        void Create(SystemProcessOperationFileUpload entity);
    }
}