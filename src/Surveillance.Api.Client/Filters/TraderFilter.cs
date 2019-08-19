namespace RedDeer.Surveillance.Api.Client.Filters
{
    using RedDeer.Surveillance.Api.Client.Infrastructure;

    public class TraderFilter<T> : Filter<TraderFilter<T>, T>
        where T : Parent
    {
        public TraderFilter(T node)
            : base(node)
        {
        }
    }
}