namespace Surveillance.Universe
{
    /// <summary>
    /// These state events are expected to be 1:1 mapping with deserialisable types
    /// Such as a trade being deserialised to TradeOrderFrame
    /// </summary>
    public enum UniverseStateEvent
    {
        Unknown, // A tree fell in a forest with no observers
        TradeReddeer, // a trade happened (in reddeer format)
        StockTickReddeer, // a stock tick happened (in reddeer format)
    }
}
