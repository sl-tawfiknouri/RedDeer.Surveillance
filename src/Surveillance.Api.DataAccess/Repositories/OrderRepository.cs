﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Surveillance.Api.DataAccess.Abstractions.DbContexts.Factory;
using Surveillance.Api.DataAccess.Abstractions.Entities;
using Surveillance.Api.DataAccess.Abstractions.Repositories;
using Surveillance.Api.DataAccess.Entities;

namespace Surveillance.Api.DataAccess.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly IGraphQlDbContextFactory _factory;

        public OrderRepository(IGraphQlDbContextFactory factory)
        {
            _factory = factory ?? throw new ArgumentNullException(nameof(factory));
        }

        public async Task<ILookup<int, IOrder>> GetAllForRuleBreach(IEnumerable<int> ruleBreachId)
        {
            using (var dbContext = _factory.Build())
            {
                var orderRuleBreaches =
                    await dbContext
                        .RuleBreachOrders
                        .Where(a => ruleBreachId.Contains(a.RuleBreachId))
                        .AsNoTracking()
                        .ToListAsync();

                var orderIds = orderRuleBreaches.Select(i => i.OrderId).ToList();

                var orders =
                    await dbContext
                        .Orders
                        .Where(i => orderIds.Contains(i.Id))
                        .ToListAsync();


                var groupedRuleBreaches = orderRuleBreaches
                    .Select(i =>
                        new KeyValuePair<int, IOrder>(
                            i.RuleBreachId,
                            orders.FirstOrDefault(y => y.Id == i.OrderId)))
                    .ToLookup(i => i.Key, i => i.Value);

                return groupedRuleBreaches;
            }
        }

        public async Task<IOrderLedger> QueryUnallocated()
        {
            using (var dbContext = _factory.Build())
            {
                var orders =
                    await dbContext
                        .Orders
                        .AsNoTracking()
                        .ToArrayAsync();

                var portfolio = new OrderLedger
                {
                    Manager = string.Empty,
                    Name = "Combined Portfolio",
                    Orders = orders
                };

                return portfolio;
            }
        }

        public async Task<ILookup<string, IOrderLedger>> GetForClientAccount(IEnumerable<string> clientAccounts)
        {
            var filteredClientAccounts = clientAccounts?.Where(i => !string.IsNullOrWhiteSpace(i)).ToList();

            if (filteredClientAccounts == null
                || !filteredClientAccounts.Any())
            {
                return new Dictionary<string, IOrderLedger>().ToLookup(i => i.Key, i => i.Value);
            }

            using (var dbContext = _factory.Build())
            {
                var orderAllocations =
                    dbContext
                        .OrdersAllocation
                        .Where(i => i.Live)
                        .Where(i => filteredClientAccounts.Contains(i.ClientAccountId))
                        .AsNoTracking()
                        .ToList();

                var orderAllocationOrderIds = orderAllocations.Select(i => i.OrderId).ToList();

                var orders =
                    await dbContext
                        .Orders
                        .Where(i => i.Live)
                        .Where(i => orderAllocationOrderIds.Contains(i.ClientOrderId))
                        .AsNoTracking()
                        .ToListAsync();

                var portfolios = new List<KeyValuePair<string, IOrderLedger>>();
                foreach (var clientAccount in filteredClientAccounts)
                {
                    var clientAccountsAllocs =
                        orderAllocations
                            .Where(i => string.Equals(i.ClientAccountId, clientAccount, StringComparison.InvariantCultureIgnoreCase))
                            .ToList();

                    var clientAccountAllocOrderIds = clientAccountsAllocs.Select(i => i.OrderId).ToList();
                    var clientAccountOrders = orders.Where(i => clientAccountAllocOrderIds.Contains(i.ClientOrderId)).ToList();

                    var projectedOrders = MappedOrders(clientAccountOrders, clientAccountsAllocs);

                    var portfolio = new OrderLedger
                    {
                        Name = clientAccount,
                        Orders = projectedOrders
                    };

                    portfolios.Add(new KeyValuePair<string, IOrderLedger>(clientAccount, portfolio));
                }

                return portfolios.ToLookup(i => i.Key, i => i.Value);
            }
        }

        public async Task<ILookup<string, IOrderLedger>> GetForStrategy(IEnumerable<string> strategies)
        {
            var filteredStrategies = strategies?.Where(i => !string.IsNullOrWhiteSpace(i)).ToList();

            if (filteredStrategies == null
                || !filteredStrategies.Any())
            {
                return new Dictionary<string, IOrderLedger>().ToLookup(i => i.Key, i => i.Value);
            }

            using (var dbContext = _factory.Build())
            {
                var orderAllocations =
                    dbContext
                        .OrdersAllocation
                        .Where(i => i.Live)
                        .Where(i => filteredStrategies.Contains(i.Strategy))
                        .AsNoTracking()
                        .ToList();

                var orderAllocationOrderIds = orderAllocations.Select(i => i.OrderId).ToList();

                var orders =
                    await dbContext
                        .Orders
                        .Where(i => i.Live)
                        .Where(i => orderAllocationOrderIds.Contains(i.ClientOrderId))
                        .AsNoTracking()
                        .ToListAsync();

                var portfolios = new List<KeyValuePair<string, IOrderLedger>>();
                foreach (var strategy in filteredStrategies)
                {
                    var strategyAllocs = orderAllocations.Where(i => string.Equals(i.Strategy, strategy, StringComparison.InvariantCultureIgnoreCase)).ToList();
                    var strategyAllocOrderIds = strategyAllocs.Select(i => i.OrderId).ToList();
                    var strategyOrders = orders.Where(i => strategyAllocOrderIds.Contains(i.ClientOrderId)).ToList();

                    var projectedOrders = MappedOrders(strategyOrders, strategyAllocs);

                    var portfolio = new OrderLedger
                    {
                        Name = strategy,
                        Orders = projectedOrders
                    };

                    portfolios.Add(new KeyValuePair<string, IOrderLedger>(strategy, portfolio));
                }

                return portfolios.ToLookup(i => i.Key, i => i.Value);
            }
        }

        public async Task<ILookup<string, IOrderLedger>> GetForFund(IEnumerable<string> funds)
        {
            var filteredFunds = funds?.Where(i => !string.IsNullOrWhiteSpace(i)).ToList();

            if (filteredFunds == null
                || !filteredFunds.Any())
            {
                return new Dictionary<string, IOrderLedger>().ToLookup(i => i.Key, i => i.Value);
            }

            using (var dbContext = _factory.Build())
            {
                var orderAllocations =
                    dbContext
                        .OrdersAllocation
                        .Where(i => i.Live)
                        .Where(i => filteredFunds.Contains(i.Fund))
                        .AsNoTracking()
                        .ToList();

                var orderAllocationOrderIds = orderAllocations.Select(i => i.OrderId).ToList();

                var orders =
                    await dbContext
                        .Orders
                        .Where(i => i.Live)
                        .Where(i => orderAllocationOrderIds.Contains(i.ClientOrderId))
                        .AsNoTracking()
                        .ToListAsync();

                var portfolios = new List<KeyValuePair<string, IOrderLedger>>();
                foreach (var fund in filteredFunds)
                {
                    var fundAllocs = orderAllocations.Where(i => string.Equals(i.Fund, fund, StringComparison.InvariantCultureIgnoreCase)).ToList();
                    var fundAllocOrderIds = fundAllocs.Select(i => i.OrderId).ToList();
                    var fundOrders = orders.Where(i => fundAllocOrderIds.Contains(i.ClientOrderId)).ToList();

                    var projectedOrders = MappedOrders(fundOrders, fundAllocs);

                    var portfolio = new OrderLedger
                    {
                        Name = fund,
                        Orders = projectedOrders
                    };

                    portfolios.Add(new KeyValuePair<string, IOrderLedger>(fund, portfolio));
                }

                return portfolios.ToLookup(i => i.Key, i => i.Value);
            }
        }

        public async Task<IEnumerable<ITrader>> TradersQuery(Func<IQueryable<IOrder>, IQueryable<IOrder>> query)
        {
            using (var dbContext = _factory.Build())
            {
                var orders =
                    await query(dbContext
                    .Orders)
                    .Where(i => i.Live)
                    .Select(ta => new { ta.TraderId, ta.TraderName })
                    .Distinct()
                    .AsNoTracking()
                    .ToListAsync();

                return orders.Select(i => new Trader {Id = i.TraderId, Name = i.TraderName}).ToList();
            }
        }

        public async Task<IEnumerable<IClientAccount>> QueryClientAccount(Func<IQueryable<IOrdersAllocation>, IQueryable<IOrdersAllocation>> query)
        {
            using (var dbContext = _factory.Build())
            {
                var clientAccounts =
                    await query(dbContext.OrdersAllocation)
                        .Where(a => a.Live)
                        .Select(a => a.ClientAccountId)
                        .Distinct()
                        .AsNoTracking()
                        .ToListAsync();

                var projectedAccounts =
                    clientAccounts
                        .Select(Map)
                        .Where(i => i != null)
                        .ToList();

                return projectedAccounts;
            }
        }

        private IClientAccount Map(string clientAccount)
        {
            if (string.IsNullOrWhiteSpace(clientAccount))
            {
                return null;
            }

            return new ClientAccount(clientAccount);
        }

        public async Task<IEnumerable<IFund>> QueryFund(Func<IQueryable<IOrdersAllocation>, IQueryable<IOrdersAllocation>> query)
        {
            using (var dbContext = _factory.Build())
            {
                var funds =
                    await query(dbContext
                            .OrdersAllocation)
                        .Where(a => a.Live)
                        .Select(a => a.Fund)
                        .Distinct()
                        .AsNoTracking()
                        .ToListAsync();

                var projectedFunds =
                    funds
                        .Select(MapFund)
                        .Where(i => i != null)
                        .ToList();

                return projectedFunds;
            }
        }

        private Fund MapFund(string fund)
        {
            if (string.IsNullOrWhiteSpace(fund))
            {
                return null;
            }

            return new Fund(fund);
        }

        public async Task<IEnumerable<IStrategy>> QueryStrategy(Func<IQueryable<IOrdersAllocation>, IQueryable<IOrdersAllocation>> query)
        {
            using (var dbContext = _factory.Build())
            {
                var strategies =
                    await query(dbContext.OrdersAllocation)
                        .Where(a => a.Live)
                        .Select(a => a.Strategy)
                        .Distinct()
                        .AsNoTracking()
                        .ToListAsync();

                var projectedStrategies =
                    strategies
                        .Select(MapStrategy)
                        .Where(i => i != null)
                        .ToList();

                return projectedStrategies;
            }
        }

        private IStrategy MapStrategy(string strategy)
        {
            if (string.IsNullOrWhiteSpace(strategy))
            {
                return null;
            }

            return new Strategy(strategy);
        }

        private IOrder[] MappedOrders(
            List<Abstractions.Entities.IOrder> orders,
            List<IOrdersAllocation> orderAllocations)
        {
            var mappedOrders = new List<IOrder>();

            foreach (var orderAlloc in orderAllocations)
            {
                var orderForAlloc = orders.FirstOrDefault(i => string.Equals(i.ClientOrderId, orderAlloc.OrderId, StringComparison.CurrentCultureIgnoreCase));

                if (orderForAlloc == null)
                {
                    continue;
                }

                var baseOrder = (Order)orderForAlloc.Clone();
                baseOrder.FilledVolume = orderAlloc.OrderFilledVolume;
                baseOrder.Fund = orderAlloc.Fund;
                baseOrder.Strategy = orderAlloc.Strategy;
                baseOrder.ClientAccount = orderAlloc.ClientAccountId;

                mappedOrders.Add(baseOrder);
            }

            return mappedOrders.ToArray();
        }
    }
}