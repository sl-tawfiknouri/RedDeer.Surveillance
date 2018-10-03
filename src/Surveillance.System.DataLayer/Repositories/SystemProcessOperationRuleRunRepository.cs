using Surveillance.System.Auditing.Processes;
using Surveillance.System.DataLayer.Repositories.Interfaces;

namespace Surveillance.System.DataLayer.Repositories
{
    public class SystemProcessOperationRuleRunRepository : ISystemProcessOperationRuleRunRepository
    {
        public void Create(SystemProcessOperationRuleRun entity)
        {
            if (entity == null
                || string.IsNullOrWhiteSpace(entity.OperationId))
            {
                return;
            }

            // TODO handle saving to Aurora
        }

    }
}
