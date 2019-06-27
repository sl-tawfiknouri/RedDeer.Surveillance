using System.Linq;
using Microsoft.EntityFrameworkCore;
using Surveillance.Api.DataAccess.Abstractions.DbContexts;
using Surveillance.Api.DataAccess.Abstractions.Entities;
using Surveillance.Api.DataAccess.Entities;
// ReSharper disable InconsistentNaming

namespace Surveillance.Api.DataAccess.DbContexts
{
    public class GraphQlDbContext : DbContext, IGraphQlDbContext
    {
        public GraphQlDbContext(DbContextOptions options) : base(options)
        {
        }

        protected virtual DbSet<Market> _market { get; set; }
        public IQueryable<IMarket> Market => _market;

        protected virtual DbSet<Broker> _broker { get; set; }
        public IQueryable<IBroker> Broker => _broker;

        protected virtual DbSet<Order> _orders { get; set; }
        public IQueryable<IOrder> Orders => _orders;

        protected virtual DbSet<OrdersAllocation> _ordersAllocation { get; set; }
        public IQueryable<IOrdersAllocation> OrdersAllocation => _ordersAllocation;

        protected virtual DbSet<RuleBreachOrder> _ruleBreachOrders { get; set; }
        public IQueryable<IRuleBreachOrder> RuleBreachOrders => _ruleBreachOrders;

        protected virtual DbSet<RuleBreach> _ruleBreach { get; set; }
        public IQueryable<IRuleBreach> RuleBreach => _ruleBreach;

        protected virtual DbSet<SystemProcessOperationRuleRun> _ruleRun { get; set; }
        public IQueryable<ISystemProcessOperationRuleRun> RuleRun => _ruleRun;

        protected virtual DbSet<SystemProcessOperationUploadFile> _uploadFile { get; set; }
        public IQueryable<ISystemProcessOperationUploadFile> UploadFile => _uploadFile;

        protected virtual DbSet<SystemProcessOperationDataSynchroniser> _dataSynchroniser { get; set; }
        public IQueryable<ISystemProcessOperationDataSynchroniser> DataSynchroniser => _dataSynchroniser;

        protected virtual DbSet<SystemProcessOperationDistributeRule> _distributeRule { get; set; }
        public IQueryable<ISystemProcessOperationDistributeRule> DistributeRule => _distributeRule;

        protected virtual DbSet<SystemProcessOperation> _processOperation { get; set; }
        public IQueryable<ISystemProcessOperation> ProcessOperation => _processOperation;

        protected virtual DbSet<SystemProcess> _systemProcess { get; set; }
        public IQueryable<ISystemProcess> SystemProcess => _systemProcess;

        protected virtual DbSet<FinancialInstrument> _financialInstrument { get; set; }
        public IQueryable<IFinancialInstrument> FinancialInstrument => _financialInstrument;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Market>().ToTable("Market");
            modelBuilder.Entity<Broker>().ToTable("Brokers");
            modelBuilder.Entity<Order>().ToTable("Orders");
            modelBuilder.Entity<OrdersAllocation>().ToTable("OrdersAllocation");
            modelBuilder.Entity<RuleBreach>().ToTable("RuleBreach");
            modelBuilder.Entity<RuleBreachOrder>().ToTable("RuleBreachOrders").HasKey(c => new { c.RuleBreachId, c.OrderId });
            modelBuilder.Entity<SystemProcess>().ToTable("SystemProcess");
            modelBuilder.Entity<SystemProcessOperation>().ToTable("SystemProcessOperation");
            modelBuilder.Entity<SystemProcessOperationRuleRun>().ToTable("SystemProcessOperationRuleRun");
            modelBuilder.Entity<SystemProcessOperationUploadFile>().ToTable("SystemProcessOperationUploadFile");
            modelBuilder.Entity<SystemProcessOperationDataSynchroniser>().ToTable("SystemProcessOperationDataSynchroniserRequest");
            modelBuilder.Entity<SystemProcessOperationDistributeRule>().ToTable("SystemProcessOperationDistributeRule");
            modelBuilder.Entity<FinancialInstrument>().ToTable("FinancialInstruments");
        }
    }
}
