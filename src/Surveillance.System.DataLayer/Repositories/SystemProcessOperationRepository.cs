using Surveillance.System.DataLayer.Processes.Interfaces;
using Surveillance.System.DataLayer.Repositories.Interfaces;

namespace Surveillance.System.DataLayer.Repositories
{
    public class SystemProcessOperationRepository : ISystemProcessOperationRepository
    {
        public void Create(ISystemProcessOperation entity)
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
