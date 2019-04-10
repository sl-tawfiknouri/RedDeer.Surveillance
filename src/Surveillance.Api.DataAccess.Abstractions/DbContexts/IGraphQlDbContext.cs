using System;
using System.Linq;
using Surveillance.Api.DataAccess.Abstractions.Entities;

namespace Surveillance.Api.DataAccess.Abstractions.DbContexts
{
    public interface IGraphQlDbContext : IDisposable
    {
        IQueryable<IMarket> Market { get; }
        IQueryable<IOrder> Orders { get; }
        IQueryable<IRuleBreachOrder> RuleBreachOrders { get; }
        IQueryable<IOrdersAllocation> OrdersAllocation { get; }
        IQueryable<IRuleBreach> RuleBreach { get; }
        IQueryable<ISystemProcessOperationRuleRun> RuleRun { get; }
        IQueryable<ISystemProcessOperationUploadFile> UploadFile { get; }
        IQueryable<ISystemProcessOperationDataSynchroniser> DataSynchroniser { get; }
        IQueryable<ISystemProcessOperationDistributeRule> DistributeRule { get; }
        IQueryable<ISystemProcessOperation> ProcessOperation { get; }
        IQueryable<ISystemProcess> SystemProcess { get; }
        IQueryable<IFinancialInstrument> FinancialInstrument { get; }
    }
}