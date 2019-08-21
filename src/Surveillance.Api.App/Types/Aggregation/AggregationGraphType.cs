namespace Surveillance.Api.App.Types.Aggregation
{
    using GraphQL.Authorization;
    using GraphQL.Types;

    using Surveillance.Api.App.Authorization;
    using Surveillance.Api.DataAccess.Abstractions.Entities;

    public class AggregationGraphType : ObjectGraphType<IAggregation>
    {
        public AggregationGraphType()
        {
            this.AuthorizeWith(PolicyManifest.UserPolicy);

            this.Name = "Trader";

            this.Field(i => i.Key).Description("The identifier for aggregation grouping");
            this.Field(i => i.Count).Description("The count value of the aggregation grouping");
        }
    }
}