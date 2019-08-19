namespace Surveillance.Api.DataAccess.Abstractions.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Surveillance.Api.DataAccess.Abstractions.Entities;

    public interface IFinancialInstrumentRepository
    {
        Task<IFinancialInstrument> GetById(int id);

        Task<IEnumerable<IFinancialInstrument>> Query(
            Func<IQueryable<IFinancialInstrument>, IQueryable<IFinancialInstrument>> query);
    }
}