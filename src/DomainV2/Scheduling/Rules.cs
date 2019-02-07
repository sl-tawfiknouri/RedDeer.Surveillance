using System.ComponentModel;

namespace DomainV2.Scheduling
{
    public enum Rules
    {
        [Description("Spoofing")]
        Spoofing = 0,
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
        [Description("Front Running")]
        FrontRunning,
        [Description("Painting The Tape")]
        PaintingTheTape,
        [Description("Improper Matched Orders")]
        ImproperMatchedOrders,
        [Description("Cross Asset Manipulation")]
        CrossAssetManipulation,
        [Description("Pump And Dump")]
        PumpAndDump,
        [Description("Trash And Cash")]
        TrashAndCash
    }
}