using Surveillance.System.DataLayer.Processes.Interfaces;
using Surveillance.System.DataLayer.Repositories.Interfaces;

namespace Surveillance.System.DataLayer.Repositories
{
    public class SystemProcessOperationRuleRunRepository : ISystemProcessOperationRuleRunRepository
    {
        public void Create(ISystemProcessOperationRuleRun entity)
        {
            if (entity == null
                || string.IsNullOrWhiteSpace(entity.OperationId))
            {
                return;
            }

            // TODO handle saving to Aurora
        }

        public void Update(ISystemProcessOperationRuleRun entity)
        {
            if (entity == null
                || string.IsNullOrWhiteSpace(entity.Id))
            {
                return;
            }
        }
    }
}
