using GraphQL.Authorization;
using GraphQL.Types;
using Surveillance.Api.App.Authorization;
using Surveillance.Api.DataAccess.Abstractions.Entities;

namespace Surveillance.Api.App.Types.Trading
{
    public class OrderDatesGraphType : ObjectGraphType<IOrderDates>
    {
        public OrderDatesGraphType()
        {
            this.AuthorizeWith(PolicyManifest.UserPolicy);
            Field(i => i.PlacedDate, nullable: true).Type(new DateTimeGraphType()).Description("Order placed on");
            Field(i => i.BookedDate, nullable: true).Type(new DateTimeGraphType()).Description("Order booked with internal traders on");
            Field(i => i.AmendedDate, nullable: true).Type(new DateTimeGraphType()).Description("Order last amended on");
            Field(i => i.RejectedDate, nullable: true).Type(new DateTimeGraphType()).Description("Order rejected by dealing on");
            Field(i => i.CancelledDate, nullable: true).Type(new DateTimeGraphType()).Description("Order cancelled by portfolio manager / trader on");
            Field(i => i.FilledDate, nullable: true).Type(new DateTimeGraphType()).Description("Order filled on");
            Field(i => i.StatusChangedDate, nullable: true).Type(new DateTimeGraphType()).Description("Order status last changed on");
        }
    }
}
