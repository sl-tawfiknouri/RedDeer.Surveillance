namespace RedDeer.Surveillance.Api.Client.Filters
{
    using RedDeer.Surveillance.Api.Client.Infrastructure;

    public class StrategyFilter<T> : Filter<StrategyFilter<T>, T>
        where T : Parent
    {
        public StrategyFilter(T node)
            : base(node)
        {
        }
    }
}