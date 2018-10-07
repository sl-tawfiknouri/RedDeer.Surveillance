// ReSharper disable UnusedMember.Global
namespace Domain.Trades.Orders
{
    public enum OrderStatus
    {
        Booked = 0,
        Cancelled = 1,
        PartialFulfilled = 2,
        Fulfilled = 3,
        CallAmended = 4,
        Rejected = 5,
        CancelledPostBooking = 6, 
    }
}
