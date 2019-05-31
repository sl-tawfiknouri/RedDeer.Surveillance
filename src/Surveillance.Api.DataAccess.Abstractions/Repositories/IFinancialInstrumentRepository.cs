using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Surveillance.Api.DataAccess.Abstractions.Entities;

namespace Surveillance.Api.DataAccess.Abstractions.Repositories
{
    public interface IFinancialInstrumentRepository
    {
        Task<IEnumerable<IFinancialInstrument>> Query(Func<IQueryable<IFinancialInstrument>, IQueryable<IFinancialInstrument>> query);
        Task<IFinancialInstrument> GetById(int id);
    }
}