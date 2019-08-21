namespace Surveillance.Api.DataAccess.Abstractions.Repositories
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Surveillance.Api.DataAccess.Abstractions.Entities;

    public interface ISystemProcessOperationDataSynchroniserRepository
    {
        Task<IEnumerable<ISystemProcessOperationDataSynchroniser>> GetAllDb();
    }
}