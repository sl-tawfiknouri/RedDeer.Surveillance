namespace Surveillance.Api.DataAccess.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Domain.Core.Trading.Orders;

    using Microsoft.EntityFrameworkCore;

    using Surveillance.Api.DataAccess.Abstractions.DbContexts.Factory;
    using Surveillance.Api.DataAccess.Abstractions.Entities;
    using Surveillance.Api.DataAccess.Abstractions.Repositories;
    using Surveillance.Api.DataAccess.Abstractions.Services;
    using Surveillance.Api.DataAccess.Entities;

    using Order = Surveillance.Api.DataAccess.Entities.Order;

    public class OrderRepository : IOrderRepository
    {
        private readonly IGraphQlDbContextFactory _factory;

        private readonly ITimeZoneService _timeZoneService;

        public OrderRepository(IGraphQlDbContextFactory factory, ITimeZoneService timeZoneService)
        {
            this._factory = factory ?? throw new ArgumentNullException(nameof(factory));
            this._timeZoneService = timeZoneService ?? throw new ArgumentNullException(nameof(timeZoneService));
        }

        public async Task<IEnumerable<IAggregation>> AggregationQuery(IOrderQueryOptions options)
        {
            using (var dbContext = this._factory.Build())
            {
                var aggregations = new Dictionary<string, int>();

                var from = options.PlacedDateFrom ?? throw new ArgumentNullException($"{options.PlacedDateFrom}");
                var to = options.PlacedDateTo ?? throw new ArgumentNullException($"{options.PlacedDateTo}");

                var segments = this._timeZoneService.GetOffsetSegments(from, to, options.TzName);

                foreach (var segment in segments)
                {
                    var query = this.SetOrderQueryOptions(dbContext.Orders, options);

                    if (segment.FromIncluding) query = query.Where(x => x.PlacedDate >= segment.FromUtc);
                    else query = query.Where(x => x.PlacedDate > segment.FromUtc);

                    if (segment.ToIncluding) query = query.Where(x => x.PlacedDate <= segment.ToUtc);
                    else query = query.Where(x => x.PlacedDate < segment.ToUtc);

                    var segmentAggregations = await query
                                                  .GroupBy(x => x.PlacedDate.Value.AddHours(segment.HourOffset).Date)
                                                  .Select(
                                                      x => new Aggregation
                                                               {
                                                                   Key = x.Key.ToString("yyyy-MM-dd"), Count = x.Count()
                                                               }).AsNoTracking().ToListAsync();

                    foreach (var segmentAggregation in segmentAggregations)
                    {
                        var key = segmentAggregation.Key;
                        if (!aggregations.ContainsKey(key)) aggregations[key] = 0;
                        aggregations[key] += segmentAggregation.Count;
                    }
                }

                return aggregations.Select(x => new Aggregation { Key = x.Key, Count = x.Value }).OrderBy(x => x.Key)
                    .ToList();
            }
        }

        public async Task<ILookup<int, IOrder>> GetAllForRuleBreach(IEnumerable<int> ruleBreachId)
        {
            using (var dbContext = this._factory.Build())
            {
                var orderRuleBreaches = await dbContext.RuleBreachOrders
                                            .Where(a => ruleBreachId.Contains(a.RuleBreachId)).AsNoTracking()
                                            .ToListAsync();

                var orderIds = orderRuleBreaches.Select(i => i.OrderId).ToList();

                var orders = await dbContext.Orders.Where(i => orderIds.Contains(i.Id)).ToListAsync();

                var groupedRuleBreaches = orderRuleBreaches
                    .Select(
                        i => new KeyValuePair<int, IOrder>(
                            i.RuleBreachId,
                            orders.FirstOrDefault(y => y.Id == i.OrderId))).ToLookup(i => i.Key, i => i.Value);

                return groupedRuleBreaches;
            }
        }

        public async Task<ILookup<string, IOrderLedger>> GetForClientAccount(IEnumerable<string> clientAccounts)
        {
            var filteredClientAccounts = clientAccounts?.Where(i => !string.IsNullOrWhiteSpace(i)).ToList();

            if (filteredClientAccounts == null || !filteredClientAccounts.Any())
                return new Dictionary<string, IOrderLedger>().ToLookup(i => i.Key, i => i.Value);

            using (var dbContext = this._factory.Build())
            {
                var orderAllocations = dbContext.OrdersAllocation.Where(i => i.Live)
                    .Where(i => filteredClientAccounts.Contains(i.ClientAccountId)).AsNoTracking().ToList();

                var orderAllocationOrderIds = orderAllocations.Select(i => i.OrderId).ToList();

                var orders = await dbContext.Orders.Where(i => i.Live)
                                 .Where(i => orderAllocationOrderIds.Contains(i.ClientOrderId)).AsNoTracking()
                                 .ToListAsync();

                var portfolios = new List<KeyValuePair<string, IOrderLedger>>();
                foreach (var clientAccount in filteredClientAccounts)
                {
                    var clientAccountsAllocs = orderAllocations.Where(
                        i => string.Equals(
                            i.ClientAccountId,
                            clientAccount,
                            StringComparison.InvariantCultureIgnoreCase)).ToList();

                    var clientAccountAllocOrderIds = clientAccountsAllocs.Select(i => i.OrderId).ToList();
                    var clientAccountOrders =
                        orders.Where(i => clientAccountAllocOrderIds.Contains(i.ClientOrderId)).ToList();

                    var projectedOrders = this.MappedOrders(clientAccountOrders, clientAccountsAllocs);

                    var portfolio = new OrderLedger { Name = clientAccount, Orders = projectedOrders };

                    portfolios.Add(new KeyValuePair<string, IOrderLedger>(clientAccount, portfolio));
                }

                return portfolios.ToLookup(i => i.Key, i => i.Value);
            }
        }

        public async Task<ILookup<string, IOrderLedger>> GetForFund(IEnumerable<string> funds)
        {
            var filteredFunds = funds?.Where(i => !string.IsNullOrWhiteSpace(i)).ToList();

            if (filteredFunds == null || !filteredFunds.Any())
                return new Dictionary<string, IOrderLedger>().ToLookup(i => i.Key, i => i.Value);

            using (var dbContext = this._factory.Build())
            {
                var orderAllocations = dbContext.OrdersAllocation.Where(i => i.Live)
                    .Where(i => filteredFunds.Contains(i.Fund)).AsNoTracking().ToList();

                var orderAllocationOrderIds = orderAllocations.Select(i => i.OrderId).ToList();

                var orders = await dbContext.Orders.Where(i => i.Live)
                                 .Where(i => orderAllocationOrderIds.Contains(i.ClientOrderId)).AsNoTracking()
                                 .ToListAsync();

                var portfolios = new List<KeyValuePair<string, IOrderLedger>>();
                foreach (var fund in filteredFunds)
                {
                    var fundAllocs = orderAllocations.Where(
                        i => string.Equals(i.Fund, fund, StringComparison.InvariantCultureIgnoreCase)).ToList();
                    var fundAllocOrderIds = fundAllocs.Select(i => i.OrderId).ToList();
                    var fundOrders = orders.Where(i => fundAllocOrderIds.Contains(i.ClientOrderId)).ToList();

                    var projectedOrders = this.MappedOrders(fundOrders, fundAllocs);

                    var portfolio = new OrderLedger { Name = fund, Orders = projectedOrders };

                    portfolios.Add(new KeyValuePair<string, IOrderLedger>(fund, portfolio));
                }

                return portfolios.ToLookup(i => i.Key, i => i.Value);
            }
        }

        public async Task<ILookup<string, IOrderLedger>> GetForStrategy(IEnumerable<string> strategies)
        {
            var filteredStrategies = strategies?.Where(i => !string.IsNullOrWhiteSpace(i)).ToList();

            if (filteredStrategies == null || !filteredStrategies.Any())
                return new Dictionary<string, IOrderLedger>().ToLookup(i => i.Key, i => i.Value);

            using (var dbContext = this._factory.Build())
            {
                var orderAllocations = dbContext.OrdersAllocation.Where(i => i.Live)
                    .Where(i => filteredStrategies.Contains(i.Strategy)).AsNoTracking().ToList();

                var orderAllocationOrderIds = orderAllocations.Select(i => i.OrderId).ToList();

                var orders = await dbContext.Orders.Where(i => i.Live)
                                 .Where(i => orderAllocationOrderIds.Contains(i.ClientOrderId)).AsNoTracking()
                                 .ToListAsync();

                var portfolios = new List<KeyValuePair<string, IOrderLedger>>();
                foreach (var strategy in filteredStrategies)
                {
                    var strategyAllocs = orderAllocations.Where(
                        i => string.Equals(i.Strategy, strategy, StringComparison.InvariantCultureIgnoreCase)).ToList();
                    var strategyAllocOrderIds = strategyAllocs.Select(i => i.OrderId).ToList();
                    var strategyOrders = orders.Where(i => strategyAllocOrderIds.Contains(i.ClientOrderId)).ToList();

                    var projectedOrders = this.MappedOrders(strategyOrders, strategyAllocs);

                    var portfolio = new OrderLedger { Name = strategy, Orders = projectedOrders };

                    portfolios.Add(new KeyValuePair<string, IOrderLedger>(strategy, portfolio));
                }

                return portfolios.ToLookup(i => i.Key, i => i.Value);
            }
        }

        public async Task<IEnumerable<IOrder>> Query(IOrderQueryOptions options)
        {
            using (var dbContext = this._factory.Build())
            {
                var query = this.SetOrderQueryOptions(dbContext.Orders, options);

                if (options.PlacedDateFrom.HasValue)
                    query = query.Where(x => x.PlacedDate >= options.PlacedDateFrom.Value);

                if (options.PlacedDateTo.HasValue) query = query.Where(x => x.PlacedDate <= options.PlacedDateTo.Value);

                query = query.OrderByDescending(x => x.PlacedDate); // apply Order before Take

                if (options.Take != null) query = query.Take(options.Take.Value);

                var orders = await query.AsNoTracking().ToListAsync();

                return orders;
            }
        }

        public async Task<IEnumerable<IOrdersAllocation>> QueryAllocations(
            Func<IQueryable<IOrdersAllocation>, IQueryable<IOrdersAllocation>> query)
        {
            using (var dbContext = this._factory.Build())
            {
                var orderAllocations = await query(dbContext.OrdersAllocation)
                                           .Where(a => a.Live).AsNoTracking().ToListAsync();

                var projectedOrderAllocations = orderAllocations.Select(
                    s => new OrdersAllocation
                             {
                                 Id = s.Id,
                                 OrderId = s.OrderId,
                                 Fund = s.Fund,
                                 Strategy = s.Strategy,
                                 ClientAccountId = s.ClientAccountId,
                                 OrderFilledVolume = s.OrderFilledVolume,
                                 Live = s.Live,
                                 AutoScheduled = s.AutoScheduled,
                                 CreatedDate = s.CreatedDate
                             }).ToList();

                return projectedOrderAllocations;
            }
        }

        public async Task<IEnumerable<IClientAccount>> QueryClientAccount(
            Func<IQueryable<IOrdersAllocation>, IQueryable<IOrdersAllocation>> query)
        {
            using (var dbContext = this._factory.Build())
            {
                var clientAccounts = await query(dbContext.OrdersAllocation).Where(a => a.Live)
                                         .Select(a => a.ClientAccountId).Distinct().AsNoTracking().ToListAsync();

                var projectedAccounts = clientAccounts.Select(this.Map).Where(i => i != null).ToList();

                return projectedAccounts;
            }
        }

        public async Task<IEnumerable<IFund>> QueryFund(
            Func<IQueryable<IOrdersAllocation>, IQueryable<IOrdersAllocation>> query)
        {
            using (var dbContext = this._factory.Build())
            {
                var funds = await query(dbContext.OrdersAllocation).Where(a => a.Live).Select(a => a.Fund).Distinct()
                                .AsNoTracking().ToListAsync();

                var projectedFunds = funds.Select(this.MapFund).Where(i => i != null).ToList();

                return projectedFunds;
            }
        }

        public async Task<IEnumerable<IStrategy>> QueryStrategy(
            Func<IQueryable<IOrdersAllocation>, IQueryable<IOrdersAllocation>> query)
        {
            using (var dbContext = this._factory.Build())
            {
                var strategies = await query(dbContext.OrdersAllocation).Where(a => a.Live).Select(a => a.Strategy)
                                     .Distinct().AsNoTracking().ToListAsync();

                var projectedStrategies = strategies.Select(this.MapStrategy).Where(i => i != null).ToList();

                return projectedStrategies;
            }
        }

        public async Task<IOrderLedger> QueryUnallocated()
        {
            using (var dbContext = this._factory.Build())
            {
                var orders = await dbContext.Orders.AsNoTracking().ToArrayAsync();

                var portfolio = new OrderLedger
                                    {
                                        Manager = string.Empty, Name = "Combined Portfolio", Orders = orders
                                    };

                return portfolio;
            }
        }

        public async Task<IEnumerable<ITrader>> TradersQuery(Func<IQueryable<IOrder>, IQueryable<IOrder>> query)
        {
            using (var dbContext = this._factory.Build())
            {
                var orders = await query(dbContext.Orders).Where(i => i.Live)
                                 .Select(ta => new { ta.TraderId, ta.TraderName }).Distinct().AsNoTracking()
                                 .ToListAsync();

                return orders.Select(i => new Trader { Id = i.TraderId, Name = i.TraderName }).ToList();
            }
        }

        private IClientAccount Map(string clientAccount)
        {
            if (string.IsNullOrWhiteSpace(clientAccount)) return null;

            return new ClientAccount(clientAccount);
        }

        private Fund MapFund(string fund)
        {
            if (string.IsNullOrWhiteSpace(fund)) return null;

            return new Fund(fund);
        }

        private IOrder[] MappedOrders(List<IOrder> orders, List<IOrdersAllocation> orderAllocations)
        {
            var mappedOrders = new List<IOrder>();

            foreach (var orderAlloc in orderAllocations)
            {
                var orderForAlloc = orders.FirstOrDefault(
                    i => string.Equals(i.ClientOrderId, orderAlloc.OrderId, StringComparison.CurrentCultureIgnoreCase));

                if (orderForAlloc == null) continue;

                var baseOrder = (Order)orderForAlloc.Clone();
                baseOrder.FilledVolume = orderAlloc.OrderFilledVolume;
                baseOrder.Fund = orderAlloc.Fund;
                baseOrder.Strategy = orderAlloc.Strategy;
                baseOrder.ClientAccount = orderAlloc.ClientAccountId;

                mappedOrders.Add(baseOrder);
            }

            return mappedOrders.ToArray();
        }

        private IStrategy MapStrategy(string strategy)
        {
            if (string.IsNullOrWhiteSpace(strategy)) return null;

            return new Strategy(strategy);
        }

        private IQueryable<IOrder> SetOrderQueryOptions(IQueryable<IOrder> query, IOrderQueryOptions options)
        {
            if (options.Ids != null) query = query.Where(x => options.Ids.Contains(x.Id));
            if (options.TraderIds != null) query = query.Where(x => options.TraderIds.Contains(x.TraderId));
            if (options.ExcludeTraderIds != null)
                query = query.Where(x => !options.ExcludeTraderIds.Contains(x.TraderId));
            if (options.ReddeerIds != null)
                query = query.Where(x => options.ReddeerIds.Contains(x.FinancialInstrument.ReddeerId));
            if (options.Directions != null) query = query.Where(x => options.Directions.Contains(x.Direction));
            if (options.Types != null) query = query.Where(x => options.Types.Contains(x.OrderType));

            if (options.Statuses != null)
            {
                var statuses = options.Statuses.Select(x => (OrderStatus)x).ToList();
                query = query.Where(
                    x => statuses.Contains(OrderStatus.Cancelled) && x.CancelledDate != null
                         || statuses.Contains(OrderStatus.Rejected) && x.CancelledDate == null && x.RejectedDate != null
                         || statuses.Contains(OrderStatus.Filled) && x.CancelledDate == null && x.RejectedDate == null
                         && x.FilledDate != null
                         || statuses.Contains(OrderStatus.Amended) && x.CancelledDate == null && x.RejectedDate == null
                         && x.FilledDate == null && x.AmendedDate != null
                         || statuses.Contains(OrderStatus.Booked) && x.CancelledDate == null && x.RejectedDate == null
                         && x.FilledDate == null && x.AmendedDate == null && x.BookedDate != null
                         || statuses.Contains(OrderStatus.Placed) && x.CancelledDate == null && x.RejectedDate == null
                         && x.FilledDate == null && x.AmendedDate == null && x.BookedDate == null
                         && x.PlacedDate != null || statuses.Contains(OrderStatus.Unknown) && x.CancelledDate == null
                                                                                           && x.RejectedDate == null
                                                                                           && x.FilledDate == null
                                                                                           && x.AmendedDate == null
                                                                                           && x.BookedDate == null
                                                                                           && x.PlacedDate == null);
            }

            return query;
        }
    }
}