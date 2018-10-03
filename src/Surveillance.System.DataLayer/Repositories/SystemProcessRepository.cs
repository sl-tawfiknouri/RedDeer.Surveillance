using Surveillance.System.Auditing.Processes;
using Surveillance.System.DataLayer.Repositories.Interfaces;

namespace Surveillance.System.DataLayer.Repositories
{
    public class SystemProcessRepository : ISystemProcessRepository
    {
        public void Create(SystemProcess entity)
        {
            if (entity == null
                || string.IsNullOrWhiteSpace(entity.InstanceId))
            {
                return;
            }

            // TODO handle saving to Aurora
        }

        public void Update(SystemProcess entity)
        {
            if (entity == null
                || string.IsNullOrWhiteSpace(entity.InstanceId))
            {
                return;
            }

            // TODO handle saving to Aurora
        }
    }
}
