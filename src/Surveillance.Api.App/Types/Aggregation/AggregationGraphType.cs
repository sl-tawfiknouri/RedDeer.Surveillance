using GraphQL.Authorization;
using GraphQL.Types;
using Surveillance.Api.App.Authorization;
using Surveillance.Api.DataAccess.Abstractions.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Surveillance.Api.App.Types.Aggregation
{
    public class AggregationGraphType : ObjectGraphType<IAggregation>
    {
        public AggregationGraphType()
        {
            this.AuthorizeWith(PolicyManifest.UserPolicy);

            Name = "Trader";

            Field(i => i.Key).Description("The identifier for aggregation grouping");
            Field(i => i.Count).Description("The count value of the aggregation grouping");
        }
    }
}
