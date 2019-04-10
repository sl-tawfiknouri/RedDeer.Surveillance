using GraphQL.Types;
using Surveillance.Api.DataAccess.Abstractions.Entities;

namespace Surveillance.Api.App.Types.Trading
{
    public class OrderDatesGraphType : ObjectGraphType<IOrderDates>
    {
        public OrderDatesGraphType()
        {
            Field(i => i.PlacedDate).Description("Order placed on");
            Field(i => i.BookedDate).Description("Order booked with internal traders on");
            Field(i => i.AmendedDate).Description("Order last amended on");
            Field(i => i.RejectedDate).Description("Order rejected by dealing on");
            Field(i => i.CancelledDate).Description("Order cancelled by portfolio manager / trader on");
            Field(i => i.FilledDate).Description("Order filled on");
            Field(i => i.StatusChangedDate).Description("Order status last changed on");
        }
    }
}
