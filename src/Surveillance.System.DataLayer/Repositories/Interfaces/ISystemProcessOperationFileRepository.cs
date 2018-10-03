using Surveillance.System.DataLayer.Entities;

namespace Surveillance.System.DataLayer.Repositories.Interfaces
{
    public interface ISystemProcessOperationFileRepository
    {
        void Create(SystemProcessOperationFileUploadEntity entity);
    }
}