using Surveillance.System.DataLayer.Entities;
using Surveillance.System.DataLayer.Repositories.Interfaces;

namespace Surveillance.System.DataLayer.Repositories
{
    public class SystemProcessRepository : ISystemProcessRepository
    {
        public void Create(SystemProcessEntity entity)
        {
            if (entity == null
                || string.IsNullOrWhiteSpace(entity.InstanceId))
            {
                return;
            }

            // TODO handle saving to Aurora
        }

        public void Update(SystemProcessEntity entity)
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
