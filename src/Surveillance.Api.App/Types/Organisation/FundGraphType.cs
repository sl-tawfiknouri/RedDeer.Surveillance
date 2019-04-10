using GraphQL.Authorization;
using GraphQL.DataLoader;
using GraphQL.Types;
using Surveillance.Api.App.Authorization;
using Surveillance.Api.App.Types.Trading;
using Surveillance.Api.DataAccess.Abstractions.Entities;
using Surveillance.Api.DataAccess.Abstractions.Repositories;

namespace Surveillance.Api.App.Types.Organisation
{
    public class FundGraphType : ObjectGraphType<IFund>
    {
        public FundGraphType(IOrderRepository repository, IDataLoaderContextAccessor dataLoaderAccessor)
        {
            this.AuthorizeWith(PolicyManifest.UserPolicy);

            Field(t => t.Name).Description("Fund name");
            Field<ListGraphType<OrderLedgerGraphType>>(
                "Portfolio",
                resolve: context =>
                {
                    var loader = dataLoaderAccessor.Context.GetOrAddCollectionBatchLoader<string, IOrderLedger>(
                        $"GetPortfolioByFund-{context.Source.Name}", repository.GetForFund);

                    return loader.LoadAsync(context.Source.Name);
                });
        }
    }
}
