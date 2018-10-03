using Surveillance.System.DataLayer.Entities;

namespace Surveillance.System.DataLayer.Repositories.Interfaces
{
    public interface ISystemProcessRepository
    {
        void Create(SystemProcessEntity entity);
        void Update(SystemProcessEntity entity);
    }
}