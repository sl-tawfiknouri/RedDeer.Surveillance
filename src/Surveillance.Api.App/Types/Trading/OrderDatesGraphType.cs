namespace Surveillance.Api.App.Types.Trading
{
    using GraphQL.Authorization;
    using GraphQL.Types;

    using Surveillance.Api.App.Authorization;
    using Surveillance.Api.DataAccess.Abstractions.Entities;

    public class OrderDatesGraphType : ObjectGraphType<IOrderDates>
    {
        public OrderDatesGraphType()
        {
            this.AuthorizeWith(PolicyManifest.UserPolicy);
            this.Field(i => i.PlacedDate, true).Type(new DateTimeGraphType()).Description("Order placed on");
            this.Field(i => i.BookedDate, true).Type(new DateTimeGraphType())
                .Description("Order booked with internal traders on");
            this.Field(i => i.AmendedDate, true).Type(new DateTimeGraphType()).Description("Order last amended on");
            this.Field(i => i.RejectedDate, true).Type(new DateTimeGraphType())
                .Description("Order rejected by dealing on");
            this.Field(i => i.CancelledDate, true).Type(new DateTimeGraphType())
                .Description("Order cancelled by portfolio manager / trader on");
            this.Field(i => i.FilledDate, true).Type(new DateTimeGraphType()).Description("Order filled on");
            this.Field(i => i.StatusChangedDate, true).Type(new DateTimeGraphType())
                .Description("Order status last changed on");
        }
    }
}