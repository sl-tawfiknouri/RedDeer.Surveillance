namespace Surveillance.Api.DataAccess.Abstractions.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Surveillance.Api.DataAccess.Abstractions.Entities;

    public interface IOrderRepository
    {
        Task<IEnumerable<IAggregation>> AggregationQuery(IOrderQueryOptions options);

        Task<ILookup<int, IOrder>> GetAllForRuleBreach(IEnumerable<int> ruleBreachId);

        Task<ILookup<string, IOrderLedger>> GetForClientAccount(IEnumerable<string> clientAccounts);

        Task<ILookup<string, IOrderLedger>> GetForFund(IEnumerable<string> funds);

        Task<ILookup<string, IOrderLedger>> GetForStrategy(IEnumerable<string> strategies);

        Task<IEnumerable<IOrder>> Query(IOrderQueryOptions options);

        Task<IEnumerable<IOrdersAllocation>> QueryAllocations(
            Func<IQueryable<IOrdersAllocation>, IQueryable<IOrdersAllocation>> query);

        Task<IEnumerable<IClientAccount>> QueryClientAccount(
            Func<IQueryable<IOrdersAllocation>, IQueryable<IOrdersAllocation>> query);

        Task<IEnumerable<IFund>> QueryFund(Func<IQueryable<IOrdersAllocation>, IQueryable<IOrdersAllocation>> query);

        Task<IEnumerable<IStrategy>> QueryStrategy(
            Func<IQueryable<IOrdersAllocation>, IQueryable<IOrdersAllocation>> query);

        Task<IOrderLedger> QueryUnallocated();

        Task<IEnumerable<ITrader>> TradersQuery(Func<IQueryable<IOrder>, IQueryable<IOrder>> query);
    }
}