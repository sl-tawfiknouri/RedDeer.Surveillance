using GraphQL.Authorization;
using GraphQL.Types;
using Surveillance.Api.App.Authorization;
using Surveillance.Api.DataAccess.Abstractions.Entities;

namespace Surveillance.Api.App.Types.Trading
{
    public class OrderAllocationGraphType 
        : ObjectGraphType<IOrdersAllocation>
    {
        public OrderAllocationGraphType()
        {
            this.AuthorizeWith(PolicyManifest.UserPolicy);

            Field(i => i.Id).Description("Identifier for the order allocation");
            Field(i => i.OrderId).Description("Order Id");
            Field(i => i.Fund, nullable: true).Description("Fund");
            Field(i => i.Strategy, nullable: true).Description("Strategy");
            Field(i => i.ClientAccountId, nullable: true).Description("Client Account Id");
            Field(i => i.OrderFilledVolume, nullable: true).Description("Order Filled Volume");
            Field(i => i.Live).Description("Live");
            Field(i => i.AutoScheduled).Description("Auto Scheduled");
            Field(i => i.CreatedDate).Type(new DateTimeGraphType()).Description("Created Date");
        }
    }
}
