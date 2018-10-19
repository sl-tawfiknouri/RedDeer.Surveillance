using System.ComponentModel;

namespace Domain.Scheduling
{
    public enum Rules
    {
        [Description("Spoofing")]
        Spoofing,
        [Description("Cancelled Orders")]
        CancelledOrders,
        [Description("High Profits")]
        HighProfits,
        [Description("Marking the Close")]
        MarkingTheClose,
        [Description("Layering")]
        Layering,
        [Description("High Volume")]
        HighVolume
    }
}