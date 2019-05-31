using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Surveillance.Api.DataAccess.Abstractions.Entities;

namespace Surveillance.Api.DataAccess.Abstractions.Repositories
{
    public interface IMarketRepository
    {
        Task<IEnumerable<IMarket>> GetAllDb();
        Task<IEnumerable<IMarket>> Query(Func<IQueryable<IMarket>, IQueryable<IMarket>> query);
        Task<IMarket> GetById(int id);
    }
}