using Microsoft.EntityFrameworkCore;
using Surveillance.Api.DataAccess.DbContexts;
using Surveillance.Api.DataAccess.Entities;

namespace Surveillance.Api.Tests.Infrastructure
{
    public class DbContext : GraphQlDbContext
    {
        public DbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<RuleBreach> DbRuleBreaches => _ruleBreach;
        public DbSet<RuleBreachOrder> DbRuleBreachOrders => _ruleBreachOrders;
        public DbSet<Order> DbOrders => _orders;
        public DbSet<SystemProcessOperationRuleRun> DbRuleRuns => _ruleRun;
        public DbSet<SystemProcessOperation> DbProcessOperations => _processOperation;
        public DbSet<Market> DbMarkets => _market;
        public DbSet<FinancialInstrument> DbDFinancialInstruments => _financialInstrument;
        public DbSet<OrdersAllocation> DbOrderAllocations => _ordersAllocation;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<RuleBreachOrder>()
                .HasKey(c => new { c.RuleBreachId, c.OrderId });
        }
    }
}
