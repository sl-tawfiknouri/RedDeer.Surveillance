namespace RedDeer.Surveillance.Api.Client.Filters
{
    using RedDeer.Surveillance.Api.Client.Infrastructure;

    public class ClientAccountFilter<T> : Filter<ClientAccountFilter<T>, T>
        where T : Parent
    {
        public ClientAccountFilter(T node)
            : base(node)
        {
        }
    }
}