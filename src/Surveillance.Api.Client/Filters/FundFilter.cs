using RedDeer.Surveillance.Api.Client.Infrastructure;

namespace RedDeer.Surveillance.Api.Client.Filters
{
    public class FundFilter<T> : Filter<FundFilter<T>, T> where T : Parent
    {
        public FundFilter(T node) : base(node) { }
    }
}
