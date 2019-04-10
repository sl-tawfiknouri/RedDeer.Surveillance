using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Surveillance.Api.DataAccess.Abstractions.Entities;

namespace Surveillance.Api.DataAccess.Abstractions.Repositories
{
    public interface IRuleBreachRepository
    {
        Task<IEnumerable<IRuleBreach>> GetAllDb();
        Task<IEnumerable<IRuleBreach>> Query(Func<IQueryable<IRuleBreach>, IQueryable<IRuleBreach>> query);
    }
}