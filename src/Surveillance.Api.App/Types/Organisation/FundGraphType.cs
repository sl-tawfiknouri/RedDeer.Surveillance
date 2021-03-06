﻿namespace Surveillance.Api.App.Types.Organisation
{
    using GraphQL.Authorization;
    using GraphQL.DataLoader;
    using GraphQL.Types;

    using Surveillance.Api.App.Authorization;
    using Surveillance.Api.App.Types.Trading;
    using Surveillance.Api.DataAccess.Abstractions.Entities;
    using Surveillance.Api.DataAccess.Abstractions.Repositories;

    public class FundGraphType : ObjectGraphType<IFund>
    {
        public FundGraphType(IOrderRepository repository, IDataLoaderContextAccessor dataLoaderAccessor)
        {
            this.AuthorizeWith(PolicyManifest.UserPolicy);

            this.Field(t => t.Name).Description("Fund name");
            this.Field<ListGraphType<OrderLedgerGraphType>>(
                "portfolio",
                resolve: context =>
                    {
                        var loader = dataLoaderAccessor.Context.GetOrAddCollectionBatchLoader<string, IOrderLedger>(
                            $"GetPortfolioByFund-{context.Source.Name}",
                            repository.GetForFund);

                        return loader.LoadAsync(context.Source.Name);
                    });
        }
    }
}