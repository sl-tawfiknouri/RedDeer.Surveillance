using Surveillance.System.Auditing.Processes;
using Surveillance.System.DataLayer.Repositories.Interfaces;

namespace Surveillance.System.DataLayer.Repositories
{
    public class SystemProcessOperationFileRepository : ISystemProcessOperationFileRepository
    {
        public void Create(SystemProcessOperationFileUpload entity)
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
