using Surveillance.System.DataLayer.Entities;

namespace Surveillance.System.DataLayer.Repositories.Interfaces
{
    public interface ISystemProcessOperationRuleRunRepository
    {
        void Create(SystemProcessOperationRuleRunEntity entity);
    }
}