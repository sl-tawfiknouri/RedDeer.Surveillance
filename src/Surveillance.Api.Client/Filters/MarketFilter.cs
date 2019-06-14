using RedDeer.Surveillance.Api.Client.Infrastructure;

namespace RedDeer.Surveillance.Api.Client.Filters
{
    public class MarketFilter<T> : Filter<MarketFilter<T>, T> where T : Parent
    {
        public MarketFilter(T node) : base(node) { }
    }
}
