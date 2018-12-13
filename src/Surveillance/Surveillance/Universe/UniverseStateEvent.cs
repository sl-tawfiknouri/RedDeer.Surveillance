using System.ComponentModel;

namespace Surveillance.Universe
{
    /// <summary>
    /// These state events are expected to be 1:1 mapping with deserialisable types
    /// Such as a trade being deserialised to TradeOrderFrame
    /// DO NOT CHANGE without updating ========== UniverseEventComparer ==========
    /// </summary>
    public enum UniverseStateEvent
    {
        [Description("Unknown")]
        Unknown, // A tree fell in a forest with no observers
        [Description("Genesis")]
        Genesis, // The, or at least a - beginning of the universe
        [Description("Eschaton")]
        Eschaton, // The end of the universe
        [Description("Trade")]
        TradeReddeer, // A trade happened (in reddeer format)
        [Description("Trade Placed")]
        TradeReddeerSubmitted, // A trade was initially submitted (in reddeer format)
        [Description("Stock Tick")]
        StockTickReddeer, // A stock tick happened (in reddeer format)
        [Description("Stock Market Open")]
        StockMarketOpen, // A stock market has opened
        [Description("Stock Market Close")]
        StockMarketClose, // a stock market has closed
    }
}
