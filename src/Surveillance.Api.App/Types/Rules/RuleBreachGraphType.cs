namespace Surveillance.Api.App.Types.Rules
{
    using GraphQL.Authorization;
    using GraphQL.DataLoader;
    using GraphQL.Types;

    using Surveillance.Api.App.Authorization;
    using Surveillance.Api.App.Types.Engine;
    using Surveillance.Api.App.Types.Trading;
    using Surveillance.Api.DataAccess.Abstractions.Entities;
    using Surveillance.Api.DataAccess.Abstractions.Repositories;

    public class RuleBreachGraphType : ObjectGraphType<IRuleBreach>
    {
        public RuleBreachGraphType(
            IOrderRepository orders,
            ISystemProcessOperationRepository operationRepository,
            IDataLoaderContextAccessor dataLoaderAccessor)
        {
            this.AuthorizeWith(PolicyManifest.UserPolicy);

            this.Field(t => t.Id).Description("Primary Key");
            this.Field(t => t.CorrelationId).Description("Correlation identifier for rule runs");
            this.Field(t => t.IsBackTest).Description("Was the rule run part of a back test?");
            this.Field(t => t.CreatedOn).Type(new DateTimeGraphType())
                .Description("Rule breach created by the rule engine");
            this.Field(t => t.Title).Description("Title description of the rule breach");
            this.Field(t => t.Description).Description("Detailed explanation of why the rule was breached");
            this.Field(t => t.Venue).Description("Name of the market that the rule breach occurred on");
            this.Field(t => t.StartOfPeriodUnderInvestigation).Type(new DateTimeGraphType())
                .Description("Oldest order in the rule breach orders");
            this.Field(t => t.EndOfPeriodUnderInvestigation).Type(new DateTimeGraphType())
                .Description("Youngest order in the rule breach orders");
            this.Field(t => t.AssetCfi).Description(
                "Asset CFI code. Six letters to categorise the type of asset such as E for equities or D for debt instruments");
            this.Field(t => t.ReddeerEnrichmentId)
                .Description("Identifier for the financial instrument in the security master list");
            this.Field(t => t.RuleId).Description("Rule run id");

            this.Field<OrganisationTypeEnumGraphType>(
                "organisationFactor",
                resolve: context => context.Source.OrganisationFactor);
            this.Field(t => t.OrganisationalFactorValue).Description(
                "The organisational factor value for the organisational factor type i.e. for funds 'The medallion fund'");
            this.Field(t => t.SystemOperationId).Description("sys op id");

            this.Field<SystemProcessOperationGraphType>(
                "processOperation",
                resolve: context =>
                    {
                        var loader = dataLoaderAccessor.Context.GetOrAddLoader(
                            $"GetSystemProcessOperationById-{context.Source.SystemOperationId}",
                            () => operationRepository.GetForId(context.Source.SystemOperationId));

                        return loader.LoadAsync();
                    });

            this.Field<ListGraphType<OrderGraphType>>(
                "orders",
                resolve: context =>
                    {
                        var loader = dataLoaderAccessor.Context.GetOrAddCollectionBatchLoader<int, IOrder>(
                            $"GetRuleBreachOrdersByRuleBreachId-{context.Source.Id}",
                            orders.GetAllForRuleBreach);

                        return loader.LoadAsync(context.Source.Id);
                    });
        }
    }
}