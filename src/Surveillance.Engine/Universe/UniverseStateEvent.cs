namespace Surveillance.Engine.Rules.Universe
{
    using System.ComponentModel;

    /// <summary>
    ///     These state events are expected to be 1:1 mapping with deserialisable types
    ///     Such as a trade being deserialised to TradeOrderFrame
    ///     DO NOT CHANGE without updating ========== UniverseEventComparer ==========
    /// </summary>
    public enum UniverseStateEvent
    {
        [Description("Unknown")]
        Unknown = 0, // A tree fell in a forest with no observers

        [Description("Genesis")]
        Genesis, // The, or at least a - beginning of the universe

        [Description("Eschaton")]
        Eschaton, // The end of the universe

        [Description("Order")]
        Order, // A order 

        [Description("Order Placed")]
        OrderPlaced, // An order submitted

        [Description("Order Filled")]
        OrderFilled, // An order filled

        [Description("Security Data Intra Day Tick (equity)")]
        EquityIntradayTick, // An equity tick

        [Description("Security Data Inter Daily Tick (equity)")]
        EquityInterDayTick, // An equity tick

        [Description("Exchange Open")]
        ExchangeOpen, // A stock market has opened

        [Description("Exchange Close")]
        ExchangeClose, // a stock market has closed

        [Description("Primordial Universe Epoch")]
        EpochPrimordialUniverse, // the time before the real universe occured - allow back filling of events out of the scope of the orders under analysis

        [Description("Real Universe Epoch")]
        EpochRealUniverse, // the time that the rule run is primarily concerned with - orders must be sourced from the real universe timeline

        [Description("Future Universe Epoch")]
        EpochFutureUniverse // the time after the real universe has ended - trades will not exist here but other types of events may
    }

    public static class UniverseStateEventExtensions
    {
        public static bool IsOrderType(this UniverseStateEvent universeEvents)
        {
            return universeEvents == UniverseStateEvent.Order || universeEvents == UniverseStateEvent.OrderFilled
                                                              || universeEvents == UniverseStateEvent.OrderPlaced;
        }
    }
}