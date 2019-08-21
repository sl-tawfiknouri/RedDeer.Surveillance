namespace Surveillance.Api.App.Types.Trading
{
    using GraphQL.Authorization;
    using GraphQL.Types;

    using Surveillance.Api.App.Authorization;
    using Surveillance.Api.DataAccess.Abstractions.Entities;

    public class OrderAllocationGraphType : ObjectGraphType<IOrdersAllocation>
    {
        public OrderAllocationGraphType()
        {
            this.AuthorizeWith(PolicyManifest.UserPolicy);

            this.Field(i => i.Id).Description("Identifier for the order allocation");
            this.Field(i => i.OrderId).Description("Order Id");
            this.Field(i => i.Fund, true).Description("Fund");
            this.Field(i => i.Strategy, true).Description("Strategy");
            this.Field(i => i.ClientAccountId, true).Description("Client Account Id");
            this.Field(i => i.OrderFilledVolume, true).Description("Order Filled Volume");
            this.Field(i => i.Live).Description("Live");
            this.Field(i => i.AutoScheduled).Description("Auto Scheduled");
            this.Field(i => i.CreatedDate).Type(new DateTimeGraphType()).Description("Created Date");
        }
    }
}