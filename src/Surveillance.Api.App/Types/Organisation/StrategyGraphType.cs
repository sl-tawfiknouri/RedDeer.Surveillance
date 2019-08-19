namespace Surveillance.Api.App.Types.Organisation
{
    using GraphQL.Authorization;
    using GraphQL.DataLoader;
    using GraphQL.Types;

    using Surveillance.Api.App.Authorization;
    using Surveillance.Api.App.Types.Trading;
    using Surveillance.Api.DataAccess.Abstractions.Entities;
    using Surveillance.Api.DataAccess.Abstractions.Repositories;

    public class StrategyGraphType : ObjectGraphType<IStrategy>
    {
        public StrategyGraphType(IOrderRepository repository, IDataLoaderContextAccessor dataLoaderAccessor)
        {
            this.AuthorizeWith(PolicyManifest.UserPolicy);

            this.Field(i => i.Name).Description("Name of the strategy");

            this.Field<ListGraphType<OrderLedgerGraphType>>(
                "portfolio",
                resolve: context =>
                    {
                        var loader = dataLoaderAccessor.Context.GetOrAddCollectionBatchLoader<string, IOrderLedger>(
                            $"GetPortfolioByStrategy-{context.Source.Name}",
                            repository.GetForStrategy);

                        return loader.LoadAsync(context.Source.Name);
                    });
        }
    }
}