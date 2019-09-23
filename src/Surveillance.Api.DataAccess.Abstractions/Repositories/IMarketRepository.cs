namespace Surveillance.Api.DataAccess.Abstractions.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Surveillance.Api.DataAccess.Abstractions.Entities;

    public interface IMarketRepository
    {
        Task<IEnumerable<IMarket>> GetAllDb();

        Task<IMarket> GetById(int id);

        Task<IEnumerable<IMarket>> Query(Func<IQueryable<IMarket>, IQueryable<IMarket>> query);
    }
}