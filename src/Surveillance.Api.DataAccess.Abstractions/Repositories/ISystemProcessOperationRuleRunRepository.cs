namespace Surveillance.Api.DataAccess.Abstractions.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Surveillance.Api.DataAccess.Abstractions.Entities;

    public interface ISystemProcessOperationRuleRunRepository
    {
        Task<IEnumerable<ISystemProcessOperationRuleRun>> GetAllDb();

        Task<IEnumerable<ISystemProcessOperationRuleRun>> Query(
            Func<IQueryable<ISystemProcessOperationRuleRun>, IQueryable<ISystemProcessOperationRuleRun>> query);
    }
}