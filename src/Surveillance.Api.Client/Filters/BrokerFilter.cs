namespace RedDeer.Surveillance.Api.Client.Filters
{
    using RedDeer.Surveillance.Api.Client.Infrastructure;

    /// <summary>
    /// The broker filter.
    /// </summary>
    /// <typeparam name="T">
    /// The node to apply the filter to
    /// </typeparam>
    public class BrokerFilter<T> : Filter<BrokerFilter<T>, T>
        where T : Parent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BrokerFilter{T}"/> class.
        /// </summary>
        /// <param name="node">
        /// The node.
        /// </param>
        public BrokerFilter(T node)
            : base(node)
        {
        }
    }
}