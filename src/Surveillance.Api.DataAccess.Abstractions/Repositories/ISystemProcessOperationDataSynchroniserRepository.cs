using System.Collections.Generic;
using System.Threading.Tasks;
using Surveillance.Api.DataAccess.Abstractions.Entities;

namespace Surveillance.Api.DataAccess.Abstractions.Repositories
{
    public interface ISystemProcessOperationDataSynchroniserRepository
    {
        Task<IEnumerable<ISystemProcessOperationDataSynchroniser>> GetAllDb();
    }
}