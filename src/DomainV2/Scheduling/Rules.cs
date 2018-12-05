using System.ComponentModel;

namespace DomainV2.Scheduling
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
        HighVolume,
        [Description("Wash Trades")]
        WashTrade,
        [Description("Universe Filter")]
        UniverseFilter,
    }
}