using Surveillance.System.DataLayer.Processes.Interfaces;

namespace Surveillance.System.DataLayer.Repositories.Interfaces
{
    public interface ISystemProcessOperationRuleRunRepository
    {
        void Create(ISystemProcessOperationRuleRun entity);
    }
}