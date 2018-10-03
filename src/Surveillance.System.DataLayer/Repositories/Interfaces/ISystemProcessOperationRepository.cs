using Surveillance.System.DataLayer.Entities;

namespace Surveillance.System.DataLayer.Repositories.Interfaces
{
    public interface ISystemProcessOperationRepository
    {
        void Create(SystemProcessOperationEntity entity);
    }
}