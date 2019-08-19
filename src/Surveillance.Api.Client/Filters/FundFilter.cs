namespace RedDeer.Surveillance.Api.Client.Filters
{
    using RedDeer.Surveillance.Api.Client.Infrastructure;

    public class FundFilter<T> : Filter<FundFilter<T>, T>
        where T : Parent
    {
        public FundFilter(T node)
            : base(node)
        {
        }
    }
}