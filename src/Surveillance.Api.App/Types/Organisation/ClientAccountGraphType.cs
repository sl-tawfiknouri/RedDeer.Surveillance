namespace Surveillance.Api.App.Types.Organisation
{
    using GraphQL.Authorization;
    using GraphQL.DataLoader;
    using GraphQL.Types;

    using Surveillance.Api.App.Authorization;
    using Surveillance.Api.App.Types.Trading;
    using Surveillance.Api.DataAccess.Abstractions.Entities;
    using Surveillance.Api.DataAccess.Abstractions.Repositories;

    public class ClientAccountGraphType : ObjectGraphType<IClientAccount>
    {
        public ClientAccountGraphType(IOrderRepository repository, IDataLoaderContextAccessor dataLoaderAccessor)
        {
            this.AuthorizeWith(PolicyManifest.UserPolicy);

            this.Field(i => i.Id).Description("Identifier for the client account");
            this.Field<ListGraphType<OrderLedgerGraphType>>(
                "portfolio",
                resolve: context =>
                    {
                        var loader = dataLoaderAccessor.Context.GetOrAddCollectionBatchLoader<string, IOrderLedger>(
                            $"GetPortfolioByClientAccount-{context.Source.Id}",
                            repository.GetForClientAccount);

                        return loader.LoadAsync(context.Source.Id);
                    });
        }
    }
}