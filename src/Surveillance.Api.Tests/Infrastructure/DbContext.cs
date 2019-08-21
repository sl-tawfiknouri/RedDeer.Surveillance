namespace Surveillance.Api.Tests.Infrastructure
{
    using Microsoft.EntityFrameworkCore;

    using Surveillance.Api.DataAccess.DbContexts;
    using Surveillance.Api.DataAccess.Entities;

    public class DbContext : GraphQlDbContext
    {
        public DbContext(DbContextOptions options)
            : base(options)
        {
        }

        public DbSet<Broker> DbBrokers => this._broker;

        public DbSet<FinancialInstrument> DbDFinancialInstruments => this._financialInstrument;

        public DbSet<Market> DbMarkets => this._market;

        public DbSet<OrdersAllocation> DbOrderAllocations => this._ordersAllocation;

        public DbSet<Order> DbOrders => this._orders;

        public DbSet<SystemProcessOperation> DbProcessOperations => this._processOperation;

        public DbSet<RuleBreach> DbRuleBreaches => this._ruleBreach;

        public DbSet<RuleBreachOrder> DbRuleBreachOrders => this._ruleBreachOrders;

        public DbSet<SystemProcessOperationRuleRun> DbRuleRuns => this._ruleRun;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<RuleBreachOrder>().HasKey(c => new { c.RuleBreachId, c.OrderId });
        }
    }
}