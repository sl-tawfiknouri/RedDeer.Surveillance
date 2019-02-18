using System.ComponentModel;

namespace Domain.Financial
{
    public enum OrderStatus
    {
        [Description("Unknown")]
        Unknown = 0,
        [Description("Placed")]
        Placed = 1,
        [Description("Booked")]
        Booked = 2,
        [Description("Amended")]
        Amended = 3,
        [Description("Rejected")]
        Rejected = 4,
        [Description("Cancelled")]
        Cancelled = 5,
        [Description("Filled")]
        Filled = 6
    }
}
