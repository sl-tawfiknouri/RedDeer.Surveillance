namespace Domain.Trades.Orders
{
    public enum OrderStatus
    {
        Booked,
        Cancelled,
        PartialFulfilled,
        Fulfilled,
        CallAmended,
        Rejected,
        CancelledPostBooking,
    }
}
