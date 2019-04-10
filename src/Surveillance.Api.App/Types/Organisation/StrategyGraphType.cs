using GraphQL.Authorization;
using GraphQL.DataLoader;
using GraphQL.Types;
using Surveillance.Api.App.Authorization;
using Surveillance.Api.App.Types.Trading;
using Surveillance.Api.DataAccess.Abstractions.Entities;
using Surveillance.Api.DataAccess.Abstractions.Repositories;

namespace Surveillance.Api.App.Types.Organisation
{
    public class StrategyGraphType : ObjectGraphType<IStrategy>
    {
        public StrategyGraphType(IOrderRepository repository, IDataLoaderContextAccessor dataLoaderAccessor)
        {
            this.AuthorizeWith(PolicyManifest.UserPolicy);

            Field(i => i.Name).Description("Name of the strategy");

            Field<ListGraphType<OrderLedgerGraphType>>(
                "Portfolio",
                resolve: context =>
                {
                    var loader = dataLoaderAccessor.Context.GetOrAddCollectionBatchLoader<string, IOrderLedger>(
                        $"GetPortfolioByStrategy-{context.Source.Name}", repository.GetForStrategy);

                    return loader.LoadAsync(context.Source.Name);
                });
        }
    }
}
