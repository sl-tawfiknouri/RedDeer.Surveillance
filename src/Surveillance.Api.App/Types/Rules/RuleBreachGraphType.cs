using GraphQL.Authorization;
using GraphQL.DataLoader;
using GraphQL.Types;
using Surveillance.Api.App.Authorization;
using Surveillance.Api.App.Types.Engine;
using Surveillance.Api.App.Types.Trading;
using Surveillance.Api.DataAccess.Abstractions.Entities;
using Surveillance.Api.DataAccess.Abstractions.Repositories;

namespace Surveillance.Api.App.Types.Rules
{
    public class RuleBreachGraphType : ObjectGraphType<IRuleBreach>
    {
        public RuleBreachGraphType(
            IOrderRepository orders,
            ISystemProcessOperationRepository operationRepository,
            IDataLoaderContextAccessor dataLoaderAccessor)
        {
            this.AuthorizeWith(PolicyManifest.UserPolicy);

            Field(t => t.Id).Description("Primary Key");
            Field(t => t.CorrelationId).Description("Correlation identifier for rule runs");
            Field(t => t.IsBackTest).Description("Was the rule run part of a back test?");
            Field(t => t.Created).Description("Rule breach created by the rule engine");
            Field(t => t.Title).Description("Title description of the rule breach");
            Field(t => t.Description).Description("Detailed explanation of why the rule was breached");
            Field(t => t.Venue).Description("Name of the market that the rule breach occurred on");
            Field(t => t.StartOfRuleBreachPeriod).Description("Oldest order in the rule breach orders");
            Field(t => t.EndOfRuleBreachPeriod).Description("Youngest order in the rule breach orders");
            Field(t => t.AssetCfi).Description("Asset CFI code. Six letters to categorise the type of asset such as E for equities or D for debt instruments");
            Field(t => t.ReddeerEnrichmentId).Description("Identifier for the financial instrument in the security master list");
            Field(t => t.RuleId).Description("Rule run id");

            Field<OrganisationTypeEnumGraphType>("organisationFactor", resolve: context => context.Source.OrganisationFactor);
            Field(t => t.OrganisationalFactorValue).Description("The organisational factor value for the organisational factor type i.e. for funds 'The medallion fund'");
            Field(t => t.SystemOperationId).Description("sys op id");

            Field<SystemProcessOperationGraphType>("processOperation", resolve: context =>
            {
                var loader =
                        dataLoaderAccessor.Context.GetOrAddLoader<ISystemProcessOperation>(
                            $"GetSystemProcessOperationById-{context.Source.SystemOperationId}",
                            () => operationRepository.GetForId(context.Source.SystemOperationId));

                    return loader.LoadAsync();
            });

            Field<ListGraphType<OrderGraphType>>(
                "orders",
                resolve: context =>
                {
                    var loader =
                        dataLoaderAccessor.Context.GetOrAddCollectionBatchLoader<int, IOrder>(
                            $"GetRuleBreachOrdersByRuleBreachId-{context.Source.Id}", orders.GetAllForRuleBreach);

                    return loader.LoadAsync(context.Source.Id);
                });
        }
    }
}
