using System.Collections.Generic;
using System.Threading.Tasks;
using Surveillance.Api.DataAccess.Abstractions.Entities;

namespace Surveillance.Api.DataAccess.Abstractions.Repositories
{
    public interface ISystemProcessOperationRepository
    {
        Task<IEnumerable<ISystemProcessOperation>> GetAllDb();
        Task<ISystemProcessOperation> GetForId(int id);
    }
}