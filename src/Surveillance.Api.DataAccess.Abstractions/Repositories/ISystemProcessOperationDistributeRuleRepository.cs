namespace Surveillance.Api.DataAccess.Abstractions.Repositories
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Surveillance.Api.DataAccess.Abstractions.Entities;

    public interface ISystemProcessOperationDistributeRuleRepository
    {
        Task<IEnumerable<ISystemProcessOperationDistributeRule>> GetAllDb();
    }
}