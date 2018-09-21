namespace Surveillance.Universe
{
    /// <summary>
    /// These state events are expected to be 1:1 mapping with deserialisable types
    /// Such as a trade being deserialised to TradeOrderFrame
    /// </summary>
    public enum UniverseStateEvent
    {
        Genesis, // The, or at least a - beginning of the universe
        Eschaton, // The end of the universe
        Unknown, // A tree fell in a forest with no observers
        TradeReddeer, // A trade happened (in reddeer format)
        StockTickReddeer, // A stock tick happened (in reddeer format)
    }
}
