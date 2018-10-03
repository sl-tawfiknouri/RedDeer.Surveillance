using Surveillance.System.DataLayer.Processes.Interfaces;
using Surveillance.System.DataLayer.Repositories.Interfaces;

namespace Surveillance.System.DataLayer.Repositories
{
    public class SystemProcessRepository : ISystemProcessRepository
    {
        public void Create(ISystemProcess entity)
        {
            if (entity == null
                || string.IsNullOrWhiteSpace(entity.InstanceId))
            {
                return;
            }

            // TODO handle saving to Aurora
        }

        public void Update(ISystemProcess entity)
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
