using System.ComponentModel;

namespace Domain.Surveillance.Scheduling
{
    public enum Rules
    {
        [Description("Equities Spoofing")]
        Spoofing = 0,
        [Description("Equities Cancelled Orders")]
        CancelledOrders,
        [Description("Equities High Profits")]
        HighProfits,
        [Description("Equities Marking the Close")]
        MarkingTheClose,
        [Description("Equities Layering")]
        Layering,
        [Description("Equities High Volume")]
        HighVolume,
        [Description("Equities Wash Trades")]
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
        TrashAndCash,
        [Description("Fixed Income High Profits")]
        FixedIncomeHighProfits,
        [Description("Fixed Income High Volume Issuance")]
        FixedIncomeHighVolumeIssuance,
        [Description("Fixed Income Wash Trades")]
        FixedIncomeWashTrades,
        [Description("Ramping")]
        Ramping
    }
}