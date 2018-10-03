using Surveillance.System.Auditing.Processes;

namespace Surveillance.System.DataLayer.Repositories.Interfaces
{
    public interface ISystemProcessOperationRuleRunRepository
    {
        void Create(SystemProcessOperationRuleRun entity);
    }
}