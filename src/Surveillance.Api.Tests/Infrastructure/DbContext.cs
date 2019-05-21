using Microsoft.EntityFrameworkCore;
using Surveillance.Api.DataAccess.DbContexts;
using Surveillance.Api.DataAccess.Entities;
using System;
using System.Collections.Generic;
using System.Text;

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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<RuleBreachOrder>()
                .HasKey(c => new { c.RuleBreachId, c.OrderId });
        }
    }
}
