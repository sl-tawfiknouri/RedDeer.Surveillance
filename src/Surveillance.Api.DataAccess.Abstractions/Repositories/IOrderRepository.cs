using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Surveillance.Api.DataAccess.Abstractions.Entities;

namespace Surveillance.Api.DataAccess.Abstractions.Repositories
{
    public interface IOrderRepository
    {
        Task<IOrderLedger> QueryUnallocated();
        Task<System.Linq.ILookup<int, IOrder>> GetAllForRuleBreach(IEnumerable<int> ruleBreachId);
        Task<ILookup<string, IOrderLedger>> GetForStrategy(IEnumerable<string> strategies);
        Task<System.Linq.ILookup<string, IOrderLedger>> GetForFund(IEnumerable<string> funds);
        Task<ILookup<string, IOrderLedger>> GetForClientAccount(IEnumerable<string> clientAccounts);
        Task<IEnumerable<ITrader>> TradersQuery(Func<IQueryable<IOrder>, IQueryable<IOrder>> query);
        Task<IEnumerable<IClientAccount>> QueryClientAccount(Func<IQueryable<IOrdersAllocation>, IQueryable<IOrdersAllocation>> query);
        Task<IEnumerable<IFund>> QueryFund(Func<IQueryable<IOrdersAllocation>, IQueryable<IOrdersAllocation>> query);
        Task<IEnumerable<IStrategy>> QueryStrategy(Func<IQueryable<IOrdersAllocation>, IQueryable<IOrdersAllocation>> query);
        Task<IEnumerable<IOrder>> Query(Func<IQueryable<IOrder>, IQueryable<IOrder>> query);
        Task<IEnumerable<IAggregation>> AggregationQuery(Func<IQueryable<IOrder>, IQueryable<IOrder>> query);
    }
}