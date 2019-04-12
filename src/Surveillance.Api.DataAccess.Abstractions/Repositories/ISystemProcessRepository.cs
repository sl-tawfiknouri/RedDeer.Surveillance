using System.Collections.Generic;
using System.Threading.Tasks;
using Surveillance.Api.DataAccess.Abstractions.Entities;

namespace Surveillance.Api.DataAccess.Abstractions.Repositories
{
    public interface ISystemProcessRepository
    {
        Task<IEnumerable<ISystemProcess>> GetAllDb();
        Task<ISystemProcess> GetForId(string id);
    }
}