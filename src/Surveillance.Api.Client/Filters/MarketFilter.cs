namespace RedDeer.Surveillance.Api.Client.Filters
{
    using RedDeer.Surveillance.Api.Client.Infrastructure;

    public class MarketFilter<T> : Filter<MarketFilter<T>, T>
        where T : Parent
    {
        public MarketFilter(T node)
            : base(node)
        {
        }
    }
}