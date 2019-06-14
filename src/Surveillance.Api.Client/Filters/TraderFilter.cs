using RedDeer.Surveillance.Api.Client.Infrastructure;

namespace RedDeer.Surveillance.Api.Client.Filters
{
    public class TraderFilter<T> : Filter<TraderFilter<T>, T> where T : Parent
    {
        public TraderFilter(T node) : base(node) { }
    }
}
