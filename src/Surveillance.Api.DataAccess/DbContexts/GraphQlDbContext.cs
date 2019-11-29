// ReSharper disable InconsistentNaming

namespace Surveillance.Api.DataAccess.DbContexts
{
    using System.Linq;

    using Microsoft.EntityFrameworkCore;

    using Surveillance.Api.DataAccess.Abstractions.DbContexts;
    using Surveillance.Api.DataAccess.Abstractions.Entities;
    using Surveillance.Api.DataAccess.Entities;

    public class GraphQlDbContext : DbContext, IGraphQlDbContext
    {
        public GraphQlDbContext(DbContextOptions options)
            : base(options)
        {
        }

        public IQueryable<IBroker> Broker => this._broker;

        public IQueryable<ISystemProcessOperationDataSynchroniser> DataSynchroniser => this._dataSynchroniser;

        public IQueryable<ISystemProcessOperationDistributeRule> DistributeRule => this._distributeRule;

        public IQueryable<IFinancialInstrument> FinancialInstrument => this._financialInstrument;

        public IQueryable<IMarket> Market => this._market;

        public IQueryable<IOrder> Orders => this._orders;

        public IQueryable<IOrdersAllocation> OrdersAllocation => this._ordersAllocation;

        public IQueryable<ISystemProcessOperation> ProcessOperation => this._processOperation;

        public IQueryable<IRuleBreach> RuleBreach => this._ruleBreach;

        public IQueryable<IRuleBreachOrder> RuleBreachOrders => this._ruleBreachOrders;

        public IQueryable<ISystemProcessOperationRuleRun> RuleRun => this._ruleRun;

        public IQueryable<ISystemProcess> SystemProcess => this._systemProcess;

        public IQueryable<ISystemProcessOperationUploadFile> UploadFile => this._uploadFile;

        public IQueryable<IRuleDataRequest> RuleDataRequest => this._ruleDataRequest;

        protected virtual DbSet<Broker> _broker { get; set; }

        protected virtual DbSet<SystemProcessOperationDataSynchroniser> _dataSynchroniser { get; set; }

        protected virtual DbSet<SystemProcessOperationDistributeRule> _distributeRule { get; set; }

        protected virtual DbSet<FinancialInstrument> _financialInstrument { get; set; }

        protected virtual DbSet<Market> _market { get; set; }

        protected virtual DbSet<Order> _orders { get; set; }

        protected virtual DbSet<OrdersAllocation> _ordersAllocation { get; set; }

        protected virtual DbSet<SystemProcessOperation> _processOperation { get; set; }

        protected virtual DbSet<RuleBreach> _ruleBreach { get; set; }

        protected virtual DbSet<RuleBreachOrder> _ruleBreachOrders { get; set; }

        protected virtual DbSet<SystemProcessOperationRuleRun> _ruleRun { get; set; }

        protected virtual DbSet<SystemProcess> _systemProcess { get; set; }

        protected virtual DbSet<SystemProcessOperationUploadFile> _uploadFile { get; set; }

        protected virtual DbSet<RuleDataRequest> _ruleDataRequest { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Market>().ToTable("Market");
            modelBuilder.Entity<Broker>().ToTable("Brokers");
            modelBuilder.Entity<Order>().ToTable("Orders");
            modelBuilder.Entity<OrdersAllocation>().ToTable("OrdersAllocation");
            modelBuilder.Entity<RuleBreach>().ToTable("RuleBreach");
            modelBuilder.Entity<RuleBreachOrder>().ToTable("RuleBreachOrders")
                .HasKey(c => new { c.RuleBreachId, c.OrderId });
            modelBuilder.Entity<SystemProcess>().ToTable("SystemProcess");
            modelBuilder.Entity<SystemProcessOperation>().ToTable("SystemProcessOperation");
            modelBuilder.Entity<SystemProcessOperationRuleRun>().ToTable("SystemProcessOperationRuleRun");
            modelBuilder.Entity<SystemProcessOperationUploadFile>().ToTable("SystemProcessOperationUploadFile");
            modelBuilder.Entity<SystemProcessOperationDataSynchroniser>()
                .ToTable("SystemProcessOperationDataSynchroniserRequest");
            modelBuilder.Entity<SystemProcessOperationDistributeRule>().ToTable("SystemProcessOperationDistributeRule");
            modelBuilder.Entity<FinancialInstrument>().ToTable("FinancialInstruments");
            modelBuilder.Entity<RuleDataRequest>().ToTable("RuleDataRequest");
        }
    }
}