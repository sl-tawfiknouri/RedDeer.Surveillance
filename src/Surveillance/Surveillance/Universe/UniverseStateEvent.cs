namespace Surveillance.Universe
{
    /// <summary>
    /// These state events are expected to be 1:1 mapping with deserialisable types
    /// Such as a trade being deserialised to TradeOrderFrame
    /// </summary>
    public enum UniverseStateEvent
    {
        Unknown, // A tree fell in a forest with no observers
        Genesis, // The, or at least a - beginning of the universe
        Eschaton, // The end of the universe
        TradeReddeer, // A trade happened (in reddeer format)
        TradeReddeerSubmitted, // A trade was initially submitted (in reddeer format)
        StockTickReddeer, // A stock tick happened (in reddeer format)
        StockMarketOpen, // A stock market has opened
        StockMarketClose, // a stock market has closed
    }
}
