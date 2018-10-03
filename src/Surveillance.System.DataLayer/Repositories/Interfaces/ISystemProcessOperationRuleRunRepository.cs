using Surveillance.System.DataLayer.Processes;

namespace Surveillance.System.DataLayer.Repositories.Interfaces
{
    public interface ISystemProcessOperationRuleRunRepository
    {
        void Create(SystemProcessOperationRuleRun entity);
    }
}