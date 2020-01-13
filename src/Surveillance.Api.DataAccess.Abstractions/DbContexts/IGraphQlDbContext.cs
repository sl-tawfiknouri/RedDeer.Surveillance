namespace Surveillance.Api.DataAccess.Abstractions.DbContexts
{
    using System;
    using System.Linq;

    using Surveillance.Api.DataAccess.Abstractions.Entities;

    public interface IGraphQlDbContext : IDisposable
    {
        IQueryable<IBroker> Broker { get; }

        IQueryable<ISystemProcessOperationDataSynchroniser> DataSynchroniser { get; }

        IQueryable<ISystemProcessOperationDistributeRule> DistributeRule { get; }

        IQueryable<IFinancialInstrument> FinancialInstrument { get; }

        IQueryable<IMarket> Market { get; }

        IQueryable<IOrder> Orders { get; }

        IQueryable<IOrdersAllocation> OrdersAllocation { get; }

        IQueryable<ISystemProcessOperation> ProcessOperation { get; }

        IQueryable<IRuleBreach> RuleBreach { get; }

        IQueryable<IRuleBreachOrder> RuleBreachOrders { get; }

        IQueryable<ISystemProcessOperationRuleRun> RuleRun { get; }

        IQueryable<ISystemProcess> SystemProcess { get; }

        IQueryable<ISystemProcessOperationUploadFile> UploadFile { get; }

        IQueryable<IRuleDataRequest> RuleDataRequest { get; }
    }
}