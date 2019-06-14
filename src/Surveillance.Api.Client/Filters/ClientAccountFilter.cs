using RedDeer.Surveillance.Api.Client.Infrastructure;

namespace RedDeer.Surveillance.Api.Client.Filters
{
    public class ClientAccountFilter<T> : Filter<ClientAccountFilter<T>, T> where T : Parent
    {
        public ClientAccountFilter(T node) : base(node) { }
    }
}
