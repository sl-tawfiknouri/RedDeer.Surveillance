namespace Surveillance.Data.Universe
{
    using System.ComponentModel;

    /// <summary>
    ///     These state events are expected to be 1:1 mapping with de serializable types
    ///     Such as a trade being deserialized to TradeOrderFrame
    ///     DO NOT CHANGE without updating ========== UniverseEventComparer ==========
    /// </summary>
    public enum UniverseStateEvent
    {
        /// <summary>
        /// The unknown.
        /// A tree fell in a forest with no observers
        /// </summary>
        [Description("Unknown")]
        Unknown = 0,

        /// <summary>
        /// The genesis.
        /// The, or at least a - beginning of the universe
        /// </summary>
        [Description("Genesis")]
        Genesis,

        /// <summary>
        /// The eschaton.
        /// The end of the universe
        /// </summary>
        [Description("Eschaton")]
        Eschaton,

        /// <summary>
        /// The order.
        /// A order 
        /// </summary>
        [Description("Order")]
        Order,

        /// <summary>
        /// The order placed.
        /// An order submitted
        /// </summary>
        [Description("Order Placed")]
        OrderPlaced,

        /// <summary>
        /// The order filled.
        /// An order filled
        /// </summary>
        [Description("Order Filled")]
        OrderFilled,

        /// <summary>
        /// The equity intraday tick.
        /// An equity tick
        /// </summary>
        [Description("Security Data Intra Day Tick (equity)")]
        EquityIntradayTick,

        /// <summary>
        /// The equity inter day tick.
        /// An equity tick
        /// </summary>
        [Description("Security Data Inter Daily Tick (equity)")]
        EquityInterDayTick,

        /// <summary>
        /// The exchange open.
        /// A stock market has opened
        /// </summary>
        [Description("Exchange Open")]
        ExchangeOpen,

        /// <summary>
        /// The exchange close.
        /// a stock market has closed
        /// </summary>
        [Description("Exchange Close")]
        ExchangeClose,

        /// <summary>
        /// The epoch primordial universe.
        /// the time before the real universe occurred - allow back filling of events out of the scope of the orders under analysis
        /// </summary>
        [Description("Primordial Universe Epoch")]
        EpochPrimordialUniverse,

        /// <summary>
        /// The epoch real universe.
        /// the time that the rule run is primarily concerned with - orders must be sourced from the real universe timeline
        /// </summary>
        [Description("Real Universe Epoch")]
        EpochRealUniverse,

        /// <summary>
        /// The epoch future universe.
        /// the time after the real universe has ended - trades will not exist here but other types of events may
        /// </summary>
        [Description("Future Universe Epoch")]
        EpochFutureUniverse
    }

    /// <summary>
    /// The universe state event extensions.
    /// </summary>
    public static class UniverseStateEventExtensions
    {
        /// <summary>
        /// The is order type.
        /// </summary>
        /// <param name="universeEvents">
        /// The universe events.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public static bool IsOrderType(this UniverseStateEvent universeEvents)
        {
            return universeEvents == UniverseStateEvent.Order 
                   || universeEvents == UniverseStateEvent.OrderFilled
                   || universeEvents == UniverseStateEvent.OrderPlaced;
        }
    }
}